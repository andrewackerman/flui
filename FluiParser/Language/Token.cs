using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language
{
    public class Token
    {
        private Lazy<TokenCategory> _catagory;

        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public TokenCategory Catagory => _catagory.Value;
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public TokenKind Kind { get; }
        public SourceSpan Span { get; }
        public string Value { get; }

        public bool IsTrivia => Kind == TokenKind.WhiteSpace || Catagory == TokenCategory.Comment;

        public Token(TokenKind kind, string value, SourceLocation start, SourceLocation end)
        {
            Kind = kind;
            Value = value;
            Span = new SourceSpan(start, end);

            _catagory = new Lazy<TokenCategory>(GetTokenCategory);
        }

        public static bool operator !=(Token left, string right) => left?.Value != right;
        public static bool operator !=(string left, Token right) => left != right?.Value;
        public static bool operator !=(Token left, TokenKind right) => left?.Kind != right;
        public static bool operator !=(TokenKind left, Token right) => left != right?.Kind;
        public static bool operator !=(Token left, TokenCategory right) => left?.Catagory != right;
        public static bool operator !=(TokenCategory left, Token right) => left != right?.Catagory;

        public static bool operator ==(Token left, string right) => left?.Value == right;
        public static bool operator ==(string left, Token right) => left == right?.Value;
        public static bool operator ==(Token left, TokenKind right) => left?.Kind == right;
        public static bool operator ==(TokenKind left, Token right) => left == right?.Kind;
        public static bool operator ==(Token left, TokenCategory right) => left?.Catagory == right;
        public static bool operator ==(TokenCategory left, Token right) => left == right?.Catagory;

        public override bool Equals(object obj)
        {
            if (obj is Token)
            {
                return Equals((Token)obj);
            }
            return base.Equals(obj);
        }

        public bool Equals(Token other)
        {
            if (other == null) return false;
            return other.Value == Value &&
                   other.Span == Span &&
                   other.Kind == Kind;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() ^ Span.GetHashCode() ^ Kind.GetHashCode();
        }

        public override string ToString()
        {
            return $"Kind: {Kind} - Value: {Value}";
        }

        private TokenCategory GetTokenCategory()
        {
            switch (Kind)
            {
                case TokenKind.Colon:
                case TokenKind.Dot:
                case TokenKind.Comma:
                case TokenKind.DollarSign:
                case TokenKind.Ampersand:
                    return TokenCategory.Punctuation;

                case TokenKind.BlockComment:
                case TokenKind.LineComment:
                    return TokenCategory.Comment;

                case TokenKind.NewLine:
                case TokenKind.WhiteSpace:
                case TokenKind.Indentation:
                    return TokenCategory.WhiteSpace;

                case TokenKind.Identifier:
                case TokenKind.Widget:
                    return TokenCategory.Identifier;

                case TokenKind.StringLiteral:
                case TokenKind.IntegerLiteral:
                case TokenKind.FloatLiteral:
                case TokenKind.BooleanLiteral:
                    return TokenCategory.Constant;

                case TokenKind.EndOfFile:
                    return TokenCategory.Metadata;

                case TokenKind.Error:
                    return TokenCategory.Invalid;

                default:
                    return TokenCategory.Unknown;
            }
        }
    }
}
