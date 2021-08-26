using System;
using System.Collections.Generic;

namespace ClassLibrary1
{
    public static class StringParserWithCustom
    {
        private static readonly char[] separators = { ',', ';' };
        private static readonly char[] trimChars = { '{', '}', ' ' };

        public static string[] ToStringArraySafe_Custom(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                return Array.Empty<string>();
            }

            var result = new List<string>();

            property = property.Trim(trimChars.AsSpan());

            while (!property.IsEmpty) {
                var index = property.IndexOfAny(separators);
                ReadOnlySpan<char> slice;
                if (index == -1) {
                    slice = property;
                } else {
                    slice = property.Slice(0, index);
                }

                result.Add(slice.Trim().ToString());

                if (index == -1) {
                    break;
                }

                property = property.Slice(index + 1);
            }

            return result.ToArray();
        }
    }
}
