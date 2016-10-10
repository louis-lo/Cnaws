using Cnaws.Templates;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cnaws.Data.Query
{
    public sealed class DbDeleteInstanceQuery<T> where T : IDbReader
    {
        private DbQuery<T> _query;
        private T _instance;

        internal DbDeleteInstanceQuery(DbQuery<T> query, T instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (TDbTable<T>.PrimaryKeys.Length == 0)
                throw new ArgumentException(string.Concat("data table \"", DbTable.GetTableName<T>(), "\" not have primary key"));
            _query = query;
            _instance = instance;
        }

        public int Execute()
        {
            int i = 0;
            DataParameter dp;
            StringBuilder wheres = new StringBuilder();
            KeyValuePair<string, FieldInfo>[] pks = TDbTable<T>.PrimaryKeys;
            List<DataParameter> list = new List<DataParameter>(pks.Length);
            foreach (KeyValuePair<string, FieldInfo> key in pks)
            {
                if (i++ > 0) wheres.Append(" AND ");
                wheres.Append(_query.Provider.EscapeName(key.Key));
                wheres.Append('=');
                dp = _query.BuildParameter(key.Value.GetValue(_instance));
                wheres.Append(dp.GetParameterName());
                list.Add(dp);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM ");
            sb.Append(_query.Provider.EscapeName(DbTable.GetTableName<T>()));
            sb.Append(" WHERE ");
            sb.Append(wheres.ToString());
            sb.Append(';');

            return _query.DataSource.ExecuteNonQuery(sb.ToString(), list.ToArray());
        }
    }
}
