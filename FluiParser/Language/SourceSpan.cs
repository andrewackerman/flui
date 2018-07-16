using System;
using System.Collections.Generic;
using System.Text;

namespace FluiParser.Language
{
    public struct SourceSpan
    {
        private readonly SourceLocation _start;
        private readonly SourceLocation _end;

        public SourceLocation Start => _start;
        public SourceLocation End => _end;
        public int Length => _end.Index - _start.Index;

        public SourceSpan(SourceLocation start, SourceLocation end)
        {
            _start = start;
            _end = end;
        }

        public static bool operator !=(SourceSpan left, SourceSpan right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(SourceSpan left, SourceSpan right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is SourceSpan)
            {
                return Equals((SourceSpan)obj);
            }
            return base.Equals(obj);
        }

        public bool Equals(SourceSpan other)
        {
            return GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            return 0x509CE ^ Start.GetHashCode() ^ End.GetHashCode();
        }

        public override string ToString()
        {
            return $"{_start.Line} {_start.Column} {Length}";
        }
    }
}
