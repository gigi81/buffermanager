using System;
using System.Collections.Generic;
using System.Text;

namespace Grillisoft.BufferManager
{
    public interface ICacheEvents
    {
        void Cache(int size);

        void FreeCache(int size);
    }
}
