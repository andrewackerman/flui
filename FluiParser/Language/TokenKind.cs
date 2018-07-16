using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language
{
    public enum TokenKind
    {
        EndOfFile,
        Error,

        #region Whitespace

        WhiteSpace,
        NewLine,
        Indentation,

        #endregion

        #region Comments

        LineComment,
        BlockComment,

        #endregion

        #region Constants

        IntegerLiteral,
        StringLiteral,
        FloatLiteral,
        BooleanLiteral,

        #endregion

        #region Identifiers

        Identifier,
        Widget,

        #endregion

        #region Punctuation

        Dot,
        Colon,
        Comma,
        DollarSign,
        Ampersand,

        #endregion
    }
}
