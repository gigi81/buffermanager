namespace Grillisoft.BufferManager.Unmanaged
{
    public static class AutoPtrExtensions
    {
        public static AutoPtr[] AllocateAuto(this IUnmanagedBufferManager manager, int size)
        {
            var ptrs = manager.Allocate(size);
            var ret = new AutoPtr[ptrs.Length];

            for (int i = 0; i < ptrs.Length; i++)
                ret[i] = new AutoPtr(ptrs[i], ptr => manager.Free(ptr));

            return ret;
        }

        public static AutoPtr AllocateAuto(this IContiguousUnmanagedBufferManager manager, int size)
        {
            return new AutoPtr(manager.Allocate(size), ptr => manager.Free(ptr));
        }
    }
}
