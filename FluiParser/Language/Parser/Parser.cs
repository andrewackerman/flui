using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluiParser.Language.Syntax;
using FluiParser.Language.Syntax.Nodes;

namespace FluiParser.Language.Parser
{
    public sealed class Parser
    {
        private static Lazy<Parser> _inst = new Lazy<Parser>();
        public static Parser Instance => _inst.Value;

        private static readonly string[] rootNodes = { "Stateless", "Stateful" };

        private bool _error;
        private ErrorSink _errorSink;
        private int _index;
        private SourceCode _sourceCode;
        private ParserOptions _options;
        private IEnumerable<Token> _tokens;

        private Token _current => _tokens.ElementAtOrDefault(_index) ?? _tokens.Last();
        private Token _last => Peek(-1);
        private Token _next => Peek(1);

        public bool Error => _error;

        public Parser() : this(new ErrorSink()) { }
        public Parser(ErrorSink errorSink)
        {
            _errorSink = errorSink;
        }

        public SourceDocument ParseFile(SourceCode code, IEnumerable<Token> tokens) => ParseFile(code, tokens, ParserOptions.Default);
        public SourceDocument ParseFile(SourceCode code, IEnumerable<Token> tokens, ParserOptions options)
        {
            InitializeParser(code, tokens, options);

            try
            {
                return ParseDocument();
            }
            catch (SyntaxException ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        private void InitializeParser(SourceCode sourceCode, IEnumerable<Token> tokens, ParserOptions options)
        {
            _sourceCode = sourceCode;
            _tokens = tokens.Where(t => !t.IsTrivia);
            _tokens = _tokens.Where((t, i) => {
                if (t == TokenKind.Indentation)
                {
                    Token last = _tokens.ElementAtOrDefault(i - 1);
                    Token next = _tokens.ElementAtOrDefault(i + 1);
                    if ((last == null || last == TokenKind.NewLine) && (next == null || next == TokenKind.NewLine))
                    {
                        return false;
                    }
                }
                return true;
            }).ToArray();
            _options = ParserOptions.Default;
            _index = 0;
        }
        
        #region Navigation
        private Token Peek(int ahead) => _tokens.ElementAtOrDefault(_index + ahead) ?? _tokens.Last();

        private void Advance() => _index++;
        private void AdvanceNewLine()
        {
            while (_current == TokenKind.NewLine)
            {
                Take(TokenKind.NewLine);
            }
        }

        private Token Take()
        {
            var token = _current;
            Advance();
            return token;
        }

        private Token Take(TokenKind kind)
        {
            if (_current != kind) throw UnexpectedToken(kind);
            return Take();
        }

        private Token Take(params TokenKind[] kinds)
        {
            for (int i = 0; i < kinds.Length; i++)
            {
                if (_current == kinds[i])
                {
                    return Take();
                }
            }

            throw UnexpectedToken(kinds);
        }

        private Token Take(string contextualKeyword)
        {
            if (_current != TokenKind.Identifier && _current != contextualKeyword) throw UnexpectedToken(contextualKeyword);
            return Take();
        }

        private Token TakeColon()
        {
            if (_options.EnforceColons || _current == TokenKind.Colon)
            {
                return Take(TokenKind.Colon);
            }
            return _current;
        }
        #endregion

        #region Node Parsing
        private SourceDocument ParseDocument()
        {
            WidgetKind kind;

            var token = Take(TokenKind.Identifier);
            if (!Enum.TryParse(token.Value, out kind))
            {
                throw SyntaxError(Severity.Fatal, "Root node identifier must indicate a widget type (e.g. Stateful, Stateless)");
            }

            var viewModelClass = ParseAttribute(0) as AttributeSingleNode;
            if (viewModelClass == null || viewModelClass.Value != "viewModel")
            {
                throw SyntaxError(Severity.Fatal, "Root node must have a specified `viewModel` attribute with a single identifier as a value");
            }

            Take(TokenKind.Comma);

            var viewClass = ParseAttribute(0) as AttributeSingleNode;
            if (viewClass == null || viewClass.Value != "view")
            {
                throw SyntaxError(Severity.Fatal, "Root node must have a specified `view` attribute with a single child widget as a value");
            }

            if (!(viewClass.Child is ElementSingleNode))
            {
                throw SyntaxError(Severity.Fatal, "The `view` attribute of the root ndoe must have a single widget as a child");
            }

            //if (_current != TokenKind.EndOfFile)
            //{
            //    try
            //    {
            //        root = ParseNode() as ElementNode;
            //    }
            //    catch (SyntaxException) { }
            //}

            return new SourceDocument(_sourceCode, kind, viewModelClass, viewClass);
        }

        private SyntaxNode ParseNode(int indentLevel = 0)
        {
            SyntaxNode node;
            
            if (_current == TokenKind.Indentation)
            {
                var indent = _current;
                Advance();
                indentLevel = indent.Value.Length;
            }

            if (_current == TokenCategory.Constant)
            {
                node = ParseConstant();
            }
            else if (_current == TokenKind.Dot)
            {
                node = ParseAttribute(indentLevel);
            }
            else if (_current == TokenKind.DollarSign)
            {
                node = ParseFunctionCall();
            }
            else if (_current == TokenKind.Ampersand)
            {
                node = ParseCallback();
            }
            else
            {
                node = ParseElement(indentLevel);
            }

            return node;
        }

        private SyntaxNode ParseElement(int indentLevel)
        {
            Token start = Take(TokenKind.Identifier, TokenKind.Widget);

            #region Check if Identifier
            if (_current == TokenKind.Comma)
            {
                return new IdentifierNode(CreateSpan(start, start), start.Value);
            }

            if (_current != TokenKind.Dot && _current != TokenKind.Colon)
            {
                if (_current == TokenKind.EndOfFile)
                {
                    return new IdentifierNode(CreateSpan(start, start), start.Value);
                }

                if (_current == TokenKind.NewLine)
                {
                    Token next = Peek(1);
                    if (next != TokenKind.Indentation || next.Span.Length <= indentLevel)
                    {
                        return new IdentifierNode(CreateSpan(start, start), start.Value);
                    }
                }
            }
            #endregion

            List<SyntaxNode> children = new List<SyntaxNode>();
            SyntaxNode last = null;
            bool parsingChildNodes = true;

            do
            {
                while (_current != TokenKind.Colon && _current != TokenKind.Indentation && _current != TokenKind.NewLine && _current != TokenKind.EndOfFile)
                {
                    last = ParseNode(indentLevel);
                    children.Add(last);
                    if (_current == TokenKind.Comma)
                    {
                        Take(TokenKind.Comma);
                    }
                }

                if (_current == TokenKind.EndOfFile)
                {
                    break;
                }

                if (_current == TokenKind.Comma)
                {
                    Advance();
                    break;
                }

                if (last == null || (last.Category != SyntaxCategory.Value))
                {
                    TakeColon();
                }

                AdvanceNewLine();

                if (_current == TokenKind.Indentation && _current.Span.Length > indentLevel)
                {
                    children.Add(ParseNode());
                }
                else
                {
                    parsingChildNodes = false;
                }
            } while (parsingChildNodes);

            if (children.Count == 1)
            {
                return new ElementSingleNode(CreateSpan(start), start.Value, children[0]);
            }

            return new ElementNode(CreateSpan(start), start.Value, children.ToArray());
        }

        private NodeWithValue ParseAttribute(int indentLevel)
        {
            Token start = Take(TokenKind.Dot);
            Token attr = Take(TokenKind.Identifier);
            
            if (_current == TokenKind.Colon || _current == TokenKind.NewLine)
            {
                TakeColon();
                AdvanceNewLine();

                List<SyntaxNode> children = new List<SyntaxNode>();
                bool parsingChildNodes = true;

                do
                {
                    if (_current == TokenKind.Indentation && _current.Span.Length > indentLevel)
                    {
                        children.Add(ParseNode());
                    }
                    else
                    {
                        parsingChildNodes = false;
                    }
                } while (parsingChildNodes);

                if (children.Count == 1)
                {
                    return new AttributeSingleNode(CreateSpan(start), attr.Value, children[0]);
                }

                return new AttributeNode(CreateSpan(start), attr.Value, children.ToArray());
            }
            else
            {
                SyntaxNode child = ParseNode(indentLevel);
                return new AttributeSingleNode(CreateSpan(start, attr), attr.Value, child);
            }
        }

        private FunctionCallNode ParseFunctionCall()
        {
            Token start = Take(TokenKind.DollarSign);
            Token id = Take(TokenKind.Identifier);

            return new FunctionCallNode(CreateSpan(start, id), id.Value);
        }

        private CallbackNode ParseCallback()
        {
            Token start = Take(TokenKind.Ampersand);
            Token id = Take(TokenKind.Identifier);

            return new CallbackNode(CreateSpan(start, id), id.Value);
        }

        private ConstantNode ParseConstant()
        {
            ConstantKind kind;
            switch (_current.Kind)
            {
                case TokenKind.BooleanLiteral:
                    kind = ConstantKind.Boolean;
                    break;

                case TokenKind.StringLiteral:
                    kind = ConstantKind.String;
                    break;

                case TokenKind.IntegerLiteral:
                    kind = ConstantKind.Integer;
                    break;

                case TokenKind.FloatLiteral:
                    kind = ConstantKind.Float;
                    break;

                case TokenKind.Identifier when _current.Value == "null":
                    kind = ConstantKind.Null;
                    break;

                default:
                    throw UnexpectedToken("Constant");
            }

            var token = Take();
            var node = new ConstantNode(token.Span, token.Value, kind);
            return node;
        }
        #endregion

        #region Utility
        private SourceSpan CreateSpan(Token start) => CreateSpan(start.Span.Start, _current.Span.End);
        private SourceSpan CreateSpan(SyntaxNode start) => CreateSpan(start.Span.Start, _current.Span.End);
        private SourceSpan CreateSpan(SourceLocation start) => CreateSpan(start, _current.Span.End);
        private SourceSpan CreateSpan(Token start, Token end) => CreateSpan(start.Span.Start, end.Span.End);
        private SourceSpan CreateSpan(SourceLocation start, SourceLocation end)
        {
            return new SourceSpan(start, end);
        }
        #endregion

        #region Error Handling
        private void AddError(Severity severity, string message, SourceSpan? span = null)
        {
            _errorSink.AddError(message, _sourceCode, severity, span ?? CreateSpan(_current));
        }

        private SyntaxException SyntaxError(Severity severity, string message, SourceSpan? span = null)
        {
            _error = true;
            AddError(severity, message, span);
            return new SyntaxException(message);
        }

        private SyntaxException UnexpectedToken(TokenKind expected) => UnexpectedToken(expected.ToString());
        private SyntaxException UnexpectedToken(TokenKind[] expecteds)
        {
            var sb = new StringBuilder();
            foreach (var e in expecteds)
            {
                if (sb.Length > 0) sb.Append(",");
                sb.Append(e.ToString());
            }
            return UnexpectedToken(sb.ToString());
        }
        private SyntaxException UnexpectedToken(string expected)
        {
            Advance();
            var value = string.IsNullOrEmpty(_last?.Value) ? _last?.Kind.ToString() : _last?.Value;
            string message = $"Unexpected '{value}'. Expected '{expected}'.";

            return SyntaxError(Severity.Error, message, _last?.Span);
        }
        #endregion
    }
}
