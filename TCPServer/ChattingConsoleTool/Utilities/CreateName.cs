using System;
using System.Collections.Generic;
using System.Xml;

namespace ChattingMultiTool.Utilities
{
    public static class CreateName
    {
        private readonly static object _lock = new object();

        private readonly static List<string> FirstNames = new List<string>();
        private readonly static List<string> FamilyNames = new List<string>();

        private readonly static List<string> _duplicateNames = new List<string>();

        private readonly static Random _random = new Random(Environment.TickCount);

        public static void Init()
        {
            FamilyNames.AddRange(Read("FamilyNames.xml"));
            FirstNames.AddRange(Read("FirstNames.xml"));
        }

        private static List<string> Read(string path)
        {
            var list = new List<string>();
            var reader = new XmlTextReader(path);
            try {
                while (reader.Read()) {
                    switch (reader.NodeType) {
                        case XmlNodeType.Element: {
                                if (string.Equals(reader.Name, "class", StringComparison.Ordinal)) {
                                    var name = reader.GetAttribute("Name");
                                    if (!list.Contains(name)) {
                                        list.Add(name);
                                    }
                                }
                            }
                            break;
                    }
                }
            } catch (Exception e) {
                //logger?.Error($"CreateName - {path} read fail, e:{e.Message}");
            }
            reader.Close();
            return list;
        }

        /// <summary>
        /// 지정 된 성과 이름을 조합 한다. 최대 5회 수행한다.
        /// </summary>
        public static string Create()
        {
            int count = 0;
            while (true) {
                if (count > 5) {
                    return Guid.NewGuid().ToString();
                }
                var familyName = FamilyNames[_random.Next(0, FamilyNames.Count)];
                var firstName = FirstNames[_random.Next(0, FirstNames.Count)];
                var fullName = familyName + firstName;
                if (!_duplicateNames.Contains(fullName)) {
                    _duplicateNames.Add(fullName);
                    return fullName;
                }
                count++;
            }
        }

        /// <summary>
        /// [ThreadSafe] 지정 된 성과 이름을 조합 한다. 최대 5회 수행한다.
        /// </summary>
        public static string CreateTS()
        {
            int count = 0;
            while (true) {
                if (count > 5) {
                    return Guid.NewGuid().ToString();
                }
                lock (_lock) {
                    var familyName = FamilyNames[_random.Next(0, FamilyNames.Count)];
                    var firstName = FirstNames[_random.Next(0, FirstNames.Count)];
                    var fullName = familyName + firstName;
                    if (!_duplicateNames.Contains(fullName)) {
                        _duplicateNames.Add(fullName);
                        return fullName;
                    }
                }
                count++;
            }
        }
    }
}
