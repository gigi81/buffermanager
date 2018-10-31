using System;

namespace Grillisoft.BufferManager
{
    public interface IUnmanagedBufferManager : IDisposable
    {
        IntPtr[] Alloc(int size);

        IntPtr Free(IntPtr[] buffer);

        IntPtr Free(IntPtr buffer);
    }
}