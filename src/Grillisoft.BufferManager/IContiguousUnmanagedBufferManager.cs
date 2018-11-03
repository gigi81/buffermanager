using System;

namespace Grillisoft.BufferManager
{
    public interface IContiguousUnmanagedBufferManager : IDisposable
    {
        IntPtr Alloc(int size);

        void Free(IntPtr buffer);
    }
}