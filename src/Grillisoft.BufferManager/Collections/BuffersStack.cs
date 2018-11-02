using System;
using System.Collections.Generic;

namespace Grillisoft.BufferManager.Collections
{
    internal class BuffersStack<T>
    {
        private readonly int _bufferSize;
        private readonly IAllocEvents _events;
        private readonly int _maxCount;

        /// <summary>
        /// Container for the buffers NOT in use
        /// </summary>
        private readonly Stack<T> _buffers = new Stack<T>();

        internal BuffersStack(int bufferSize, IAllocEvents events, int maxSize)
        {
            _bufferSize = bufferSize;
            _events = events;
            _maxCount = maxSize / bufferSize;
        }

        public bool TryPush(T buffer)
        {
            if (_buffers.Count >= _maxCount)
                return false;

            _buffers.Push(buffer);
            _events?.Allocate(_bufferSize);
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