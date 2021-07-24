using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TextPerformance
{
    public class Trie
    {
        private class Node
        {
            public bool Terminal { get; set; }
            public Dictionary<char, Node> Nodes { get; private set; }
            public Node ParentNode { get; private set; }
            public char C { get; private set; }

            /// <summary>
            /// String word represented by this node
            /// </summary>
            public string Word {
                get {
                    var b = new StringBuilder();
                    b.Insert(0, C.ToString(CultureInfo.InvariantCulture));
                    var selectedNode = ParentNode;
                    while (selectedNode != null) {
                        b.Insert(0, selectedNode.C.ToString(CultureInfo.InvariantCulture));
                        selectedNode = selectedNode.ParentNode;
                    }
                    return b.ToString();
                }
            }

            public Node(Node parent, char c)
            {
                C = c;
                ParentNode = parent;
                Terminal = false;
                Nodes = new Dictionary<char, Node>();
            }

            /// <summary>
            /// Return list of terminal nodes under this node
            /// </summary>
            public IEnumerable<Node> TerminalNodes(char? ignoreChar = null)
            {
                var r = new List<Node>();
                if (Terminal) r.Add(this);
                foreach (var node in Nodes.Values) {
                    if (ignoreChar != null && node.C == ignoreChar) continue;
                    r = r.Concat(node.TerminalNodes()).ToList();
                }
                return r;
            }
        }

        private Node TopNode_ { get; set; }
        private Node TopNode {
            get {
                if (TopNode_ == null) TopNode_ = new Node(null, ' ');
                return TopNode_;
            }
        }
        private bool CaseSensitive { get; set; }
        private HashSet<char> m_ignore_set;
        Trie m_white_trie = null;

        /// <summary>
        /// Get list of all words in trie that start with
        /// </summary>
        public HashSet<string> GetAutocompleteSuggestions(string wordStart, int fetchMax = 10)
        {
            if (fetchMax <= 0) throw new Exception("Fetch max must be positive integer.");

            wordStart = NormaliseWord(wordStart);

            var r = new HashSet<string>();

            var selectedNode = TopNode;
            foreach (var c in wordStart) {
                // Nothing starting with this word
                if (!selectedNode.Nodes.ContainsKey(c)) return r;
                selectedNode = selectedNode.Nodes[c];
            }

            // Get terminal nodes for this node
            {
                var terminalNodes = selectedNode.TerminalNodes().Take(fetchMax);
                foreach (var node in terminalNodes) {
                    r.Add(node.Word);
                }
            }

            // Go up a node if not found enough suggestions
            if (r.Count < fetchMax) {
                var parentNode = selectedNode.ParentNode;
                if (parentNode != null) {
                    var remainingToFetch = fetchMax - r.Count;
                    var terminalNodes = parentNode.TerminalNodes(selectedNode.C).Take(remainingToFetch);
                    foreach (var node in terminalNodes) {
                        r.Add(node.Word);
                    }
                }
            }

            return r;
        }

        /// <summary>
        /// Initialise instance of trie with set of words
        /// </summary>
        public Trie(IEnumerable<string> words, HashSet<char> ignore_set, Trie white_trie = null, bool caseSensitive = false)
        {
            m_white_trie = white_trie;
            m_ignore_set = ignore_set;
            if (m_ignore_set == null) throw new NullReferenceException("ignore set is null");
            CaseSensitive = caseSensitive;
            foreach (var word in words) {
                AddWord(word);
            }
        }

        /// <summary>
        /// Add a single word to the trie
        /// </summary>
        public void AddWord(string word)
        {
            word = NormaliseWord(word);
            var selectedNode = TopNode;

            for (var i = 0; i < word.Length; i++) {
                var c = word[i];
                if (!selectedNode.Nodes.ContainsKey(c)) {
                    selectedNode.Nodes.Add(c, new Node(selectedNode, c));
                }
                selectedNode = selectedNode.Nodes[c];
            }
            selectedNode.Terminal = true;
        }

        /// <summary>
        /// Normalise word for trie
        /// </summary>
        private string NormaliseWord(string word)
        {
            if (String.IsNullOrWhiteSpace(word)) word = String.Empty;
            word = word.Trim();
            if (CaseSensitive == false) {
                word = word.ToLower();
            }
            return word;
        }

        /// <summary>
        /// Does this word exist in this trie?
        /// </summary>
        public bool IsWordInTrie(string word)
        {
            word = NormaliseWord(word);
            if (String.IsNullOrWhiteSpace(word)) return false;
            var selectedNode = TopNode;
            foreach (var c in word) {
                if (!selectedNode.Nodes.ContainsKey(c)) return false;
                selectedNode = selectedNode.Nodes[c];
            }
            return selectedNode.Terminal;
        }

        public string ConvertValidString(string word)
        {
            bool[] marked_array = null;
            if (m_white_trie != null) marked_array = m_white_trie.GetMarkedArray(word);

            string original = word;
            char[] original_char = original.ToCharArray();
            word = NormaliseWord(word);
            if (String.IsNullOrWhiteSpace(word)) return original;

            char[] sen = word.ToCharArray();

            for (int i = 0; i < sen.Length - 1; ++i) {
                if (marked_array != null && marked_array[i] == true) continue;

                var selectedNode = TopNode;
                char c = sen[i];
                if (selectedNode.Nodes.ContainsKey(c) == false) {
                    continue;
                } else {
                    selectedNode = selectedNode.Nodes[c];
                    int last_index = -1;
                    for (int j = i + 1; j < sen.Length; ++j) {
                        if (marked_array != null && marked_array[i] == true) continue;
                        if (m_ignore_set.Contains(sen[j])) continue;

                        if (selectedNode.Nodes.ContainsKey(sen[j]) == false) {
                            break;
                        } else {
                            selectedNode = selectedNode.Nodes[sen[j]];
                            if (selectedNode.Terminal) {
                                last_index = j;
                            }
                        }
                    }

                    if (last_index != -1) {
                        int pre = i;
                        i = last_index;
                        for (int k = pre; k <= i; ++k) {
                            original_char[k] = '*';
                        }
                    }
                }
            }

            return new String(original_char);
        }

        /// <summary>
        /// 사전과 매칭된 단어의 index는 true로 표기
        /// </summary>
        /// <param name="word"></param>
        /// <returns>bool[]</returns>
        public bool[] GetMarkedArray(string word)
        {
            bool[] marked_array = new bool[word.ToCharArray().Length];
            //Array.Clear(marked_array, 0, marked_array.Length);

            word = NormaliseWord(word);
            if (String.IsNullOrWhiteSpace(word)) return marked_array;

            char[] sen = word.ToCharArray();

            for (int i = 0; i < sen.Length - 1; ++i) {
                var selectedNode = TopNode;
                char c = sen[i];
                if (selectedNode.Nodes.ContainsKey(c) == false) {
                    continue;
                } else {
                    selectedNode = selectedNode.Nodes[c];
                    int last_index = -1;
                    for (int j = i + 1; j < sen.Length; ++j) {
                        if (m_ignore_set.Contains(sen[j])) continue;

                        if (selectedNode.Nodes.ContainsKey(sen[j]) == false) {
                            break;
                        } else {
                            selectedNode = selectedNode.Nodes[sen[j]];
                            if (selectedNode.Terminal) {
                                last_index = j;
                            }
                        }
                    }

                    if (last_index != -1) {
                        int pre = i;
                        i = last_index;
                        for (int k = pre; k <= i; ++k) {
                            marked_array[k] = true;
                        }
                    }
                }
            }

            return marked_array;
        }

    }
}
