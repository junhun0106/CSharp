using System;

namespace BoolParseLib
{
    public static class BoolParser
    {
        private static bool IsTrueStringIgnoreCase(ReadOnlySpan<char> value) => value.Length == 4
                                                                             && (value[0] == 't' || value[0] == 'T')
                                                                             && (value[1] == 'r' || value[1] == 'R')
                                                                             && (value[2] == 'u' || value[2] == 'U')
                                                                             && (value[3] == 'e' || value[3] == 'E');

        internal static bool IsFalseStringIgnoreCase(ReadOnlySpan<char> value) => value.Length == 5
                                                                               && (value[0] == 'f' || value[0] == 'F')
                                                                               && (value[1] == 'a' || value[1] == 'A')
                                                                               && (value[2] == 'l' || value[2] == 'L')
                                                                               && (value[3] == 's' || value[3] == 'S')
                                                                               && (value[4] == 'e' || value[4] == 'E');

        public static bool Parse(ReadOnlySpan<char> s)
        {
            // https://source.dot.net/#System.Private.CoreLib/Boolean.cs,198ff42f14d8c64b
            if (IsTrueStringIgnoreCase(s)) {
                return true;
            }

            if (IsFalseStringIgnoreCase(s)) {
                return false;
            }

            const char nullChar = (char)0x0000;

            // trim null and whitespace

            int start = 0;
            while (start < s.Length) {
                if (!char.IsWhiteSpace(s[start]) && s[start] != nullChar) {
                    break;
                }
                start++;
            }

            int end = s.Length - 1;
            while (end >= start) {
                if (!char.IsWhiteSpace(s[end]) && s[end] != nullChar) {
                    break;
                }
                end--;
            }

            if (IsTrueStringIgnoreCase(s)) { 
                return true;
            }

            return false;
        }
    }
}
