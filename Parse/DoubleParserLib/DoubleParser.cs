using IntParserLib;
using System;
using System.Numerics;

namespace DoubleParserLib
{
    public static class DoubleParser
    {
        private readonly static char[] negativeSep = new char[] { '-' };

        private readonly static char[] exponentSep = new char[] { 'e', 'E' };

        public static double Parse(ReadOnlySpan<char> value)
        {
            // AllowLeadingWhite - 선행 공백 문자 trim
            // AllowTrailingWhite - 후행 공백 문자 trim
            // AllowLeadingSign - 선행 부호
            // AllowDecimalPoint - 소숫점, ex) '.'
            // https://docs.microsoft.com/ko-kr/dotnet/api/system.globalization.numberformatinfo.numberdecimalseparator?view=net-5.0#System_Globalization_NumberFormatInfo_NumberDecimalSeparator
            // AllowExponent - 지수 표시
            // AllowThousands - 1000 단위 구분 기호와 같은 그룹 구분 기호, ex) ','
            if (value.IsEmpty) {
                return 0;
            }

            bool isNegative = false;
            ulong result = 0;

            var copy = value.Trim(); // AllowLeadingWhite | AllowTrailingWhite

            int emponent = 0;
            {
                var emponentIndex = copy.IndexOfAny(exponentSep);
                if (emponentIndex != -1) {
                    var emponentSlice = copy.Slice(emponentIndex + 1);
                    emponent = IntParser.Parse(in emponentSlice);
                    copy = copy.Slice(0, emponentIndex);
                }
            }

            var nagativeIndex = copy.IndexOfAny(negativeSep); // AllowLeadingSign
            if (nagativeIndex != -1) {
                isNegative = true;
            }

            // https://source.dot.net/#System.Private.CoreLib/Number.Parsing.cs,ba88db163a95b8e9
            if (emponent > 308) {
                // DoubleMaxExponent = 309
                if (isNegative) {
                    return double.NegativeInfinity;
                } else {
                    return double.PositiveInfinity;
                }
            } else if (emponent < -323) {
                // DoubleMinExponent = -324
                return isNegative ? -0d : 0d;
            }

            bool @decimal = false;
            int decimalPoint = 0;
            int totalDigit = 0;
            for (int i = 0; i < copy.Length; ++i) {
                if (@decimal) {
                    decimalPoint++;
                }

                var c = copy[i];
                if (((uint)c - '0') <= 9) {
                    result = (result * 10) + (ulong)(c - '0');
                    totalDigit++;
                } else if (c == '+' || c == '-') {
                    continue; // 기호 무시
                } else if (c == '.') {
                    // AllowDecimalPoint
                    @decimal = true;
                    continue; // 소숫점 무시
                } else if (c == ',') {
                    // AllowThousands
                    continue; // 그룹 기호 무시
                } else {
                    return default;
                }
            }

            var doubleResult = (double)result;
            if (emponent > 0 || decimalPoint > 0) {
                doubleResult *= Math.Pow(10, emponent - decimalPoint);
            }

            return isNegative ? -doubleResult : doubleResult;
        }
    }
}
