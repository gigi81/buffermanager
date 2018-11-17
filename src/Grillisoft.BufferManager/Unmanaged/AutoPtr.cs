using System;

namespace Grillisoft.BufferManager.Unmanaged
{
    public class AutoPtr : IDisposable
    {
        public readonly BufferPtr Ptr;

        private Action<BufferPtr> _free;

        public AutoPtr(BufferPtr ptr, Action<BufferPtr> free)
        {
            Ptr = ptr;

            if(ptr.Ptr != IntPtr.Zero && free != null)
                _free = free;
            else
                GC.SuppressFinalize(this);
        }

        ~AutoPtr()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (_free == null)
                return;

            _free.Invoke(Ptr);
            _free = null;

            GC.SuppressFinalize(this);
        }

        public override int GetHashCode()
        {
            return Ptr.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if(obj is AutoPtr)
                return Ptr.Equals(((AutoPtr)obj).Ptr);

            return Ptr.Equals(obj);
        }

        public override string ToString()
        {
            return this.Ptr.ToString();
        }

        /// <summary>
        /// Explicit AutoPtr to IntPtr conversion operator
        /// </summary>
        /// <param name="autoPtr"></param>
        public static explicit operator IntPtr(AutoPtr autoPtr)
        {
            return autoPtr.Ptr.Ptr;
        }
    }
}
