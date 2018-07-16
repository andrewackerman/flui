using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Syntax.Nodes
{
    public sealed class AttributeNode : NodeWithValue
    {
        [Newtonsoft.Json.JsonProperty(Order = 10)]
        public SyntaxNode[] Children { get; }

        public override SyntaxKind Kind => SyntaxKind.AttributeNode;
        public override SyntaxCategory Category => SyntaxCategory.Attribute;

        public AttributeNode(SourceSpan span, string value, SyntaxNode[] children)
            : base(span, value)
        {
            Children = children;
        }
    }
}
