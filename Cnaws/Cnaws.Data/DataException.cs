using System;

namespace Cnaws.Data
{
    public class DataException : Exception
    {
        public DataException()
        { }
        public DataException(string msg)
            : base(msg)
        { }
    }
}
