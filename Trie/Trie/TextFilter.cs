using System;
using System.Collections.Generic;

namespace TextPerformance
{
    public static class TextFilter
    {
        private static Trie trie;

        private static readonly List<string> _textFilters = new List<string> {
            "욕1",
            "욕2",
            "욕3",
            "욕4",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕5",
            "욕999",
        };

        public static int Count => _textFilters.Count;

        static TextFilter()
        {
            trie = new Trie(_textFilters, new HashSet<char>());
        }

        private static bool Length(string text)
        {
            // 0을 받아 줄 의무는 없음
            var min = 1;
            var max = 100;

            int count = 0;
            var charArray = new char[1];
            foreach (var c in text) {
                charArray[0] = c;
                int byteCount = System.Text.Encoding.UTF8.GetByteCount(charArray);
                if (byteCount > 1) {
                    count += 2;
                } else {
                    count += 1;
                }
            }

            return max >= count && min <= count;
        }

        public static bool Filter(string text)
        {
            if (!Length(text)) {
                return false;
            }

            var removeWhiteSpace = text.Replace(" ", "");

            foreach (var filter in _textFilters) {
                if (removeWhiteSpace.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0) {
                    return true;
                }
            }
            return false;
        }

        public static string Filter_Trie(string text)
        {
            text = text.Replace(" ", "");

            return trie.ConvertValidString(text);
        }
    }
}
