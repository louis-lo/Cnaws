using System;
using System.Collections.Generic;
using System.Text;

namespace Cnaws.Data.Query
{
    public sealed class DbValuesQuery<T> where T : IDbReader
    {
        private DbInsertQuery<T> _query;
        private object[] _values;

        internal DbValuesQuery(DbInsertQuery<T> query, object[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            _query = query;
            _values = values;
        }

        public bool Execute()
        {
            int i = 0;
            DataParameter dp;
            StringBuilder values = new StringBuilder();
            List<DataParameter> list = new List<DataParameter>(_values.Length);
            foreach (object v in _values)
            {
                if (i++ > 0) values.Append(',');
                dp = _query.Query.BuildParameter(v);
                values.Append(dp.GetParameterName());
                list.Add(dp);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(_query.Query.Provider.EscapeName(DbTable.GetTableName<T>()));
            sb.Append(' ');
            sb.Append(_query.GetNames());
            sb.Append("VALUES (");
            sb.Append(values.ToString());
            sb.Append(");");

            return DbTable.InsertImpl(_query.Query.DataSource, sb.ToString(), list.ToArray());
        }
        public bool Execute(string column, out long value)
        {
            int i = 0;
            DataParameter dp;
            StringBuilder values = new StringBuilder();
            List<DataParameter> list = new List<DataParameter>(_values.Length);
            foreach (object v in _values)
            {
                if (i++ > 0) values.Append(',');
                dp = _query.Query.BuildParameter(v);
                values.Append(dp.GetParameterName());
                list.Add(dp);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(_query.Query.Provider.EscapeName(DbTable.GetTableName<T>()));
            sb.Append(' ');
            sb.Append(_query.GetNames());
            sb.Append("VALUES (");
            sb.Append(values.ToString());
            sb.Append(')');
            sb.Append(_query.Query.Provider.GetInsertSqlEnd(column));
            sb.Append(';');

            return DbTable.InsertImpl(_query.Query.DataSource, sb.ToString(), out value, list.ToArray());
        }
    }
}
