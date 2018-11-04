namespace Grillisoft.BufferManager
{
    public interface IAllocator<T>
    {
        T Allocate(int size);

        void Free(T buffer);

        void Clear(T buffer, int size);
    }
}
