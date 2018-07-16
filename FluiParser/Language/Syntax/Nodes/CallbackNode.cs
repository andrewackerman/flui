using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Syntax.Nodes
{
    public sealed class CallbackNode : NodeWithValue
    {
        public override SyntaxKind Kind => SyntaxKind.CallbackNode;
        public override SyntaxCategory Category => SyntaxCategory.Value;

        public CallbackNode(SourceSpan span, string value)
            : base(span, value) { }
    }
}
