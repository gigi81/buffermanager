using System;

namespace Grillisoft.BufferManager
{
    public interface IUnmanagedBufferManager : IDisposable
    {
        IntPtr[] Allocate(int size);

        void Free(IntPtr[] buffer);

        void Free(IntPtr buffer);
    }
}