using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Syntax.Nodes
{
    public sealed class IdentifierNode : NodeWithValue
    {
        public override SyntaxKind Kind => SyntaxKind.IdentifierNode;
        public override SyntaxCategory Category => SyntaxCategory.Value;

        public IdentifierNode(SourceSpan span, string value)
            : base(span, value) { }
    }
}
