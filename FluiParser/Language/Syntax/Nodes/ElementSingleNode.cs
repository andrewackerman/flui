using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Syntax.Nodes
{
    public sealed class ElementSingleNode : NodeWithValue
    {
        [Newtonsoft.Json.JsonProperty(Order = 10)]
        public SyntaxNode Child { get; }

        public override SyntaxKind Kind => SyntaxKind.ElementSingleNode;
        public override SyntaxCategory Category => SyntaxCategory.Element;

        public ElementSingleNode(SourceSpan span, string value, SyntaxNode child)
            : base(span, value)
        {
            Child = child;
        }
    }
}
