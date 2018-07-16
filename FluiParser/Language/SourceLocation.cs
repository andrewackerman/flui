using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language
{
    public struct SourceLocation : IEquatable<SourceLocation>
    {
        private readonly int _column;
        private readonly int _index;
        private readonly int _line;

        public int Column => _column;
        public int Index => _index;
        public int Line => _line;

        public SourceLocation(int index, int line, int column)
        {
            _index = index;
            _line = line;
            _column = column;
        }

        public static bool operator !=(SourceLocation left, SourceLocation right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(SourceLocation left, SourceLocation right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is SourceLocation)
            {
                return Equals((SourceLocation)obj);
            }
            return base.Equals(obj);
        }

        public bool Equals(SourceLocation other)
        {
            return other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return 0xB1679EE ^ Index ^ Line ^ Column;
        }
    }
}
