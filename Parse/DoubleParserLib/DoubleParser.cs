using System;

namespace DoubleParserLib
{
    public static class DoubleParser
    {
        private readonly static char[] nagative = new char[] { '-' };

        public static double Parse(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty) {
                return 0;
            }

            bool isNegative = false;
            int result = 0;

            var copy = value.Trim();
            var nagativeIndex = copy.IndexOfAny(nagative);
            if (nagativeIndex != -1) {
                isNegative = true;
            }

            bool dot = false;
            int count = 0;
            for (int i = 0; i < copy.Length; ++i) {
                if (dot) {
                    count++;
                }

                var c = copy[i];
                if (((uint)c - '0') <= 9) {
                    result = (result * 10) + (c - '0');
                } else if (c == '+' || c == '-') {
                    continue; // 기호 무시
                } else if (c == '.') {
                    dot = true;
                    continue; // 소숫점 무시
                } else {
                    return default;
                }
            }

            result = isNegative ? -result : result;

            if (count == 0) {
                return result;
            }

            return result / Math.Pow(10, count);
        }
    }
}
