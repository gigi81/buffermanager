using System;
using System.Collections.Generic;

namespace Grillisoft.BufferManager
{
    internal class BuffersHashSet<T>
    {
        private readonly int _bufferSize;
        private readonly IAllocEvents _events;

        private readonly HashSet<T> _buffers = new HashSet<T>();

        internal BuffersHashSet(int bufferSize, IAllocEvents events)
        {
            _bufferSize = bufferSize;
            _events = events;
        }

        public T Add(T buffer)
        {
            _buffers.Add(buffer);
            _events?.Allocate(_bufferSize);
            return buffer;
        }

        public bool Remove(T buffer)
        {
            if (!_buffers.Contains(buffer))
                return false;

            _buffers.Remove(buffer);
            _events?.Free(_bufferSize);
            return true;
        }

        public void Clear(Action<T> action = null)
        {
            var count = _buffers.Count;

            if (action != null)
                foreach (var buffer in _buffers)
                    action.Invoke(buffer);

            _buffers.Clear();
            _events?.Free(count * _bufferSize);
        }
    }
}
