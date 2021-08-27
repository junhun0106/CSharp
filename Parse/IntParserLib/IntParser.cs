using System;

namespace IntParserLib
{
    public static class IntParser
    {
        public static int Parse(in ReadOnlySpan<char> span)
        {
            bool fail = false;
            int result = 0;
            for (int i = 0; i < span.Length; ++i) {
                var c = span[i];
                if (c == ' ') continue;
                if (c == '.') break;
                if (c >= '0' && c <= '9') {
                    result = (result * 10) + (c - '0');
                } else {
                    fail = true;
                    break;
                }
            }

            if (fail) {
                return default;
            }

            return result;
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
