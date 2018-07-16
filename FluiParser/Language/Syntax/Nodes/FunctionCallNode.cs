using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Syntax.Nodes
{
    public sealed class FunctionCallNode : NodeWithValue
    {
        public override SyntaxKind Kind => SyntaxKind.FunctionCallNode;
        public override SyntaxCategory Category => SyntaxCategory.Value;

        public FunctionCallNode(SourceSpan span, string value)
            : base(span, value) { }
    }
}
