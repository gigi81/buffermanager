using System;

namespace Grillisoft.BufferManager.Unmanaged
{
    public interface IContiguousUnmanagedBufferManager : IDisposable
    {
        IntPtr Alloc(int size);

        void Free(IntPtr buffer);
    }
}