using System;

namespace Grillisoft.BufferManager.Unmanaged
{
    public class AutoPtr : IDisposable
    {
        private readonly IntPtr _ptr;
        private Action<IntPtr> _free;

        public AutoPtr(IntPtr ptr, Action<IntPtr> free)
        {
            _ptr = ptr;

            if(ptr != IntPtr.Zero)
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

            _free.Invoke(_ptr);
            _free = null;

            GC.SuppressFinalize(this);
        }

        public override int GetHashCode()
        {
            return _ptr.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if(obj is AutoPtr)
                return _ptr.Equals(((AutoPtr)obj)._ptr);

            return _ptr.Equals(obj);
        }

        public override string ToString()
        {
            return _ptr.ToString();
        }

        /// <summary>
        /// Explicit AutoPtr to IntPtr conversion operator
        /// </summary>
        /// <param name="autoPtr"></param>
        public static explicit operator IntPtr(AutoPtr autoPtr)
        {
            return autoPtr._ptr;
        }
    }
}
