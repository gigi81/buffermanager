using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Grillisoft.BufferManager
{
	public class ContiguousUnmanagedBufferManager : IContiguousUnmanagedBufferManager, IDisposable
    {
		private readonly List<BufferManagerItem> _usedBuffers = new List<BufferManagerItem>();
		private readonly List<BufferManagerItem> _freeBuffers = new List<BufferManagerItem>();
	    private long _allocatedBytes = 0;
	    private long _inUseBytes = 0;

		public IntPtr Alloc(int size)
		{
		    if (size <= 0)
		        return IntPtr.Zero;

			var ret = this.GetFree(size);
			if (ret != IntPtr.Zero)
				return ret;

			return AllocBuffer(size);
		}

	    public IntPtr Free(IntPtr buffer)
		{
		    if (buffer == IntPtr.Zero)
                return IntPtr.Zero;

			var ret = _usedBuffers.Find(i => i.Data == buffer);
			_usedBuffers.Remove(ret);
			_freeBuffers.Add(ret);
		    _inUseBytes -= ret.Size;

			return IntPtr.Zero;
		}

        private IntPtr AllocBuffer(int size)
        {
            var ret = Marshal.AllocHGlobal(size);
            _usedBuffers.Add(new BufferManagerItem(ret, size));
            _allocatedBytes += size;
            _inUseBytes += size;
            return ret;
        }

        private IntPtr GetFree(int size)
		{
			for (int i = 0; i < _freeBuffers.Count; i++)
			{
				if (_freeBuffers[i].Size < size)
					continue;

				var ret = _freeBuffers[i];
				_usedBuffers.Add(ret);
				_freeBuffers.Remove(ret);
			    _inUseBytes += ret.Size;
				return ret.Data;
			}

			return IntPtr.Zero;
		}
	
		#region IDisposable Members

		public void Dispose()
		{
			foreach (var buffer in _freeBuffers)
				Marshal.FreeHGlobal(buffer.Data);

			foreach (var buffer in _usedBuffers)
				Marshal.FreeHGlobal(buffer.Data);

			_freeBuffers.Clear();
			_usedBuffers.Clear();
            _allocatedBytes = 0;
		    _inUseBytes = 0;
		}

		#endregion
	}

	internal struct BufferManagerItem
	{
		public readonly IntPtr Data;
		public readonly int Size;

		public BufferManagerItem(IntPtr data, int size)
		{
			this.Data = data;
			this.Size = size;
		}

		public override int GetHashCode()
		{
			return this.Data.GetHashCode();
		}
	}
}
