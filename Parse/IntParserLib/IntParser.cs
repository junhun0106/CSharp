using System;

namespace IntParserLib
{
    public static class IntParser
    {
        private readonly static char[] nagative = new char[] { '-' };

        /// <summary>
        /// implements NumberStyles.Integer, AllowLeadingWhite, AllowTrailingWhite, AllowLeadingSign
        /// </summary>
        public static int Parse(in ReadOnlySpan<char> span)
        {
            bool isNegative = false;
            int result = 0;

            var copy = span.Trim();
            var nagativeIndex = copy.IndexOfAny(nagative);
            if (nagativeIndex != -1) {
                isNegative = true;
            }

            for (int i = 0; i < copy.Length; ++i) {
                var c = copy[i];
                if (((uint)c - '0') <= 9) {
                    result = (result * 10) + (c - '0');
                }else if(c == '+' || c == '-') {
                    continue;
                } else if (c == '.') {
                    break;
                } else {
                    return default;
                }
            }


            return isNegative ? -result : result;
        }

        public static int Parse(in string value)
        {
            bool fail = false;
            int result = 0;
            for (int i = 0; i < value.Length; ++i) {
                var c = value[i];
                if (c == ' ') continue;
                if (c >= '0' && c <= '9') {
                    result = (result * 10) + (c - '0');
                } else {
                    if (c == '.') {
                        break;
                    }

                    fail = true;
                    break;
                }
            }

            if (fail) {
                return default;
            }

            return result;
        }
    }
}
