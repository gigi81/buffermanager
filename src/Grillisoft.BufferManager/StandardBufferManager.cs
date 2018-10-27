using System;
using System.Collections.Generic;

namespace Grillisoft.BufferManager
{
    public class StandardBufferManager<T> : IBufferManager<T> where T : struct, IComparable, IEquatable<T>, IConvertible
    {
        public const int DefaultBufferSize = 4096;

        /// <summary>
        /// Contains the list of the buffers in use
        /// </summary>
        private readonly HashSet<T[]> _buffers = new HashSet<T[]>();

        /// <summary>
        /// Contains the list of the buffers NOT in use
        /// </summary>
        private readonly Stack<T[]> _freeBuffers = new Stack<T[]>();

        /// <summary>
        /// Oject used to syncronise access to the BufferManager
        /// </summary>
        private readonly object _sync = new object();

        private readonly int _bufferSize;
        private readonly bool _clear;
        private int _count = 0;

        #region Constructors
        public StandardBufferManager(bool clear = true)
            : this(DefaultBufferSize, clear)
        {
        }

        public StandardBufferManager(int bufferSize, bool clear = true)
        {
            if (bufferSize <= 0)
                throw new ArgumentException("Buffer size must be bigger than 0", nameof(Buffer));

            _bufferSize = bufferSize;
            _clear = clear;
        }
        #endregion

        public void Init(int count)
        {
            lock (_sync)
            {
                while (count > 0)
                {
                    _freeBuffers.Push(new T[_bufferSize]);
                }
            }
        }

        /// <summary>
        /// The total number of <see cref="T"/> elements managed
        /// </summary>
        public long Size
        {
            get { return _bufferSize * _count; }
        }

        /// <summary>
        /// The total number of <see cref="T"/> elements free
        /// </summary>
        public long FreeSize
        {
            get
            {
                lock (_sync)
                {
                    return _freeBuffers.Count * _bufferSize;
                }
            }
        }

        /// <summary>
        /// Allocates and return the arrays for a total of <paramref name="size"/> <see cref="T"/> elements
        /// </summary>
        /// <param name="size">The total size of the arrays to return</param>
        /// <returns></returns>
        public T[][] Allocate(int size)
        {
            if (size <= 0)
                return new T[0][];

            var ret = new T[((size - 1) / _bufferSize) + 1][];

            for (int i = 0; i < ret.Length; i++)
                ret[i] = this.GetBuffer();

            return ret;
        }

        public void Free(T[][] data)
        {
            lock (_sync)
            {
                foreach (var d in data)
                    this.FreeInternal(d);
            }
        }

        public void Free(T[] data)
        {
            lock (_sync)
            {
                this.FreeInternal(data);
            }
        }

        private void FreeInternal(T[] data)
        {
            if (!_buffers.Contains(data))
                return;

            _freeBuffers.Push(data);
            _buffers.Remove(data);
        }

        private T[] GetBuffer()
        {
            lock (_sync)
            {
                return GetFreeBuffer() ?? AllocateInternal();
            }
        }

        private T[] GetFreeBuffer()
        {
            if (_freeBuffers.Count <= 0)
                return null;

            var ret = _freeBuffers.Pop();
            _buffers.Add(ret);
            if (_clear)
                Array.Clear(ret, 0, ret.Length);

            return ret;
        }

        private T[] AllocateInternal()
        {
            var ret = this.CreateBuffer();
            _buffers.Add(ret);
            return ret;
        }

        private T[] CreateBuffer()
        {
            var ret = new T[_bufferSize];
            _count++;
            return ret;
        }
    }
}
