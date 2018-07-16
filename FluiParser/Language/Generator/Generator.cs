using FluiParser.Language.Syntax;
using FluiParser.Language.Syntax.Nodes;
using FluiParser.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Generator
{
    public sealed partial class Generator
    {
        private static Lazy<Generator> _inst = new Lazy<Generator>();
        public static Generator Instance => _inst.Value;
        
        private StringBuilder _builder = new StringBuilder();
        private SourceDocument _sourceDoc;
        private void Initialize(SourceDocument sourceDocument)
        {
            _sourceDoc = sourceDocument;
            _builder.Clear();
        }

        public string GenerateViewFile(SourceDocument sourceDocument)
        {
            Initialize(sourceDocument);

            ParseSymbolsForView();

            return _builder.ToString();
        }

        public string GenerateViewModelFile(SourceDocument sourceDocument)
        {
            Initialize(sourceDocument);

            ParseSymbolsForViewModel();

            return _builder.ToString();
        }
    }
}
