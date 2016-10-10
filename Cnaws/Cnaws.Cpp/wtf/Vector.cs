using System;
using System.Collections.Generic;

namespace Cnaws.Cpp.wtf
{
    public class Vector<T> : List<T>
    {
        public Vector() { }
        public Vector(int capacity) : base(capacity) { }
    }
}
