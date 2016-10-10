using System;

namespace Cnaws.Data
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class DataTableAttribute : Attribute, ICustomName
    {
        private string _name;

        public DataTableAttribute()
            : this(null)
        {
        }
        public DataTableAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
