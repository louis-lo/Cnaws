using Cnaws.Templates;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cnaws.Data.Query
{
    public sealed class DbUpdateInstanceQuery<T> where T : IDbReader
    {
        private DbQuery<T> _query;
        private T _instance;
        private ColumnMode _mode;
        private DbColumn[] _columns;

        internal DbUpdateInstanceQuery(DbQuery<T> query, T instance)
            : this(query, instance, ColumnMode.Include, null)
        {
        }
        internal DbUpdateInstanceQuery(DbQuery<T> query, T instance, DbColumn[] columns)
            : this(query, instance, ColumnMode.Include, columns)
        {
        }
        internal DbUpdateInstanceQuery(DbQuery<T> query, T instance, ColumnMode mode, DbColumn[] columns)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (columns != null && columns.Length == 0)
                throw new ArgumentException("columns length is 0");
            if (TDbTable<T>.PrimaryKeys.Length == 0)
                throw new ArgumentException(string.Concat("data table \"", DbTable.GetTableName<T>(), "\" not have primary key"));
            _query = query;
            _instance = instance;
            _mode = mode;
            _columns = columns;
        }

        public int Execute()
        {
            int i = 0;
            DataParameter dp;
            StringBuilder sets = new StringBuilder();
            StringBuilder wheres = new StringBuilder();
            Dictionary<string, KeyValuePair<FieldInfo, DataColumnAttribute>> fields = TAllNameGetAttFields<T, DataColumnAttribute>.Fields;
            List<DataParameter> list = new List<DataParameter>(fields.Count);
            if (_columns == null)
            {
                foreach (KeyValuePair<string, KeyValuePair<FieldInfo, DataColumnAttribute>> field in fields)
                {
                    if (field.Value.Value == null || (!field.Value.Value.IsPrimaryKey && !field.Value.Value.IsIdentity))
                    {
                        if (i++ > 0) sets.Append(',');
                        sets.Append(_query.Provider.EscapeName(field.Key));
                        sets.Append('=');
                        dp = _query.BuildParameter(field.Value.Key.GetValue(_instance));
                        sets.Append(dp.GetParameterName());
                        list.Add(dp);
                    }
                }
            }
            else
            {
                if (_mode == ColumnMode.Include)
                {
                    KeyValuePair<FieldInfo, DataColumnAttribute> pair;
                    foreach (DbColumn column in _columns)
                    {
                        if (fields.TryGetValue(column.Column, out pair))
                        {
                            if (pair.Value == null || (!pair.Value.IsPrimaryKey && !pair.Value.IsIdentity))
                            {
                                if (i++ > 0) sets.Append(',');
                                sets.Append(column.Build(_query.DataSource));
                                sets.Append('=');
                                dp = _query.BuildParameter(pair.Key.GetValue(_instance));
                                sets.Append(dp.GetParameterName());
                                list.Add(dp);
                            }
                        }
                        else
                        {
                            throw new ArgumentException(string.Concat("column \"", column.Column, "\" is not in data table \"", DbTable.GetTableName<T>(), "\""));
                        }
                    }
                }
                else
                {
                    Dictionary<string, KeyValuePair<FieldInfo, DataColumnAttribute>> keys = new Dictionary<string, KeyValuePair<FieldInfo, DataColumnAttribute>>(fields);
                    foreach (DbColumn column in _columns)
                        keys.Remove(column.Column);
                    foreach (KeyValuePair<string, KeyValuePair<FieldInfo, DataColumnAttribute>> field in fields)
                    {
                        if (field.Value.Value == null || (!field.Value.Value.IsPrimaryKey && !field.Value.Value.IsIdentity))
                        {
                            if (i++ > 0) sets.Append(',');
                            sets.Append(_query.Provider.EscapeName(field.Key));
                            sets.Append('=');
                            dp = _query.BuildParameter(field.Value.Key.GetValue(_instance));
                            sets.Append(dp.GetParameterName());
                            list.Add(dp);
                        }
                    }
                }
            }

            i = 0;
            KeyValuePair<string, FieldInfo>[] pks = TDbTable<T>.PrimaryKeys;
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
            sb.Append("UPDATE ");
            sb.Append(_query.Provider.EscapeName(DbTable.GetTableName<T>()));
            sb.Append(" SET ");
            sb.Append(sets.ToString());
            sb.Append(" WHERE ");
            sb.Append(wheres.ToString());
            sb.Append(';');

            return _query.DataSource.ExecuteNonQuery(sb.ToString(), list.ToArray());
        }
    }
}
