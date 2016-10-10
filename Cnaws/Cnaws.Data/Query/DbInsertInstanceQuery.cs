using Cnaws.Templates;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cnaws.Data.Query
{
    public sealed class DbInsertInstanceQuery<T> where T : IDbReader
    {
        private DbQuery<T> _query;
        private DbTable _instance;

        internal DbInsertInstanceQuery(DbQuery<T> query, T instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            _instance = instance as DbTable;
            if (_instance == null)
                throw new ArgumentException("instance not is DbTable");
            _query = query;
        }

        public bool Execute()
        {
            int i = 0;
            DataParameter dp;
            StringBuilder names = new StringBuilder();
            StringBuilder values = new StringBuilder();
            KeyValuePair<FieldInfo, DataColumnAttribute> pair;
            Dictionary<string, KeyValuePair<FieldInfo, DataColumnAttribute>> fields = TAllNameGetAttFields<T, DataColumnAttribute>.Fields;
            List<DataParameter> list = new List<DataParameter>(fields.Count);
            foreach (string key in fields.Keys)
            {
                pair = fields[key];
                if (pair.Value == null || !pair.Value.IsIdentity)
                {
                    if (i++ > 0)
                    {
                        names.Append(',');
                        values.Append(',');
                    }
                    names.Append(_query.Provider.EscapeName(key));
                    dp = _query.BuildParameter(pair.Key.GetValue(_instance));
                    values.Append(dp.GetParameterName());
                    list.Add(dp);
                }
            }

            string id = null;
            KeyValuePair<string, bool>[] pks = TDbTable<T>.IdentityKeyEx;
            if (pks.Length == 1)
            {
                KeyValuePair<string, bool> kvp = pks[0];
                if (kvp.Value)
                    id = kvp.Key;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(_query.Provider.EscapeName(DbTable.GetTableName<T>()));
            sb.Append(" (");
            sb.Append(names.ToString());
            sb.Append(") VALUES (");
            sb.Append(values.ToString());
            sb.Append(')');
            sb.Append(_query.Provider.GetInsertSqlEnd(id));
            sb.Append(';');

            return DbTable.InsertImpl(_query.DataSource, sb.ToString(), _instance, list.ToArray());
        }
    }
}
