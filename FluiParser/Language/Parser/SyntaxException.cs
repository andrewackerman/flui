using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language.Parser
{
    [Serializable]
    public class SyntaxException : Exception
    {
        public SyntaxException() { }
        public SyntaxException(string message) { }
        public SyntaxException(string message, Exception inner) { }

        protected SyntaxException(
                        System.Runtime.Serialization.SerializationInfo info,
                        System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
