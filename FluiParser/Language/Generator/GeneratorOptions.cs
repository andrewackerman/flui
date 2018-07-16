using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Generator
{
    public sealed class GeneratorOptions
    {
        public static readonly GeneratorOptions Tabs = new GeneratorOptions() { IndentationCharacter = '\t', IndentationLength = 1 };
        public static readonly GeneratorOptions Spaces = new GeneratorOptions() { IndentationCharacter = ' ', IndentationLength = 2 };
        public static readonly GeneratorOptions Default = Spaces;

        public char IndentationCharacter { get; set; }
        public int IndentationLength { get; set; }

        public GeneratorOptions()
        {
            IndentationCharacter = '\t';
            IndentationLength = 1;
        }
    }
}
