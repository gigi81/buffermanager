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

        public IntPtr CAllocate(int size)
        {
            var ret = Marshal.AllocHGlobal(size);
            ClearBuffer(ret, size);
            return ret;
        }

        public void Clear(IntPtr buffer, int size)
        {
            ClearBuffer(buffer, size);
        }

        public void Free(IntPtr buffer)
        {
            Marshal.FreeHGlobal(buffer);
        }

        /// <summary>
        /// Fills a block of memory with zeros.
        /// </summary>
        /// <param name="buffer">Pointer to the start of the block</param>
        /// <param name="size">Size of the block</param>
        /// <remarks>
        /// There is probably a more efficient way of doing this. Contributions are welcome but needs to be crossplatform
        /// </remarks>
        internal static void ClearBuffer(IntPtr buffer, int size)
        {
            if (buffer == IntPtr.Zero || size <= 0)
                return;

            for (int i = 0; i < size / 8; i += 8)
                Marshal.WriteInt64(buffer, i, 0x00);

            for (int i = size - (size % 8); i < size; i++)
                Marshal.WriteByte(buffer, i, 0x00);
        }
    }
}
