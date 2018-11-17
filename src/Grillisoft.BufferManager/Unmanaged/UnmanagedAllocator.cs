using System;
using System.Runtime.InteropServices;

namespace Grillisoft.BufferManager.Unmanaged
{
    /// <summary>
    /// Unmanaged memory <see cref="IAllocator{T}"/>
    /// </summary>
    public class UnmanagedAllocator : IAllocator<IntPtr>
    {
        public IntPtr Allocate(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        public void Clear(IntPtr buffer, int size)
        {
            throw new NotImplementedException("Need to find a good implementation of memset");
        }

        public void Free(IntPtr buffer)
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}
