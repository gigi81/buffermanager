using System;
using System.Collections.Generic;

namespace Grillisoft.BufferManager
{
    internal class BuffersCache<T>
    {
        private readonly int _bufferSize;
        private readonly ICacheEvents _events;
        private readonly int _max;

        /// <summary>
        /// Contains the list of the buffers NOT in use
        /// </summary>
        private readonly Stack<T> _buffers = new Stack<T>();

        internal BuffersCache(int bufferSize, ICacheEvents events, int cacheSize)
        {
            _bufferSize = bufferSize;
            _events = events;
            _max = cacheSize / bufferSize;
        }

        public bool TryPush(T buffer)
        {
            if ((_buffers.Count + 1) > _max)
                return false;

            _buffers.Push(buffer);
            _events?.Cache(_bufferSize);
            return true;
        }

        public bool TryPop(out T buffer)
        {
            if (_buffers.Count <= 0)
            {
                buffer = default(T);
                return false;
            }

            buffer = _buffers.Pop();
            _events?.FreeCache(_bufferSize);
            return true;
        }

        public void Clear(Action<T> action)
        {
            var count = _buffers.Count;

            if (action != null)
                foreach (var buffer in _buffers)
                    action.Invoke(buffer);

            _buffers.Clear();
            _events?.FreeCache(count * _bufferSize);
        }
    }
}