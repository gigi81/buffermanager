namespace Grillisoft.BufferManager
{
    public interface IAllocEvents
    {
        long Allocated { get; }

        void Allocate(int size);

        void Free(int size);
    }
}
