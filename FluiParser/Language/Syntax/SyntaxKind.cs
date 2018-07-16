using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Syntax
{
    public enum SyntaxKind
    {
        Invalid,
        SourceDocument,
        ElementNode,
        AttributeNode,
        IdentifierNode,
        FunctionCallNode,
        ConstantNode,
        AttributeSingleNode,
        ElementSingleNode,
        CallbackNode,
    }
}
