namespace Grillisoft.BufferManager
{
    public interface IBufferManagerEvents
    {
        void Allocate(int size);

        void Free(int size);

        void Cache(int size);

        void FreeCache(int size);
    }
}
