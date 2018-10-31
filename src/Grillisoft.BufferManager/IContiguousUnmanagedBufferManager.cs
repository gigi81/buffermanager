using System;

namespace Grillisoft.BufferManager
{
    public interface IContiguousUnmanagedBufferManager : IDisposable
    {
        IntPtr Alloc(int size);
        IntPtr Free(IntPtr buffer);
    }
}