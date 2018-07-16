using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluiParser.Language.Tokenizer
{
    public sealed class Tokenizer
    {
        private static Lazy<Tokenizer> _inst = new Lazy<Tokenizer>();
        public static Tokenizer Instance => _inst.Value;
        
        private static readonly string[] _widgets = { "AbsorbPointer", "AlertDialog", "Align", "AnimatedBuilder", "AnimatedContainer", "AnimatedCrossFade",
            "AnimatedDefaultTextStyle", "AnimatedListState", "AnimatedModalBarrier", "AnimatedOpacity", "AnimatedPhysicalModel", "AnimatedPositioned",
            "AnimatedSize", "AnimatedWidget", "AnimatedWidgetBaseState", "Appbar", "AspectRatio", "AssetBundle", "BackdropFilter", "Baseline", "BottomNavigationBar",
            "BottomSheet", "ButtonBar", "Card", "Center", "Checkbox", "Chip", "ClipOval", "ClipPath", "ClipRect", "Column", "ConstrainedBox", "Container",
            "CupertinoActivityIndicator", "CupertinoAlertDialog", "CupertinoButton", "CupertinoDialog", "CupertinoDialogAction", "CupertinoFullscreenDialogTransition",
            "CupertinoNavigationBar", "CupertinoPageScaffold", "CupertinoPageTransition", "CupertinoPicker", "CupertinoSlider", "CupertinoSwitch", "CupertinoTabBar",
            "CupertinoTabScaffold", "CupertinoTabView", "CustomMultiChildLayout", "CustomPaint", "CustomScrollView", "CustomSingleChildLayout", "DataTable", "DecoratedBox",
            "DecoratedBoxTransition", "DefaultTextStyle", "Dismissible", "Divider", "DragTarget", "Draggable", "Drawer", "ExcludeSemantics", "ExpansionPanel",
            "FadeTransition", "FittedBox", "FlatButton", "FloatingActionButton", "Flow", "FlutterLogo", "Form", "FormField", "FractionalTranslation", "FractionallySizedBox",
            "FutureBuilder", "GestureDetector", "GridView", "Hero", "Icon", "IconButton", "IgnorePointer", "Image", "IndexedStack", "IntrinsicHeight", "IntrinsicWidth",
            "LayoutBuilder", "LimitedBox", "LinearProgressIndicator", "ListBody", "ListTile", "ListView", "LongPressDraggable", "MediaQuery", "MergeSemantics", "Navigator",
            "NestedScrollView", "NotificationListener", "Offstage", "Opacity", "OverflowBox", "Padding", "Placeholder", "PopupMenuButton", "PositionedTransition", "Radio",
            "RaisedButton", "RawImage", "RawKeyboardListener", "RefreshIndicator", "RichText", "RotatedBox", "RotationTransition", "Row", "Scaffold", "ScaleTransition",
            "ScrollConfiguration", "Scrollable", "Scrollbar", "Semantics", "SimpleDialog", "SingleChildScrollView", "SizeTransition", "SizedBox", "SizedOverflowBox",
            "SlideTransition", "Slider", "SnackBar", "Stack", "Stepper", "StreamBuilder", "Switch", "TabBar", "TabBarView", "Table", "Text", "TextField", "Theme",
            "Tooltip", "Transform", "Wrap" };
        private static readonly string[] _boolLiterals = { "true", "false" };

        private StringBuilder _builder;
        private ErrorSink _errorSink;
        private int _line;
        private int _column;
        private int _index;
        private SourceCode _sourceCode;
        private SourceLocation _tokenStart;
        private char _indentationType;

        public ErrorSink ErrorSink => _errorSink;

        private char _ch => _sourceCode[_index];
        private char _last => Peek(-1);
        private char _next => Peek(1);

        private char Peek(int ahead) => _sourceCode[_index + ahead];

        public Tokenizer() : this(new ErrorSink()) { }
        public Tokenizer(ErrorSink errorSink)
        {
            _errorSink = errorSink;
            _builder = new StringBuilder();
            _sourceCode = null;
        }

        public IEnumerable<Token> TokenizeFile(string sourceCode) => TokenizeFile(new SourceCode(sourceCode));
        public IEnumerable<Token> TokenizeFile(SourceCode sourceCode)
        {
            _sourceCode = sourceCode;
            _builder.Clear();
            _errorSink.Clear();
            _tokenStart = new SourceLocation();
            _line = 1;
            _column = 0;
            _index = 0;
            _indentationType = '\0';
            CreateToken(TokenKind.EndOfFile);

            return TokenizeContents();
        }

        private void Advance()
        {
            _index++;
            _column++;
        }

        private void Consume()
        {
            _builder.Append(_ch);
            Advance();
        }

        private void DoNewLine()
        {
            _line++;
            _column = 0;
        }

        private Token CreateToken(TokenKind kind)
        {
            string contents = _builder.ToString();
            SourceLocation start = _tokenStart;
            SourceLocation end = new SourceLocation(_index, _line, _column);

            _tokenStart = end;
            _builder.Clear();

            return new Token(kind, contents, start, end);
        }

        private bool IsLetter => Char.IsLetter(_ch);
        private bool IsDigit => Char.IsDigit(_ch);
        private bool IsLetterOrDigit => Char.IsLetterOrDigit(_ch);
        private bool IsEOF => _ch == '\0';
        private bool IsIdentifier => IsLetterOrDigit || _ch == '_';
        private bool IsNewLine => _ch == '\r' || _ch == '\n';
        private bool IsWhiteSpace => (Char.IsWhiteSpace(_ch) || IsEOF) && !IsNewLine;
        private bool IsPunctuation => "<>{}()[]!$%^&*+-=/.,?;:|~@#".Contains(_ch);
        private bool IsWidget => _widgets.Contains(_builder.ToString());
        private bool IsBoolLiteral => _boolLiterals.Contains(_builder.ToString());

        private IEnumerable<Token> TokenizeContents()
        {
            while (!IsEOF)
            {
                yield return GetNextToken();
            }

            yield return CreateToken(TokenKind.EndOfFile);
        }

        private Token GetNextToken()
        {
            if (IsEOF)
            {
                return CreateToken(TokenKind.EndOfFile);
            }
            else if (IsNewLine)
            {
                return ScanNewLine();
            }
            else if (IsWhiteSpace)
            {
                return ScanWhiteSpace();
            }
            else if (IsDigit || (_ch == '-' && _next >= '0' && _next <= '9'))
            {
                return ScanInteger();
            }
            else if (_ch == '!')
            {
                return ScanComment();
            }
            else if (IsLetter || _ch == '_')
            {
                return ScanIdentifier();
            }
            else if (_ch == '\'' || _ch == '"')
            {
                return ScanStringLiteral();
            }
            else if (IsPunctuation)
            {
                return ScanPunctuation();
            }
            else
            {
                return ScanWord();
            }
        }

        private Token ScanNewLine()
        {
            while (IsNewLine)
            {
                Consume();
            }

            DoNewLine();

            return CreateToken(TokenKind.NewLine);
        }

        private Token ScanWhiteSpace()
        {
            if (_column == 0 && IsIndentation)
            {
                return ScanIndentation();
            }

            while (IsWhiteSpace)
            {
                Consume();
            }

            return CreateToken(TokenKind.WhiteSpace);
        }

        private bool IsIndentation => _ch == '\t' || _ch == ' ';
        private Token ScanIndentation()
        {
            if (_indentationType == '\0')
            {
                _indentationType = _ch;

            }
            else if (_ch != _indentationType)
            {
                AddError($"Inconsistent indentation character, {(_indentationType == '\t' ? "tab" : "space")} expected.", Severity.Warning);
            }

            while (IsIndentation)
            {
                Consume();
            }

            return CreateToken(TokenKind.Indentation);
        }

        private bool IsHexDigit => IsDigit || (Char.ToUpper(_ch) >= 'A' && Char.ToUpper(_ch) <= 'F');
        private Token ScanInteger()
        {
            if (_ch == '-')
            {
                Consume();
            }

            while (IsDigit)
            {
                Consume();
            }

            if (_ch == '.' || _ch == 'e')
            {
                return ScanFloat();
            }
            else if (_ch == 'x' && _last == '0')
            {
                Consume();
                while (IsHexDigit)
                {
                    Consume();
                }
            }

            if (!IsWhiteSpace && !IsNewLine && !IsPunctuation && !IsEOF)
            {
                return ScanWord();
            }

            return CreateToken(TokenKind.IntegerLiteral);
        }

        private Token ScanFloat()
        {
            int preDotLength = _index - _tokenStart.Index;

            if (_ch == '.')
            {
                Consume();

                while (IsDigit)
                {
                    Consume();
                }

                if (_last == '.')
                {
                    ScanWord(message: "Must contain digits after '.'");
                }
            }
            else if (_ch == 'e')
            {
                Consume();
                if (_ch == '-')
                {
                    Consume();
                }
                while (IsDigit)
                {
                    Consume();
                }

                if (_last == 'e')
                {
                    ScanWord(message: "Must contain digits after 'e'");
                }
                else if (_last == '-')
                {
                    ScanWord(message: "Must contain digits after '-'");
                }
            }

            if (!IsWhiteSpace && !IsNewLine && !IsPunctuation && !IsEOF)
            {
                if (IsLetter)
                {
                    return ScanWord(message: "'{0}' is an invalid float value.");
                }

                return ScanWord();
            }

            return CreateToken(TokenKind.FloatLiteral);
        }

        private Token ScanComment()
        {
            Consume();
            if (_ch == '!')
            {
                return ScanBlockComment();
            }

            Consume();

            while (!IsNewLine && !IsEOF)
            {
                Consume();
            }

            return CreateToken(TokenKind.LineComment);
        }

        private bool IsEndOfBlockComment => _ch == '!' && _next == '!';
        private Token ScanBlockComment()
        {
            while (!IsEndOfBlockComment)
            {
                if (IsEOF)
                {
                    return CreateToken(TokenKind.Error);
                }
                if (IsNewLine)
                {
                    do
                    {
                        Consume();
                    } while (IsNewLine);

                    DoNewLine();
                }

                Consume();
            }

            Consume();
            Consume();

            return CreateToken(TokenKind.BlockComment);
        }

        private Token ScanIdentifier()
        {
            while (IsIdentifier)
            {
                Consume();
            }

            if (!IsWhiteSpace && !IsNewLine && !IsPunctuation && !IsEOF)
            {
                return ScanWord();
            }

            if (IsBoolLiteral)
            {
                return CreateToken(TokenKind.BooleanLiteral);
            }
            else if (IsWidget)
            {
                return CreateToken(TokenKind.Widget);
            }

            return CreateToken(TokenKind.Identifier);
        }

        private Token ScanStringLiteral()
        {
            char quoteType = _ch;

            Advance();

            while (_ch != quoteType)
            {
                if (IsEOF)
                {
                    AddError("Unexpected end of file", Severity.Fatal);
                    return CreateToken(TokenKind.Error);
                }
                if (IsNewLine)
                {
                    AddError("Unexpected line break in string literal", Severity.Error);
                    return CreateToken(TokenKind.Error);
                }

                Consume();
            }

            Advance();

            return CreateToken(TokenKind.StringLiteral);
        }

        private Token ScanPunctuation()
        {
            switch (_ch)
            {
                case ':':
                    Consume();
                    return CreateToken(TokenKind.Colon);
                case '.':
                    Consume();
                    return CreateToken(TokenKind.Dot);
                case ',':
                    Consume();
                    return CreateToken(TokenKind.Comma);
                case '$':
                    Consume();
                    return CreateToken(TokenKind.DollarSign);
                case '@':
                    Consume();
                    return CreateToken(TokenKind.Ampersand);
                default:
                    return ScanWord();
            }
        }

        private Token ScanWord(Severity severity = Severity.Error, string message = "Unexpected token '{0}'")
        {
            while (!IsWhiteSpace && !IsNewLine && !IsEOF && !IsPunctuation)
            {
                Consume();
            }

            AddError(string.Format(message, _builder.ToString()), severity);
            return CreateToken(TokenKind.Error);
        }

        private void AddError(string message, Severity severity)
        {
            var span = new SourceSpan(_tokenStart, new SourceLocation(_index, _line, _column));
            _errorSink.AddError(message, _sourceCode, severity, span);
        }
    }
}
