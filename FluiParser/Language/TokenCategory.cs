using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language
{
    public enum TokenCategory
    {
        Unknown,
        WhiteSpace,
        Comment,

        Constant,
        Identifier,
        Punctuation,

        Metadata,

        Invalid,
    }
}
