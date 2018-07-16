using FluiParser.Language.Syntax.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Syntax
{
    public sealed class SourceDocument
    {
        public WidgetKind Kind { get; }
        public AttributeSingleNode ViewModelClass { get; }
        public AttributeSingleNode ViewClass { get; }

        [Newtonsoft.Json.JsonIgnore()]
        public String ViewClassName => (ViewClass.Child as NodeWithValue).Value;
        [Newtonsoft.Json.JsonIgnore()]
        public String ViewModelClassName => (ViewModelClass.Child as NodeWithValue).Value;

        [Newtonsoft.Json.JsonIgnore()]
        public SourceCode SourceCode { get; }

        public SourceDocument(SourceCode sourceCode, WidgetKind kind, AttributeSingleNode viewModelClass, AttributeSingleNode viewClass)
        {
            SourceCode = sourceCode;
            Kind = kind;
            ViewModelClass = viewModelClass;
            ViewClass = viewClass;
        }
    }
}
