using System;
using System.Collections.Generic;

namespace ClassLibrary1
{
    public static class StringParserWithSpan
    {
        private static readonly char[] separators = {',', ';'};
        private static readonly char[] trimChars = {'{', '}', ' '};
 
        public static KeyValuePair<string, string> ToStringString(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                return new KeyValuePair<string, string>();
            }

            int idx = 0;
            string elem0 = null, elem1 = null;
            foreach (var entry in property.Split(':')) {
                if (idx == 0) {
                    elem0 = entry.Trim().ToString();
                } else if (idx == 1) {
                    elem1 = entry.Trim().ToString();
                } else {
                    break;
                }
                ++idx;
            }

            return new KeyValuePair<string, string>(elem0 ?? string.Empty, elem1 ?? string.Empty);
        }

        public static Dictionary<string, List<string>> ToStringListDictionary(this ReadOnlySpan<char> property)
        {
            var stringDic = new Dictionary<string, List<string>>();
            
            if (property.IsEmpty == false) {
                foreach (var entry in property.Trim(trimChars.AsSpan()).Split(separators.AsSpan(), StringSplitOptions.RemoveEmptyEntries)) {
                    var stringData = entry.Trim();
                    var keyVal = stringData.ToStringString();
                    if (stringDic.TryGetValue(keyVal.Key, out var list)) {
                        list.Add(keyVal.Value);
                    } else {
                        stringDic.Add(keyVal.Key, new List<string>() {keyVal.Value});
                    }
                }
            }
            
            return stringDic;
        }

        public static Dictionary<string, int> ToStringIntDictionary(this ReadOnlySpan<char> self, int def = 1)
        {
            var result = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var entry in self.Trim(trimChars).Split(separators, StringSplitOptions.RemoveEmptyEntries)) {
                string key = null;
                int val = def;
                int idx = 0;
                foreach (var pair in entry.Trim().Split(':')) {
                    if (idx == 0) {
                        key = pair.ToString();
                    } else if (idx == 1) {
                        int.TryParse(pair.ToString(), out val);
                    } else {
                        break;
                    }
                    ++idx;
                }

                if (string.IsNullOrEmpty(key)) {
                    continue;
                }
                result.Add(key, val);
            }

            return result;
        }

        public static string[] ToStringArraySafe(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                return Array.Empty<string>();
            }

            var result = new List<string>();
            foreach (var entry in property.Trim(trimChars.AsSpan()).Split(separators.AsSpan(), StringSplitOptions.RemoveEmptyEntries)) {
                result.Add(entry.Trim().ToString());
            }

            return result.ToArray();
        }

        public static string[] ToStringArraySafeNotResize(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                return Array.Empty<string>();
            }

            int capacity = 0;
            for (int i = 0; i < property.Length; ++i) {
                var prop = property[i];
                if (prop == ',' || prop == ';') {
                    capacity++;
                }
            }

            var result = new List<string>(capacity);
            foreach (var entry in property.Trim(trimChars).Split(separators, StringSplitOptions.RemoveEmptyEntries)) {
                result.Add(entry.Trim().ToString());
            }

            return result.ToArray();
        }

        public static string[] ToStringArraySafe_2(this ReadOnlySpan<char> property)
        {
            if (property.IsEmpty) {
                return Array.Empty<string>();
            }

            var result = new List<string>();
            foreach (var entry in property.Trim(trimChars).Split(separators)) {
                result.Add(entry.Trim().ToString());
            }

            return result.ToArray();
        }

        public static Dictionary<string, KeyValuePair<string, double>> ToStringDoublePairDictionary(this ReadOnlySpan<char> property)
        {
            var dic = new Dictionary<string, KeyValuePair<string, double>>(StringComparer.Ordinal);
            foreach (var entry in property.Trim(trimChars).Split(separators)) {
                int index = 0;
                string key = null;
                string kvKey = null;
                double kvValue = 0d;
                foreach (var entry2 in entry.Split(':')) {
                    if (index == 0) {
                        key = entry2.ToString();
                    } else if (index == 1) {
                        kvKey = entry2.ToString();
                    } else {
                        kvValue = double.Parse(entry2.ToString());
                    }
                }

                if (!dic.ContainsKey(key)) {
                    dic.Add(key, new KeyValuePair<string, double>(kvKey, kvValue));
                }
            }

            return dic;
        }
    }
}
