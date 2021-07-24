using System.Collections;
using System.Collections.Generic;

namespace ChattingMultiTool.Utilities
{
    public class TSList<T> : ICollection<T>
    {
        private readonly List<T> _list;

        public int Count => _list.Count;

        public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

        public TSList(int capacity)
        {
            _list = new List<T>(capacity);
        }

        public void Add(T item)
        {
            lock (_list) {
                _list.Add(item);
            }
        }

        public bool Remove(T item)
        {
            lock (_list) {
                return _list.Remove(item);
            }
        }

        public bool Contains(T item)
        {
            lock (_list) {
                return _list.Contains(item);
            }
        }

        public void Clear()
        {
            lock (_list) {
                _list.Clear();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_list) {
                _list.CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}
