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
        private GeneratorOptions _options;
        private SourceDocument _sourceDoc;

        private void Initialize(SourceDocument sourceDocument, GeneratorOptions options)
        {
            _sourceDoc = sourceDocument;
            _options = options;
            _builder.Clear();
        }

        public string GenerateViewFile(SourceDocument sourceDocument) => GenerateViewFile(sourceDocument, GeneratorOptions.Default);
        public string GenerateViewFile(SourceDocument sourceDocument, GeneratorOptions options)
        {
            Initialize(sourceDocument, options);

            ParseSymbolsForView();

            return _builder.ToString();
        }

    public string GenerateViewModelFile(SourceDocument sourceDocument) => GenerateViewModelFile(sourceDocument, GeneratorOptions.Default);
        public string GenerateViewModelFile(SourceDocument sourceDocument, GeneratorOptions options)
        {
            Initialize(sourceDocument, options);

            ParseSymbolsForViewModel();

            return _builder.ToString();
        }
    }
}
