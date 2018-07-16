using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Parser
{
    public sealed class ParserOptions
    {
        public static readonly ParserOptions Default = new ParserOptions();
        public static readonly ParserOptions Strict = new ParserOptions() { EnforceStrictIndentation = true, EnforceColons = true };

        public bool EnforceStrictIndentation { get; set; }
        public bool EnforceColons { get; set; }

        public ParserOptions()
        {
            EnforceColons = false;
            EnforceStrictIndentation = false;
        }
    }
}
