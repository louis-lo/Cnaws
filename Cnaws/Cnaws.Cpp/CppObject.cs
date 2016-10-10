using System;
using System.Diagnostics;

namespace Cnaws.Cpp
{
    public abstract class CppObject
    {
        [Conditional("DEBUG")]
        protected void ASSERT(bool condition)
        {
            Debug.Assert(condition);
        }
    }
}
