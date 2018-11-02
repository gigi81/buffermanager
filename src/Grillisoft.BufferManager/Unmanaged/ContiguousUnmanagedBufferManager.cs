using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Grillisoft.BufferManager
{
	public class ContiguousUnmanagedBufferManager : IContiguousUnmanagedBufferManager, IDisposable
    {
        private readonly ConcurrentDictionary<IntPtr, int> _buffers = new ConcurrentDictionary<IntPtr, int>();

        private readonly IAllocEvents _events;

        public ContiguousUnmanagedBufferManager(IAllocEvents allocEvents = null)
        {
            _events = allocEvents;
        }

        public IntPtr Alloc(int size)
		{
		    if (size <= 0)
		        return IntPtr.Zero;

            var ret = Marshal.AllocHGlobal(size);
            _buffers.TryAdd(ret, size);
            _events?.Allocate(size);
            return ret;
        }

        public void Free(IntPtr buffer)
		{
		    if (buffer == IntPtr.Zero)
                return;

            if (!_buffers.TryRemove(buffer, out var size))
                return;

            Marshal.FreeHGlobal(buffer);
            _events?.Free(size);
        }

		#region IDisposable Members

		public void Dispose()
		{
            foreach(var ptr in _buffers.Keys)
                Marshal.FreeHGlobal(ptr);

            _buffers.Clear();
		}

		#endregion
	}
}
