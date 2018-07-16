using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Syntax.Nodes
{
    public abstract class NodeWithValue : SyntaxNode
    {
        [Newtonsoft.Json.JsonProperty(Order = -3)]
        public string Value { get; }

        protected NodeWithValue(SourceSpan span, string value)
            : base(span)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"Category: {Category}, Kind: {Kind}, Value: \"{Value}\"";
        }
    }
}
