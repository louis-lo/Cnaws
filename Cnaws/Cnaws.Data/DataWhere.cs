using System;
using System.Text;
using System.Collections.Generic;

namespace Cnaws.Data
{
    internal enum DataWhereUnionType
    {
        And,
        Or
    }

    public class DataWhere : DataParameter
    {
        private string _operator;

        public DataWhere(string name, object value, string op = "=")
            : base(name, value)
        {
            _operator = op;
        }

        public string Operator
        {
            get { return _operator; }
        }

        internal override string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
                throw new NotSupportedException("join be use DataWhere<>");
            return string.Concat(ds.Provider.EscapeName(Name), ' ', _operator, ' ', GetParameterName());
        }
    }
    public class DataWhere<T> : DataWhere where T : DbTable
    {
        public DataWhere(string name, object value, string op = "=")
            : base(name, value, op)
        {
        }

        internal override string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
                return string.Concat(string.Concat(ds.Provider.EscapeName(DbTable.GetTableName<T>()), ".", ds.Provider.EscapeName(Name)), ' ', Operator, ' ', GetParameterName());
            throw new NotSupportedException("not join be use DataWhere");
        }
    }
    public class DataNameWhere : DataWhere
    {
        private string _pname;

        public DataNameWhere(string name, object value, string pname, string op = "=")
            : base(name, value, op)
        {
            if (pname == null)
                throw new ArgumentNullException("pname");
            _pname = pname;
        }

        public string ParameterName
        {
            get { return _pname; }
        }

        internal protected override string GetParameterName()
        {
            return string.Concat("@", _pname);
        }
    }
    public class DataNameWhere<T> : DataWhere<T> where T : DbTable
    {
        private string _pname;

        public DataNameWhere(string name, object value, string pname, string op = "=")
            : base(name, value, op)
        {
            if (pname == null)
                throw new ArgumentNullException("pname");
            _pname = pname;
        }

        public string ParameterName
        {
            get { return _pname; }
        }

        internal protected override string GetParameterName()
        {
            return string.Concat("@", _pname);
        }
    }
    public class DataFormatWhere : DataParameter
    {
        public static readonly DataFormatWhere Default = new DataFormatDefaultWhere();

        private string _format;

        public DataFormatWhere(string name, object value, string format)
            : base(name, value)
        {
            if (format == null)
                throw new ArgumentNullException("format");
            _format = format;
        }

        public string Format
        {
            get { return _format; }
        }

        internal override string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
                throw new NotSupportedException("join be use DataFormatWhere<>");
            return string.Format(_format, ds.Provider.EscapeName(Name), GetParameterName());
        }

        private sealed class DataFormatDefaultWhere : DataFormatWhere
        {
            public DataFormatDefaultWhere()
                : base(null, null, "1=1")
            {
            }

            internal override string GetSqlString(DataSource ds, bool prefix, bool select)
            {
                return Format;
            }
        }
    }
    public class DataFormatWhere<T> : DataFormatWhere where T : DbTable
    {
        public DataFormatWhere(string name, object value, string format)
            : base(name, value, format)
        {
        }

        internal override string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            if (prefix)
                return string.Format(Format, string.Concat(ds.Provider.EscapeName(DbTable.GetTableName<T>()), ".", ds.Provider.EscapeName(Name)), GetParameterName());
            throw new NotSupportedException("not join be use DataFormatWhere");
        }
    }

    public sealed class DataWhereQueue
    {
        private List<object> _queue;
        private List<DataParameter> _list;

        public DataWhereQueue()
        {
            _queue = new List<object>();
        }
        internal DataWhereQueue(DataParameter value)
        {
            _queue = new List<object>(1);
            _queue.Add(value);
            _list = new List<DataParameter>();
            _list.Add(value);

        }
        internal DataWhereQueue(DataParameter l, DataParameter r, DataWhereUnionType type)
        {
            AddQueue(l, r, type);
            _list.Add(l);
            _list.Add(r);
        }
        internal DataWhereQueue(DataParameter l, DataWhereQueue r, DataWhereUnionType type)
        {
            AddQueue(l, r, type);
            _list.Add(l);
            _list.AddRange(r.Parameters);
        }
        internal DataWhereQueue(DataWhereQueue l, DataParameter r, DataWhereUnionType type)
        {
            AddQueue(l, r, type);
            _list.AddRange(l.Parameters);
            _list.Add(r);
        }
        internal DataWhereQueue(DataWhereQueue l, DataWhereQueue r, DataWhereUnionType type)
        {
            AddQueue(l, r, type);
            _list.AddRange(l.Parameters);
            _list.AddRange(r.Parameters);
        }

        public DataParameter[] Parameters
        {
            get { return _list.ToArray(); }
        }
        public static DataParameter[] GetParameters(DataWhereQueue queue)
        {
            if (queue != null)
                return queue.Parameters;
            return null;
        }

        private void AddQueue(object l, object r, DataWhereUnionType type)
        {
            _queue = new List<object>(5);
            _queue.Add("(");
            _queue.Add(l);
            _queue.Add(GetUnionTypeString(type));
            _queue.Add(r);
            _queue.Add(")");
            _list = new List<DataParameter>(2);
        }

        private void Add(DataParameter value, DataWhereUnionType type)
        {
            AddQueue(value, type);
            _list.Add(value);
        }
        private void Add(DataWhereQueue value, DataWhereUnionType type)
        {
            AddQueue(value, type);
            _list.AddRange(value.Parameters);
        }
        private void AddQueue(object value, DataWhereUnionType type)
        {
            _queue.Insert(0, "(");
            _queue.Add(GetUnionTypeString(type));
            _queue.Add(value);
            _queue.Add(")");
        }

        private static string GetUnionTypeString(DataWhereUnionType type)
        {
            if (type == DataWhereUnionType.Or)
                return " OR ";
            return " AND ";
        }

        public static DataWhereQueue operator &(DataWhereQueue l, DataParameter r)
        {
            l.Add(r, DataWhereUnionType.And);
            return l;
        }
        public static DataWhereQueue operator &(DataWhereQueue l, DataWhereQueue r)
        {
            l.Add(r, DataWhereUnionType.And);
            return l;
        }
        public static DataWhereQueue operator |(DataWhereQueue l, DataParameter r)
        {
            l.Add(r, DataWhereUnionType.Or);
            return l;
        }
        public static DataWhereQueue operator |(DataWhereQueue l, DataWhereQueue r)
        {
            l.Add(r, DataWhereUnionType.Or);
            return l;
        }

        internal string GetSqlString(DataSource ds, bool prefix, bool select)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object value in _queue)
            {
                if (value is DataParameter)
                    sb.Append(((DataParameter)value).GetSqlString(ds, prefix, select));
                else if (value is DataWhereQueue)
                    sb.Append(((DataWhereQueue)value).GetSqlString(ds, prefix, select));
                else
                    sb.Append((string)value);
            }
            return sb.ToString();
        }
    }
}
