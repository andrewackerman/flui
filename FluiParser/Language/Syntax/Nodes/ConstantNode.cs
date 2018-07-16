using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Syntax.Nodes
{
    public sealed class ConstantNode : NodeWithValue
    {
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ConstantKind ConstantKind { get; }

        public override SyntaxKind Kind => SyntaxKind.ConstantNode;
        public override SyntaxCategory Category => SyntaxCategory.Value;

        public ConstantNode(SourceSpan span, string value, ConstantKind kind)
            : base(span, value)
        {
            ConstantKind = kind;
        }
    }

    public enum ConstantKind
    {
        Invalid,
        Null,
        Integer,
        Float,
        String,
        Boolean,
    }
}
