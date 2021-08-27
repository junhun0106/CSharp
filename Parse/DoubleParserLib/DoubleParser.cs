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
            double result = 0;

            var copy = value.Trim();
            var nagativeIndex = copy.IndexOfAny(nagative);
            if (nagativeIndex != -1) {
                isNegative = true;
            }

            for (int i = 0; i < copy.Length; ++i) {
                var c = copy[i];
                if (((uint)c - '0') <= 9) {
                    result = (result * 10) + (c - '0');
                } else if (c == '+' || c == '-') {
                    continue; // 기호 무시
                } else if (c == '.') {
                    continue; // 소숫점 무시
                } else {
                    return default;
                }
            }


            return isNegative ? -result : result;
        }
    }
}
