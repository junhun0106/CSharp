using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary1
{
    public static class StringParser
    {
        private static readonly char[] separators = { ',', ';' };

        public static int ToInt(this string value)
        {
            return int.Parse(value);
        }

        public static int ToIntOrDefault(this string value, int @default = 0)
        {
            if (int.TryParse(value, out int v)) {
                return v;
            }
            return @default;
        }

        public static float ToFloat(this string value)
        {
            return float.Parse(value);
        }

        public static bool ToBool(this string value)
        {
            return bool.Parse(value);
        }

        public static bool ToBoolOrDefault(this string value)
        {
            if (bool.TryParse(value, out var v)) {
                return v;
            }
            return false;
        }

        public static bool[] ToBoolArraySafe(this string value)
        {
            if (string.IsNullOrEmpty(value)) {
                return Array.Empty<bool>();
            }

            var vArr = value.ToStringArraySafe();

            if (vArr.Length == 0) {
                return Array.Empty<bool>();
            }

            var result = new bool[vArr.Length];
            for (int i = 0; i < vArr.Length; i++) {
                result[i] = vArr[i].ToTryBool(defaultValue: false);
            }
            return result;
        }

        /// <summary> true or false 무조건 반환 </summary>
        public static bool ToTryBool(this string value, bool defaultValue = false)
        {
            if (bool.TryParse(value, out bool result)) {
                return result;
            } else {
                return defaultValue;
            }
        }

        public static int ToColorRGB(this string property)
        {
            if (string.IsNullOrEmpty(property))
                throw new ArgumentException("'property' was null or empty");

            var tokens = property.Split(separators);
            if (tokens.Length < 3)
                throw new FormatException("element count was not enough.");

            byte r = Convert.ToByte(tokens[0]);
            byte g = Convert.ToByte(tokens[1]);
            byte b = Convert.ToByte(tokens[2]);
            return ((r & 0xff) << 16) + ((g & 0xff) << 8) + (b & 0xff);
        }

        /// <summary>
        /// d.hh:mm:ss
        /// https://docs.microsoft.com/ko-kr/dotnet/api/system.timespan.parse?view=netframework-4.8 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(this string str)
        {
            return TimeSpan.Parse(str);
        }

        /// <summary>
        /// yy/MM/dd hh:mm:ss
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str)
        {
            return DateTime.Parse(str);
        }


        #region 복수 항목
        /// <summary> 특정한 룰을 따라 스트링 어레이로 전환합니다. 예외를 던지지 않습니다. </summary>
        /// <returns>
        ///     프로퍼티가 중괄호로 싸여져 있고 콤마로 분리된 스트링 리스트일 경우 갯수 만큼 어레이를 반환합니다. 
        ///     아닌 경우 그냥 하나만 반환합니다. 
        ///     빈 프로퍼티인 경우 길이가 0인 어레이를 반환합니다.
        /// </returns>
        public static string[] ToStringArraySafe(this string property, params char[] s)
        {
            if (string.IsNullOrEmpty(property))
                return Array.Empty<string>();

            if (s == null || s.Length == 0) {
                s = separators;
            }

            int start = 0;
            start = property.IndexOf('{');
            //괄호가 없는경우
            if (start == -1) {
                return property
                       .Split(s)
                       .Select(item => item.Trim())
                       .Where(trimStr => string.IsNullOrEmpty(trimStr) == false)
                       .ToArray();
            }
            int end = property.LastIndexOf('}');
            if (end == -1)
                return new[] { property };

            return
                property
                // 중괄호 사이의 스트링만 추출 : "{...}" => "..."
                .Substring(start + 1, end - start - 1).Trim()
                // 분할 "..." => "." "." "."
                .Split(s).Select(item => item.Trim())
                .Where(trimStr => string.IsNullOrEmpty(trimStr) == false)
                .ToArray();
        }

        /// <summary> 특정한 룰을 따라 스트링 어레이로 전환합니다. 예외를 던지지 않습니다. </summary>
        /// <returns>
        ///     프로퍼티가 중괄호로 싸여져 있고 콤마로 분리된 스트링 리스트일 경우 갯수 만큼 어레이를 반환합니다. 
        ///     아닌 경우 그냥 하나만 반환합니다. 
        ///     빈 프로퍼티인 경우 길이가 0인 어레이를 반환합니다.
        /// </returns>
        public static string[] ToStringArray(this string property)
        {
            if (string.IsNullOrEmpty(property))
                throw new ArgumentException("property was null or empty");

            //  괄호 하나를 받음
            int start = property.IndexOf('{');
            if (start == -1)
                throw new FormatException("{ expected in StringList");
            int end = property.LastIndexOf('}');
            if (end == -1)
                throw new FormatException("} expected in StringList");
            return
                property
                // 중괄호 사이의 스트링만 추출 : "{...}" => "..."
                .Substring(start + 1, end - start - 1).Trim()
                // 분할 "..." => "." "." "."
                .Split(separators).Select(item => item.Trim())
                .Where(trimStr => string.IsNullOrEmpty(trimStr) == false)
                .ToArray();
        }

        [Obsolete]
        public static KeyValuePair<string, string>[] GetStringKeyValuePairArray(this string property)
        {
            if (string.IsNullOrEmpty(property)) {
                throw new ArgumentException("property was null or empty");
            }

            //  괄호 하나를 받음
            int start = property.IndexOf('{');
            if (start == -1)
                throw new FormatException("{ expected in StringList");
            int end = property.LastIndexOf('}');
            if (end == -1)
                throw new FormatException("} expected in StringList");
            var aa = property
                // 중괄호 사이의 스트링만 추출 : "{...}" => "..."
                .Substring(start + 1, end - start - 1).Trim()
                // 분할 "..." => "." "." "."
                .Split(separators).Select(item => item.Trim())
                .Where(trimStr => string.IsNullOrEmpty(trimStr) == false)
                .ToArray();

            return Array.Empty<KeyValuePair<string, string>>();
        }


        public static KeyValuePair<bool, string>[][] ToBoolStringArray(this string property)
        {
            if (string.IsNullOrEmpty(property))
                throw new ArgumentException("property was null or empty");

            //  괄호 하나를 받음
            int start = property.IndexOf('{');
            if (start == -1)
                throw new FormatException("{ expected in Vec2List");
            int end = property.LastIndexOf('}');
            if (end == -1)
                throw new FormatException("} expected in Vec2List");

            var result = new List<KeyValuePair<bool, string>[]>();
            string substring = property.Substring(start + 1, end - start - 1);
            substring = substring.Trim();
            int i = 0;
            while (i < substring.Length) {
                int start2 = substring.IndexOf('(', i);
                if (start2 == -1)
                    throw new FormatException("( expected in Vec2List");
                int end2 = substring.IndexOf(')', i);
                if (end2 == 0)
                    throw new FormatException(") expected in Vec2List");
                i = end2 + 1;

                string separsators2 = substring.Substring(start2 + 1, end2 - start2 - 1);
                string[] elements = separsators2.Split(separators);
                if (elements.Length > 0) {
                    var items = new List<KeyValuePair<bool, string>>(elements.Length);
                    foreach (var element in elements) {
                        var strs = element.Split(':');
                        if (strs.Length != 2)
                            continue;
                        items.Add(new KeyValuePair<bool, string>(bool.Parse(strs[0]), strs[1]));
                    }

                    if (items.Count > 0)
                        result.Add(items.ToArray());
                }
            }

            return result.ToArray();
        }

        /// <summary> int 배열을 반환 합니다. property 가 비어(string empty or null) 있다면, 빈 int 배열을 반환 합니다. 예외를 던지지 않습니다. </summary>
        public static int[] ToIntArrayOrDefault(this string property)
        {
            if (string.IsNullOrEmpty(property)) {
                return Array.Empty<int>();
            }

            List<int> result = new List<int>();

            //  괄호 하나를 받음
            int start = property.IndexOf('{');
            if (start == -1) {
                return Array.Empty<int>();
            }

            int end = property.LastIndexOf('}');
            if (end == -1) {
                return Array.Empty<int>();
            }

            string substring = property.Substring(start + 1, end - start - 1);

            string[] elements = substring.Split(separators);
            foreach (var elem in elements) {
                var trimElem = elem.Trim();
                if (string.IsNullOrEmpty(trimElem))
                    continue;

                int i_val;
                if (int.TryParse(trimElem, out i_val)) {
                    result.Add(i_val);
                } else {
                    return Array.Empty<int>();
                }
            }
            return result.ToArray();
        }

        public static int[] ToIntArray(this string property)
        {
            if (string.IsNullOrEmpty(property))
                throw new ArgumentException("property was null or empty");

            List<int> result = new List<int>();

            //  괄호 하나를 받음
            int start = property.IndexOf('{');
            if (start == -1)
                throw new FormatException("{ expected in IntList");
            int end = property.LastIndexOf('}');
            if (end == -1)
                throw new FormatException("} expected in IntList");
            string substring = property.Substring(start + 1, end - start - 1);

            string[] elements = substring.Split(separators);
            foreach (var elem in elements) {
                var trimElem = elem.Trim();
                if (string.IsNullOrEmpty(trimElem))
                    continue;

                int i_val;
                if (int.TryParse(trimElem, out i_val))
                    result.Add(i_val);
                else
                    throw new FormatException("element was not int type. " + property);
            }
            return result.ToArray();
        }

        /// <summary> Float 배열을 반환 합니다. property 가 비어(string empty or null) 있다면, 빈 float 배열을 반환 합니다. 예외를 던지지 않습니다. </summary>
        [Obsolete]
        public static float[] ToFloatArrayOrDefault(this string property)
        {
            if (string.IsNullOrEmpty(property)) {
                return Array.Empty<float>();
            }

            List<float> result = new List<float>();

            //  괄호 하나를 받음
            int start = property.IndexOf('{');
            if (start == -1) {
                return Array.Empty<float>();
            }

            int end = property.LastIndexOf('}');
            if (end == -1) {
                return Array.Empty<float>();
            }

            string substring = property.Substring(start + 1, end - start - 1);

            string[] elements = substring.Split(separators);
            float f_val;
            foreach (var elem in elements) {
                var trimElem = elem.Trim();
                if (string.IsNullOrEmpty(trimElem))
                    continue;
                if (float.TryParse(trimElem, out f_val)) {
                    result.Add(f_val);
                } else {
                    return Array.Empty<float>();
                }
            }
            return result.ToArray();
        }

        [Obsolete]
        public static float[] ToFloatArray(this string property)
        {
            if (string.IsNullOrEmpty(property))
                throw new ArgumentException("property was null or empty");

            List<float> result = new List<float>();

            //  괄호 하나를 받음
            int start = property.IndexOf('{');
            if (start == -1)
                throw new FormatException("{ expected in FloatList");
            int end = property.LastIndexOf('}');
            if (end == -1)
                throw new FormatException("} expected in FloatList");
            string substring = property.Substring(start + 1, end - start - 1);

            string[] elements = substring.Split(separators);
            float f_val;
            foreach (var elem in elements) {
                var trimElem = elem.Trim();
                if (string.IsNullOrEmpty(trimElem))
                    continue;
                if (float.TryParse(trimElem, out f_val)) {
                    result.Add(f_val);
                } else throw new FormatException("element was not float type");
            }
            return result.ToArray();
        }

        public static double[] ToDoubleArray(this string property)
        {
            if (string.IsNullOrEmpty(property))
                throw new ArgumentException("property was null or empty");

            List<double> result = new List<double>();

            //  괄호 하나를 받음
            int start = property.IndexOf('{');
            if (start == -1)
                throw new FormatException("{ expected in DoubleList");
            int end = property.LastIndexOf('}');
            if (end == -1)
                throw new FormatException("} expected in DoubleList");
            string substring = property.Substring(start + 1, end - start - 1);

            string[] elements = substring.Split(separators);
            double d_val;
            foreach (var elem in elements) {
                var trimElem = elem.Trim();
                if (string.IsNullOrEmpty(trimElem))
                    continue;
                if (double.TryParse(trimElem, out d_val)) {
                    result.Add(d_val);
                } else throw new FormatException("element was not double type");
            }
            return result.ToArray();
        }

        public static double[] ToDoubleArrayOrDefault(this string property)
        {
            if (string.IsNullOrEmpty(property)) {
                return Array.Empty<double>();
            }

            List<double> result = new List<double>();

            //  괄호 하나를 받음
            int start = property.IndexOf('{');
            if (start == -1) {
                return Array.Empty<double>();
            }

            int end = property.LastIndexOf('}');
            if (end == -1) {
                return Array.Empty<double>();
            }

            string substring = property.Substring(start + 1, end - start - 1);

            string[] elements = substring.Split(separators);
            double d_val;
            foreach (var elem in elements) {
                var trimElem = elem.Trim();
                if (string.IsNullOrEmpty(trimElem))
                    continue;
                if (double.TryParse(trimElem, out d_val)) {
                    result.Add(d_val);
                } else {
                    return Array.Empty<double>();
                }
            }
            return result.ToArray();
        }
        #endregion

        /// <summary>  콜론으로 분리된 스트링을 keyValue 로 전환합니다. 예외를 던지지 않습니다. </summary>
        public static KeyValuePair<string, int> ToStringInt(this string property, int defaultValue = 1)
        {
            if (string.IsNullOrEmpty(property))
                return new KeyValuePair<string, int>();

            if (!property.Contains(":"))
                return new KeyValuePair<string, int>(property, defaultValue);

            var keyValue = property.Split(':');

            if (keyValue.Length != 2)
                return new KeyValuePair<string, int>(keyValue[0], defaultValue);

            int.TryParse(keyValue[1], out var value);

            return new KeyValuePair<string, int>(keyValue[0], value);
        }

        public static KeyValuePair<int, string> ToIntString(this string property, string defaultValue = "")
        {
            if (string.IsNullOrEmpty(property) || !property.Contains(":"))
                return new KeyValuePair<int, string>();

            var keyValue = property.Split(':');

            int key = -1;
            int.TryParse(keyValue[0], out key);

            return key == -1
                ? new KeyValuePair<int, string>()
                : keyValue.Length != 2
                ? new KeyValuePair<int, string>(key, defaultValue)
                : new KeyValuePair<int, string>(key, keyValue[1]);
        }

        public static KeyValuePair<int, int> ToIntInt(this string property, int defaultValue = 1)
        {
            if (string.IsNullOrEmpty(property))
                return new KeyValuePair<int, int>();

            if (!property.Contains(":"))
                return new KeyValuePair<int, int>(property.ToIntOrDefault(), defaultValue);

            var keyValue = property.Split(':');

            if (keyValue.Length != 2)
                return new KeyValuePair<int, int>(keyValue[0].ToIntOrDefault(), defaultValue);

            int.TryParse(keyValue[1], out var value);

            return new KeyValuePair<int, int>(keyValue[0].ToIntOrDefault(), value);
        }

        /// <summary>  콜론으로 분리된 스트링을 keyValue 로 전환합니다. 예외를 던지지 않습니다. </summary>
        public static KeyValuePair<string, string> ToStringString(this string property)
        {
            if (string.IsNullOrEmpty(property))
                return new KeyValuePair<string, string>();

            if (!property.Contains(":"))
                return new KeyValuePair<string, string>(property, "");

            var keyValue = property.Split(':');

            if (keyValue.Length != 2)
                return new KeyValuePair<string, string>(keyValue[0], "");

            return new KeyValuePair<string, string>(keyValue[0], keyValue[1]);
        }


        /// <summary> 특정한 룰을 따라 keyValue 로 전환합니다. 예외를 던지지 않습니다. </summary>
        /// <returns>
        ///     콜론으로 분리된 스트링을 keyValue 로 반환합니다.
        /// </returns>
        public static KeyValuePair<string, bool> ToStringBool(this string property, bool defValue = true)
        {
            if (string.IsNullOrEmpty(property))
                return new KeyValuePair<string, bool>(string.Empty, defValue);

            if (!property.Contains(":"))
                return new KeyValuePair<string, bool>(property, defValue);

            var keyValue = property.Split(':');

            if (keyValue.Length != 2)
                return new KeyValuePair<string, bool>(keyValue[0], defValue);

            bool.TryParse(keyValue[1], out var value);

            return new KeyValuePair<string, bool>(keyValue[0], value);
        }

        public static KeyValuePair<string, double> ToStringDouble(this string property, double defaultValue = 0.0)
        {
            if (string.IsNullOrEmpty(property))
                return new KeyValuePair<string, double>();

            if (!property.Contains(":"))
                return new KeyValuePair<string, double>(property, defaultValue);

            var keyValue = property.Split(':');

            if (keyValue.Length != 2)
                return new KeyValuePair<string, double>(keyValue[0], defaultValue);

            double.TryParse(keyValue[1], out var value);

            return new KeyValuePair<string, double>(keyValue[0], value);
        }

        public static KeyValuePair<double, int> ToDoubleInt(this string property)
        {
            if (string.IsNullOrEmpty(property))
                return new KeyValuePair<double, int>();

            if (!property.Contains(":"))
                return new KeyValuePair<double, int>();

            var keyValue = property.Split(':');
            if (keyValue.Length != 2)
                return new KeyValuePair<double, int>();

            double.TryParse(keyValue[0].Substring(1, keyValue[1].Length - 1), out var doubleValue);
            int.TryParse(keyValue[1].Substring(0, keyValue[1].Length - 1), out var intValue);

            return new KeyValuePair<double, int>(doubleValue, intValue);
        }

        public static Dictionary<string, KeyValuePair<string, double>> ToStringDoublePairDictionary(this string property, double defaultValue = 0.0)
        {
            var dic = new Dictionary<string, KeyValuePair<string, double>>();
            if (string.IsNullOrEmpty(property))
                return dic;

            var stringArr = property.ToStringArraySafe();
            foreach (var stringData in stringArr) {
                var keyValue = stringData.Split(':');
                if (keyValue.Length != 3)
                    continue;

                if (dic.ContainsKey(keyValue[0]))
                    continue;

                if (double.TryParse(keyValue[2], out var value)) {
                    dic.Add(keyValue[0], new KeyValuePair<string, double>(keyValue[1], value));
                }
            }

            return dic;
        }

        public static Dictionary<string, KeyValuePair<string, bool>> ToStringBoolPairDictionary(this string property, double defaultValue = 0.0)
        {
            var dic = new Dictionary<string, KeyValuePair<string, bool>>();
            if (string.IsNullOrEmpty(property))
                return dic;

            var stringArr = property.ToStringArraySafe();
            foreach (var stringData in stringArr) {
                var keyValue = stringData.Split(':');
                if (keyValue.Length != 3)
                    continue;

                if (dic.ContainsKey(keyValue[0]))
                    continue;

                if (bool.TryParse(keyValue[2], out var value)) {
                    dic.Add(keyValue[0], new KeyValuePair<string, bool>(keyValue[1], value));
                }
            }

            return dic;
        }

        /// <summary>  콜론으로 분리된 스트링들을 Dictionary로 전환합니다. 예외를 던지지 않습니다. </summary>
        public static Dictionary<string, string> ToStringDictionary(this string property)
        {
            var stringArr = property.ToStringArraySafe();
            var stringDic = new Dictionary<string, string>();
            foreach (var stringData in stringArr) {
                var keyVal = stringData.ToStringString();
                stringDic.Add(keyVal.Key, keyVal.Value);
            }
            return stringDic;
        }

        /// <summary> 
        /// 콜론으로 분리된 스트링들을 Dictionary로 전환합니다. 예외를 던지지 않습니다.
        /// 중복된 키를 입력하면 리스트에 추가합니다.
        /// </summary>
        public static Dictionary<string, List<string>> ToStringListDictionary(this string property)
        {
            var stringArr = property.ToStringArraySafe();
            var stringDic = new Dictionary<string, List<string>>();
            foreach (var stringData in stringArr) {
                var keyVal = stringData.ToStringString();
                if (stringDic.ContainsKey(keyVal.Key)) {
                    stringDic[keyVal.Key].Add(keyVal.Value);
                } else {
                    stringDic.Add(keyVal.Key, new List<string>() { keyVal.Value });
                }

            }
            return stringDic;
        }

        /// <summary>
        /// 중복된 키가 존재한다면 int 값을 더해줍니다.
        /// </summary>
        public static Dictionary<string, int> ToStringIntDictionary(this string property, int defValue = 1)
        {
            var stringArr = property.ToStringArraySafe();
            var stringDic = new Dictionary<string, int>();
            foreach (var stringData in stringArr) {
                KeyValuePair<string, int> keyVal = stringData.ToStringInt(defValue);
                if (stringDic.ContainsKey(keyVal.Key)) {
                    stringDic[keyVal.Key] += keyVal.Value;
                } else {
                    stringDic.Add(keyVal.Key, keyVal.Value);
                }
            }
            return stringDic;
        }

        /// <summary>
        /// 중복된 키가 존재한다면 덮어 마지막 값으로 덮어 씌워줍니다.
        /// </summary>
        public static Dictionary<int, string> ToIntStringDictionary(this string property, string defValue = "")
        {
            var stringArr = property.ToStringArraySafe();
            var stringDic = new Dictionary<int, string>();
            foreach (var stringData in stringArr) {
                KeyValuePair<int, string> keyVal = stringData.ToIntString(defValue);
                if (stringDic.ContainsKey(keyVal.Key)) {
                    stringDic[keyVal.Key] = keyVal.Value;
                } else {
                    stringDic.Add(keyVal.Key, keyVal.Value);
                }
            }
            return stringDic;
        }

        /// <summary> string,List<int> </int> </summary>
        public static Dictionary<string, List<int>> ToStringIntListDictionary(this string property, int defValue = 1)
        {
            var stringArr = property.ToStringArraySafe();
            var stringDic = new Dictionary<string, List<int>>();
            foreach (var stringData in stringArr) {
                KeyValuePair<string, int> keyVal = stringData.ToStringInt(defValue);
                if (stringDic.ContainsKey(keyVal.Key)) {
                    stringDic[keyVal.Key].Add(keyVal.Value);
                } else {
                    stringDic.Add(keyVal.Key, new List<int> { keyVal.Value });
                }
            }
            return stringDic;
        }

        public static Dictionary<int, int> ToIntIntDictionary(this string property, int defValue = 1)
        {
            var stringArr = property.ToStringArraySafe();
            var stringDic = new Dictionary<int, int>();
            foreach (var stringData in stringArr) {
                KeyValuePair<int, int> keyVal = stringData.ToIntInt(defValue);
                if (stringDic.ContainsKey(keyVal.Key)) {
                    stringDic[keyVal.Key] = keyVal.Value;
                } else {
                    stringDic.Add(keyVal.Key, keyVal.Value);
                }
            }
            return stringDic;
        }

        public static Dictionary<string, double> ToStringDoubleDictionary(this string property, double defValue = 0.0)
        {
            var stringArr = property.ToStringArraySafe();
            var stringDic = new Dictionary<string, double>();
            foreach (var stringData in stringArr) {
                KeyValuePair<string, double> keyVal = stringData.ToStringDouble(defValue);
                stringDic.Add(keyVal.Key, keyVal.Value);
            }
            return stringDic;
        }

        /// <summary>  콜론으로 분리된 스트링을 keyValue 로 전환합니다. 예외를 던지지 않습니다. </summary>
        public static Dictionary<string, bool> ToStringBoolDictionary(this string property, bool defValue = true)
        {
            var stringArr = property.ToStringArraySafe();
            var list = new Dictionary<string, bool>();
            foreach (var str in stringArr) {
                var keyVal = str.ToStringBool(defValue);
                list.Add(keyVal.Key, keyVal.Value);
            }
            return list;
        }

        /// <summary>  콜론으로 분리된 스트링들을 Dictionary로 전환합니다. 예외를 던지지 않습니다. </summary>
        public static Dictionary<string, List<KeyValuePair<string, string>>> ToKeyValueListDictionary(this string property)
        {
            var stringArr = property.ToStringArraySafe();
            var stringDic = new Dictionary<string, List<KeyValuePair<string, string>>>();
            foreach (var stringData in stringArr) {
                string[] tuple = stringData.Split(':');
                if (tuple.Length != 3) {
                    continue;
                }
                if (stringDic.ContainsKey(tuple[0]) == false) {
                    stringDic.Add(tuple[0], new List<KeyValuePair<string, string>>());
                }
                stringDic[tuple[0]].Add(new KeyValuePair<string, string>(tuple[1], tuple[2]));
            }
            return stringDic;
        }

        /// <summary>
        /// 현재 정상적으로 작동하지 않음.
        /// TODO: 작동하도록 수정해야 함
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, int>> ToStringStringIntDictionary(this string property)
        {
            // {xx:{:,:,:}},{yy:{:,:,:}},{zz:{:,:,:}}
            var result = new Dictionary<string, Dictionary<string, int>>();
            var stringArrList = property
                                    .Split(',', ';')
                                    .Select(item => item.Trim())
                                    .Where(trimStr => !string.IsNullOrEmpty(trimStr)).ToArray();
            // [0]={xx:{:,:,:}} [1]={yy:{:,:,:}}
            foreach (var stringArr in stringArrList) {
                // = {xx:{:,:,:}}
                int start = stringArr.IndexOf('{');
                int end = stringArr.LastIndexOf('}');
                var convertArr = stringArr
                        .Substring(start + 1, end - start - 1).Trim().ToStringString();

                // [0] = xx
                // [1] = {:,:,:}
                var stringIntDic = convertArr.Value.ToStringIntDictionary();
                result.Add(convertArr.Key, stringIntDic);
            }
            return result;
        }

        public static Tuple<string, string, string> ToStringTuple(this string property)
        {
            if (string.IsNullOrEmpty(property)) {
                throw new Exception("ToStringTuple fail. string value is null or empty");
            }

            var vals = property.Split(':');
            if (vals.Length >= 3) {
                return new Tuple<string, string, string>(vals[0], vals[1], vals[2]);
            } else {
                throw new Exception("ToStringTuple fail - property :" + property);
            }
        }

        public static Tuple<string, string, string> ToStringTupleOrDefault(this string property)
        {
            if (string.IsNullOrEmpty(property)) {
                return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
            }

            var vals = property.Split(':');
            if (vals.Length == 3) {
                return new Tuple<string, string, string>(vals[0], vals[1], vals[2]);
            } else {
                return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
            }
        }

        public static Tuple<string, string, string>[] ToStringTupleArray(this string property, params char[] s)
        {
            if (string.IsNullOrEmpty(property))
                throw new ArgumentException("ToStringTupleArray - property was null or empty");

            if (s == null || s.Length == 0) {
                s = separators;
            }

            //  괄호 하나를 받음
            int start = property.IndexOf('{');
            if (start == -1)
                throw new FormatException("{ expected in StringTupleArray");
            int end = property.LastIndexOf('}');
            if (end == -1)
                throw new FormatException("} expected in StringTupleArray");

            var strArr = property
                    // 중괄호 사이의 스트링만 추출 : "{...}" => "..."
                    .Substring(start + 1, end - start - 1).Trim()
                    // 분할 "..." => "." "." "."
                    .Split(s)
                    .Select(item => item.Trim())
                    .Where(trimStr => string.IsNullOrEmpty(trimStr) == false)
                    .ToArray();


            if (string.IsNullOrEmpty(property)) {
                return Array.Empty<Tuple<string, string, string>>();
            }

            var tuples = new List<Tuple<string, string, string>>(strArr.Length);
            foreach (var str in strArr) {
                tuples.Add(str.ToStringTupleOrDefault());
            }

            return tuples.ToArray();
        }

        public static Tuple<string, string, string>[] ToStringTupleArraySafe(this string property, params char[] s)
        {
            if (string.IsNullOrEmpty(property)) {
                return Array.Empty<Tuple<string, string, string>>();
            }

            if (s == null || s.Length == 0) {
                s = separators;
            }

            int start = 0;
            //  괄호 하나를 받음
            start = property.IndexOf('{');
            // 중괄호 없는 버전
            if (start == -1) {
                start = 0;
            } else {
                //  괄호 하나만큼 시작점 변경
                start += 1;
            }

            int end = property.LastIndexOf('}');
            if (end == -1) {
                end = property.Length - 1;
            } else {
                //  괄호 하나만큼 끝점 변경
                end -= 1;
            }

            var strArr = property
                    // 중괄호 사이의 스트링만 추출 : "{...}" => "..."
                    .Substring(start, end - start + 1).Trim()
                    // 분할 "..." => "." "." "."
                    .Split(s)
                    .Select(item => item.Trim())
                    .Where(trimStr => string.IsNullOrEmpty(trimStr) == false)
                    .ToArray();


            if (string.IsNullOrEmpty(property)) {
                return Array.Empty<Tuple<string, string, string>>();
            }

            var tuples = new List<Tuple<string, string, string>>(strArr.Length);
            foreach (var str in strArr) {
                tuples.Add(str.ToStringTupleOrDefault());
            }

            return tuples.ToArray();
        }
    }
}
