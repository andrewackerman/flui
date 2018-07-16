using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Utility
{
    public static class ExtensionMethods
    {
        public static char CharAt(this string str, int index)
        {
            if (index >= str.Length || index < 0)
            {
                return '\0';
            }

            return str[index];
        }

        public static string PascalCaseToUnderscore(this string str)
        {
            StringBuilder builder = new StringBuilder();

            char c;
            for (int i = 0; i < str.Length; i++)
            {
                c = str[i];
                if (Char.IsUpper(c) && builder.Length > 0)
                {
                    builder.Append('_');
                }

                builder.Append(Char.ToLower(c));
            }

            return builder.ToString();
        }
    }
}
