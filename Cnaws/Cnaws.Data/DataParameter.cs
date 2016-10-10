using System;

namespace Cnaws.Data
{
    public class DataParameter
    {
        private string _name;
        private object _value;
        //private int _index;

        public DataParameter(string name, object value)
        {
            _name = name;
            _value = value;
            //_index = 0;
        }

        public string Name
        {
            get { return _name; }
        }

        public object Value
        {
            get { return _value; }
        }

        internal virtual string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
                throw new NotSupportedException("join be use DataParameter<>");
            return string.Concat(ds.Provider.EscapeName(_name), "=", GetParameterName());
        }
        internal protected virtual string GetParameterName()
        {
            return string.Concat("@", _name);
        }

        public static implicit operator DataWhereQueue(DataParameter value)
        {
            return new DataWhereQueue(value);
        }
        public static DataWhereQueue operator &(DataParameter l, DataParameter r)
        {
            return new DataWhereQueue(l, r, DataWhereUnionType.And);
        }
        public static DataWhereQueue operator &(DataParameter l, DataWhereQueue r)
        {
            return new DataWhereQueue(l, r, DataWhereUnionType.And);
        }
        public static DataWhereQueue operator |(DataParameter l, DataParameter r)
        {
            return new DataWhereQueue(l, r, DataWhereUnionType.Or);
        }
        public static DataWhereQueue operator |(DataParameter l, DataWhereQueue r)
        {
            return new DataWhereQueue(l, r, DataWhereUnionType.Or);
        }
    }
    public class DataParameter<T>: DataParameter where T : DbTable
    {
        public DataParameter(string name, object value)
            : base(name, value)
        {
        }

        internal override string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
                return string.Concat(string.Concat(ds.Provider.EscapeName(DbTable.GetTableName<T>()), ".", ds.Provider.EscapeName(Name)), "=", GetParameterName());
            throw new NotSupportedException("not join be use DataParameter");
        }
    }
}
