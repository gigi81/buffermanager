using System;

namespace Grillisoft.BufferManager.Unmanaged
{
    public interface IContiguousUnmanagedBufferManager : IDisposable
    {
        IntPtr Allocate(int size);

        void Free(IntPtr buffer);

        void Init(int buffers);
    }
}