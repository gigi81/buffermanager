using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Grillisoft.BufferManager.Unmanaged
{
	public class Simple : IContiguousUnmanagedBufferManager, IDisposable
    {
        private readonly ConcurrentDictionary<IntPtr, int> _buffers = new ConcurrentDictionary<IntPtr, int>();

        private readonly IAllocEvents _events;

        public Simple(IAllocEvents allocEvents = null)
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

		public void Dispose()
		{
            foreach(var buffer in _buffers)
            {
                Marshal.FreeHGlobal(buffer.Key);
                _events?.Free(buffer.Value);
            }

            _buffers.Clear();
		}

        public void Init(int buffers)
        {
            //nothing to do
        }
    }
}
