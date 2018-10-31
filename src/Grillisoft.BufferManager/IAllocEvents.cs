namespace Grillisoft.BufferManager
{
    public interface IAllocEvents
    {
        void Allocate(int size);

        void Free(int size);
    }
}
