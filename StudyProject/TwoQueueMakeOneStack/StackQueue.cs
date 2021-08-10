using System.Collections.Generic;

namespace TwoQueueMakeOneStack
{
    public class StackQueue<T>
    {
        private Queue<T> _queue1;
        private Queue<T> _queue2;

        public bool IsEmpty => _queue1.Count == 0;

        public StackQueue(int capacity = 1)
        {
            _queue1 = new Queue<T>(capacity);
            _queue2 = new Queue<T>(capacity);
        }

        public void Push(T item)
        {
            _queue1.Enqueue(item);
        }

        public T Pop()
        {
            if (_queue1.Count > 0) {
                T l = default(T);
                while (_queue1.Count > 0) {
                    l = _queue1.Dequeue();

                    if (_queue1.Count > 0) {
                        _queue2.Enqueue(l);
                    }
                }

                var m = _queue1;
                _queue1 = _queue2;
                _queue2 = m;
                return l;
            }

            return default(T);
        }
    }
}
