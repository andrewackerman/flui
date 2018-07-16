using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Syntax.Nodes
{
    public sealed class AttributeSingleNode : NodeWithValue
    {
        [Newtonsoft.Json.JsonProperty(Order = 10)]
        public SyntaxNode Child { get; }

        public override SyntaxKind Kind => SyntaxKind.AttributeSingleNode;
        public override SyntaxCategory Category => SyntaxCategory.Attribute;

        public AttributeSingleNode(SourceSpan span, string value, SyntaxNode child)
            : base(span, value)
        {
            Child = child;
        }
    }
}
