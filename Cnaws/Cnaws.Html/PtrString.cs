using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Cnaws.Html
{
    internal unsafe sealed class PtrString : IDisposable
    {
        private IntPtr _ptr;
        private int _length;

        public PtrString(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            if (str.Length == 0)
                throw new ArgumentException();
            _length = str.Length;
            _ptr = Marshal.AllocHGlobal(_length * sizeof(char));
            Marshal.Copy(str.ToCharArray(), 0, _ptr, str.Length);
        }

        public int Length
        {
            get { return _length; }
        }
        public char* Pointer
        {
            get { return (char*)_ptr.ToPointer(); }
        }

        public string Substring(int index, int length)
        {
            return new string(Pointer, index, length);
        }
        public override string ToString()
        {
            return new string(Pointer, 0, Length);
        }

        public static implicit operator PtrString(string str)
        {
            return new PtrString(str);
        }
        public static implicit operator string(PtrString str)
        {
            return str.ToString();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (_ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_ptr);
                _length = 0;
                _ptr = IntPtr.Zero;
            }
        }
        ~PtrString()
        {
            Dispose(false);
        }
    }
}
