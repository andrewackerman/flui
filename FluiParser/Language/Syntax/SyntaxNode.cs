using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Syntax
{
    public abstract class SyntaxNode
    {
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        [Newtonsoft.Json.JsonProperty(Order = -5)]
        public abstract SyntaxCategory Category { get; }

        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        [Newtonsoft.Json.JsonProperty(Order = -4)]
        public abstract SyntaxKind Kind { get; }
        
        [Newtonsoft.Json.JsonIgnore]
        public SourceSpan Span { get; }

        protected SyntaxNode(SourceSpan span)
        {
            Span = span;
        }
    }
}
