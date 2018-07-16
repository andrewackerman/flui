using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Syntax.Nodes
{
    public sealed class ElementNode : NodeWithValue
    {
        [Newtonsoft.Json.JsonProperty(Order = 10)]
        public SyntaxNode[] Children { get; }

        public override SyntaxKind Kind => SyntaxKind.ElementNode;
        public override SyntaxCategory Category => SyntaxCategory.Element;

        public ElementNode(SourceSpan span, string value, SyntaxNode[] children)
            : base(span, value)
        {
            Children = children;
        }
    }
}
