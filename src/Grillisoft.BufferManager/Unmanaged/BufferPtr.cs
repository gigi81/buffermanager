using System;

namespace Grillisoft.BufferManager.Unmanaged
{
    public struct BufferPtr
    {
        /// <summary>
        /// A zero pointer to a zero sized array
        /// </summary>
        public static readonly BufferPtr Zero = new BufferPtr(IntPtr.Zero, 0);

        /// <summary>
        /// Pointer to start of the buffer
        /// </summary>
        public readonly IntPtr Ptr;

        /// <summary>
        /// Buffer size in bytes
        /// </summary>
        public readonly int Size;

        public BufferPtr(IntPtr ptr, int size)
        {
            this.Ptr = ptr;
            this.Size = size;
        }

        /// <summary>
        /// Explicit <see cref="BufferPtr"/> to <see cref="IntPtr"/> conversion operator
        /// </summary>
        /// <param name="arrayPtr"></param>
        public static explicit operator IntPtr(BufferPtr arrayPtr)
        {
            return arrayPtr.Ptr;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is BufferPtr)
                return this.Ptr.Equals(((BufferPtr)obj).Ptr);

            return this.Ptr.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Ptr.GetHashCode();
        }

        public override string ToString()
        {
            return this.Ptr.ToString();
        }
    }
}
