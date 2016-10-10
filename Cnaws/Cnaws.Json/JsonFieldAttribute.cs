using System;

namespace Cnaws.Json
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class JsonFieldAttribute : Attribute, ICustomName
    {
        private readonly string _name;

        public JsonFieldAttribute()
            : this(null)
        {
        }
        public JsonFieldAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
