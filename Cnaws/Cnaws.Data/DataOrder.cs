using Cnaws.Templates;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cnaws.Data
{
    public enum DataSortType
    {
        Asc,
        Desc
    }

    public class DataColumn
    {
        private string _column;

        public DataColumn(string column)
        {
            _column = column;
        }

        public string Column
        {
            get { return _column; }
        }

        internal virtual string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
                throw new NotSupportedException("join be use DataColumn<>");
            return ds.Provider.EscapeName(_column);
        }

        public static implicit operator DataColumn(string column)
        {
            return new DataColumn(column);
        }
    }
    public sealed class DataColumn<T> : DataColumn where T : DbTable
    {
        public DataColumn(string column)
            : base(column)
        {
        }

        internal override string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
            {
                string table = DbTable.GetTableName<T>();
                string column = ds.Provider.EscapeName(Column);
                if ("*".Equals(column))
                {
                    Dictionary<string, FieldInfo> fs = TAllNameSetFields<T, DataColumnAttribute>.Fields;
                    List<DataColumn<T>> list = new List<DataColumn<T>>(fs.Count);
                    foreach (string key in fs.Keys)
                        list.Add(new DataColumn<T>(key));
                    List<string> keys = new List<string>(list.Count);
                    foreach (DataColumn<T> c in list)
                        keys.Add(c.GetSqlString(ds, prefix, select));
                    return string.Join(",", keys.ToArray());
                }
                if (select)
                    return string.Concat(string.Concat(ds.Provider.EscapeName(table), '.', column, " AS ", ds.Provider.EscapeName(string.Concat(table, '_', Column))));
                return string.Concat(string.Concat(ds.Provider.EscapeName(table), '.', column));
            }
            throw new NotSupportedException("not join be use DataColumn");
        }
    }

    public class DataOrder : DataColumn
    {
        private DataSortType _sort;

        public DataOrder(string column, DataSortType sort = DataSortType.Asc)
            : base(column)
        {
            _sort = sort;
        }

        public DataSortType Sort
        {
            get { return _sort; }
        }

        internal override string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
                throw new NotSupportedException("join be use DataOrder<>");
            return string.Concat(ds.Provider.EscapeName(Column), " ", _sort.ToString());
        }
    }
    public sealed class DataOrder<T> : DataOrder where T : DbTable
    {
        public DataOrder(string column, DataSortType sort = DataSortType.Asc)
            : base(column, sort)
        {
        }

        internal override string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
            {
                if (select)
                    return string.Concat(string.Concat(ds.Provider.EscapeName(string.Concat(DbTable.GetTableName<T>(), "_", Column)), " ", Sort.ToString()));
                return string.Concat(string.Concat(ds.Provider.EscapeName(DbTable.GetTableName<T>()), ".", ds.Provider.EscapeName(Column)), " ", Sort.ToString());
            }
            return string.Concat(ds.Provider.EscapeName(Column), " ", Sort.ToString());
        }
    }

    public class DataRenameColumn : DataColumn
    {
        private string _name;

        public DataRenameColumn(string column, string name = null)
            : base(column)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        internal override string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
                throw new NotSupportedException("join be use DataRenameColumn<>");
            if (_name != null)
                return string.Concat(ds.Provider.EscapeName(Column), " AS ", ds.Provider.EscapeName(_name));
            return ds.Provider.EscapeName(Column);
        }
    }
    public sealed class DataRenameColumn<T> : DataRenameColumn where T : DbTable
    {
        public DataRenameColumn(string column, string name = null)
            : base(column, name)
        {
        }

        internal override string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
            {
                if (Name != null)
                    return string.Concat(ds.Provider.EscapeName(DbTable.GetTableName<T>()), ".", ds.Provider.EscapeName(Column), " AS ", ds.Provider.EscapeName(Name));
                string table = DbTable.GetTableName<T>();
                if (select)
                    return string.Concat(string.Concat(ds.Provider.EscapeName(table), '.', ds.Provider.EscapeName(Column), " AS ", ds.Provider.EscapeName(string.Concat(table, '_', Column))));
                return string.Concat(string.Concat(ds.Provider.EscapeName(table), '.', ds.Provider.EscapeName(Column)));
            }
            throw new NotSupportedException("not join be use DataRenameColumn");
        }
    }

    public abstract class DataMethodColumn : DataRenameColumn
    {
        protected DataMethodColumn(string column, string name = null)
            : base(column, name)
        {
        }

        protected abstract string Method { get; }

        internal override string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
                throw new NotSupportedException("join be use DataMethodColumn<>");
            if (Name != null)
                return string.Concat(Method, "(", ds.Provider.EscapeName(Column), ") AS ", ds.Provider.EscapeName(Name));
            return string.Concat(Method, "(", ds.Provider.EscapeName(Column), ")");
        }
    }
    public abstract class DataMethodColumn<T> : DataMethodColumn where T : DbTable
    {
        protected DataMethodColumn(string column, string name = null)
            : base(column, name)
        {
        }

        internal override string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
            {
                if (Name != null)
                    return string.Concat(Method, "(", ds.Provider.EscapeName(DbTable.GetTableName<T>()), ".", ds.Provider.EscapeName(Column), ") AS ", ds.Provider.EscapeName(Name));
                return string.Concat(Method, "(", ds.Provider.EscapeName(DbTable.GetTableName<T>()), ".", ds.Provider.EscapeName(Column), ")");
            }
            throw new NotSupportedException("not join be use DataMethodColumn");
        }
    }

    public sealed class DataCountColumn : DataMethodColumn
    {
        public DataCountColumn(string column, string name = null)
            : base(column, name)
        {
        }

        protected override string Method
        {
            get { return "COUNT"; }
        }
    }
    public sealed class DataCountColumn<T> : DataMethodColumn<T> where T : DbTable
    {
        public DataCountColumn(string column, string name = null)
            : base(column, name)
        {
        }

        protected override string Method
        {
            get { return "COUNT"; }
        }
    }

    public sealed class DataSumColumn : DataMethodColumn
    {
        public DataSumColumn(string column, string name = null)
            : base(column, name)
        {
        }

        protected override string Method
        {
            get { return "SUM"; }
        }
    }
    public sealed class DataSumColumn<T> : DataMethodColumn<T> where T : DbTable
    {
        public DataSumColumn(string column, string name = null)
            : base(column, name)
        {
        }

        protected override string Method
        {
            get { return "SUM"; }
        }
    }

    public sealed class DataMaxColumn : DataMethodColumn
    {
        public DataMaxColumn(string column, string name = null)
            : base(column, name)
        {
        }

        protected override string Method
        {
            get { return "MAX"; }
        }
    }
    public sealed class DataMaxColumn<T> : DataMethodColumn<T> where T : DbTable
    {
        public DataMaxColumn(string column, string name = null)
            : base(column, name)
        {
        }

        protected override string Method
        {
            get { return "MAX"; }
        }
    }

    public sealed class DataMinColumn : DataMethodColumn
    {
        public DataMinColumn(string column, string name = null)
            : base(column, name)
        {
        }

        protected override string Method
        {
            get { return "MIN"; }
        }
    }
    public sealed class DataMinColumn<T> : DataMethodColumn<T> where T : DbTable
    {
        public DataMinColumn(string column, string name = null)
            : base(column, name)
        {
        }

        protected override string Method
        {
            get { return "MIN"; }
        }
    }

    //public sealed class DataValueColumn : DataColumn
    //{
    //    private object _value;

    //    public DataValueColumn(string column, object value)
    //        : base(column)
    //    {
    //        _value = value;
    //    }

    //    public object Value
    //    {
    //        get { return _value; }
    //    }

    //    internal override string GetSqlString(DataSource ds, bool prefix = false)
    //    {
    //        if (prefix)
    //            throw new NotSupportedException("not use to join");
    //        return string.Concat(provider.EscapeName(Column), "=", string.Concat("@", Column));
    //    }

    //    public static implicit operator DataParameter(DataValueColumn column)
    //    {
    //        return new DataParameter(column.Column, column.Value);
    //    }
    //}

    public abstract class DataActionColumn : DataColumn
    {
        public DataActionColumn(string column)
            : base(column)
        {
            ;
        }

        internal protected abstract object Parameter { get; }

        public static implicit operator DataParameter(DataActionColumn column)
        {
            return new DataParameter(column.Column, column.Parameter);
        }
    }
    public sealed class DataModifiedColumn<T> : DataActionColumn
    {
        private T _value;

        public DataModifiedColumn(string column, T value)
            : base(column)
        {
            _value = value;
        }

        public T Value
        {
            get { return _value; }
        }

        protected internal override object Parameter
        {
            get { return Value; }
        }

        internal override string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
                throw new NotSupportedException("not use to join");
            return string.Concat(ds.Provider.EscapeName(Column), "=", ds.Provider.EscapeName(Column), '+', string.Concat("@", Column));
        }
    }
}
