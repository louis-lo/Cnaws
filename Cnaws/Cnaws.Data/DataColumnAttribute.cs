using System;

namespace Cnaws.Data
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class DataColumnAttribute : Attribute, ICustomName
    {
        private string _name;
        private bool _isPrimaryKey;
        private bool _isIdentity;
        private bool _isNullable;
        private int _size;
        private bool _isUnique;
        private object _defaultValue;

        public DataColumnAttribute(int size = 0)
            : this(null, false, false, true, size)
        {
        }
        public DataColumnAttribute(string name, int size = 0)
            : this(name, false, false, true, size)
        {
        }
        public DataColumnAttribute(bool isPrimaryKey, int size = 0)
            : this(null, isPrimaryKey, false, true, size)
        {
        }
        public DataColumnAttribute(bool isPrimaryKey, bool isIdentity, int size = 0)
            : this(null, isPrimaryKey, isIdentity, true, size)
        {
        }
        public DataColumnAttribute(string name, bool isPrimaryKey, int size = 0)
            : this(name, isPrimaryKey, false, true, size)
        {
        }
        public DataColumnAttribute(string name, bool isPrimaryKey, bool isIdentity, int size = 0)
            : this(null, isPrimaryKey, isIdentity, true, size)
        {
        }
        public DataColumnAttribute(string name, bool isPrimaryKey, bool isIdentity, bool isNullable, int size, bool isUnique = false, object defaultValue = null)
        {
            _name = name;
            _isPrimaryKey = isPrimaryKey;
            _isIdentity = isIdentity;
            _isNullable = isNullable;
            _size = size;
            _isUnique = isUnique;
            _defaultValue = defaultValue;
        }

        public string Name
        {
            get { return _name; }
        }
        public bool IsIdentity
        {
            get { return _isIdentity; }
        }
        public bool IsNullable
        {
            get { return _isNullable; }
        }
        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
        }
        public int Size
        {
            get { return _size; }
        }
        public bool IsUnique
        {
            get { return _isUnique; }
        }
        public object DefaultValue
        {
            get { return _defaultValue; }
        }
    }
}
