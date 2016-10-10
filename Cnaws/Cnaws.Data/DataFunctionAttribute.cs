using System;

namespace Cnaws.Data
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class DataFunctionAttribute : Attribute, ICustomName
    {
        private string _name;

        public DataFunctionAttribute()
            : this(null)
        {
        }
        public DataFunctionAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
