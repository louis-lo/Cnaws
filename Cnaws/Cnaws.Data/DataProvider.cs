using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Cnaws.Data.Providers;
using System.Text.RegularExpressions;

namespace Cnaws.Data
{
    public enum ColumnMode
    {
        Include,
        Exclude
    }

    internal abstract class DataProvider
    {
        private static DataProvider _MSSQL;
        private static DataProvider _SQLite;
        private static DataProvider _PostgreSQL;
#if(MySQL)
        private static DataProvider _MySQL;
#endif

        private static DataProvider MSSQL
        {
            get
            {
                if (_MSSQL == null)
                    _MSSQL = new MSSQLProvider();
                return _MSSQL;
            }
        }
#if (PostgreSQL)
        private static DataProvider PostgreSQL
        {
            get
            {
                if (_PostgreSQL == null)
                    _PostgreSQL = new PostgreSQLProvider();
                return _PostgreSQL;
            }
        }
#endif
#if (SQLite)
        private static DataProvider SQLite
        {
            get
            {
                if (_SQLite == null)
                    _SQLite = new SQLiteProvider();
                return _SQLite;
            }
        }
#endif
#if (MySQL)
        private static DataProvider MySQL
        {
            get
            {
                if (_MySQL == null)
                    _MySQL = new TMySQLProvider();
                return _MySQL;
            }
        }
#endif

        internal static DataProvider GetProvider(DataSourceType type)
        {
            switch (type)
            {
                case DataSourceType.MSSQL: return MSSQL;
#if (PostgreSQL)
                case DataSourceType.PostgreSQL: return PostgreSQL;
#endif
#if (SQLite)
                case DataSourceType.SQLite: return SQLite;
#endif
#if (MySQL)
                case DataSourceType.MySQL: return MySQL;
#endif
            }
            throw new Cnaws.Data.DataException();
        }

        public string EscapeName(string name)
        {
            if (name == null || "*".Equals(name))
                return "*";
            return EscapeString(name);
        }
        public virtual string EscapeString(string name)
        {
            return string.Concat("[", name, "]");
        }
        public virtual string IdentityString
        {
            get { return string.Empty; }
        }
        public virtual string GetMutiPrimaryKeyBegin(string table)
        {
            return string.Empty;
        }
        public virtual string CreateSqlEnd
        {
            get { return string.Empty; }
        }
        public virtual string InsertOrReplaceSql
        {
            get { return string.Empty; ; }
        }
        public virtual string GetInsertSqlEnd(string id)
        {
            return string.Empty;
        }
        public virtual string GetIndexName(string table, string name)
        {
            return name;
        }
        public virtual string GetDropIndexSqlEnd(string table)
        {
            return string.Empty;
        }
        public virtual string GetTruncateSql(string table)
        {
            return string.Concat("DELETE FROM ", EscapeName(table), ";");
        }
        public virtual bool SupperReplace
        {
            get { return false; }
        }
        public virtual bool SupperLimit
        {
            get { return false; }
        }
        public virtual bool SupperTop
        {
            get { return false; }
        }
        public virtual bool SupperRowNumber
        {
            get { return false; }
        }
        public virtual bool IsInsertReturnId(string sql)
        {
            return false;
        }
        public virtual string GetVacuumSql()
        {
            return string.Empty;
        }
        public abstract string GetType(Type type, int size, bool id);

        public abstract string GetTable(string table);
        public abstract string GetColumn(string table, string column);
        public abstract string GetIndex(string table, string name, string[] columns);

        public static string GetSqlString(string[] columns, DataSource ds, bool prefix, bool select)
        {
            if (columns != null && columns.Length > 0)
            {
                int i = 0;
                StringBuilder sb = new StringBuilder();
                foreach (string column in columns)
                {
                    if (i++ > 0) sb.Append(',');
                    if (prefix)
                        sb.Append(column);
                    else
                        sb.Append(ds.Provider.EscapeName(column));
                }
                return sb.ToString();
            }
            return null;
        }
        public static string GetSqlString(DataColumn[] columns, DataSource ds, bool prefix, bool select)
        {
            if (columns != null && columns.Length > 0)
            {
                int i = 0;
                StringBuilder sb = new StringBuilder();
                foreach (DataColumn column in columns)
                {
                    if (i++ > 0) sb.Append(',');
                    sb.Append(column.GetSqlString(ds, prefix, select));
                }
                return sb.ToString();
            }
            return null;
        }
        public static string GetSqlString(DataWhereQueue queue, DataSource ds, bool prefix, bool select)
        {
            if (queue != null)
                return queue.GetSqlString(ds, prefix, select);
            return null;
        }
        public static string GetSqlString(DataOrder[] ps, DataSource ds, bool prefix, bool select)
        {
            if (ps != null && ps.Length > 0)
            {
                int i = 0;
                StringBuilder sb = new StringBuilder();
                foreach (DataOrder dp in ps)
                {
                    if (i++ > 0) sb.Append(',');
                    sb.Append(dp.GetSqlString(ds, prefix, select));
                }
                return sb.ToString();
            }
            return null;
        }

        public string GetCreateTableSql(string table, Dictionary<string, KeyValuePair<FieldInfo, DataColumnAttribute>> fields, string[] pks)
        {
            StringBuilder sb = new StringBuilder("CREATE TABLE ");
            sb.Append(EscapeName(table));
            sb.Append("(");
            int i = 0;
            KeyValuePair<FieldInfo, DataColumnAttribute> pair;
            foreach (string key in fields.Keys)
            {
                if (i++ > 0) sb.Append(',');
                pair = fields[key];
                sb.Append(EscapeName(key));
                sb.Append(" ");
                sb.Append(GetType(pair.Key.FieldType.IsEnum ? Enum.GetUnderlyingType(pair.Key.FieldType) : pair.Key.FieldType, pair.Value != null ? pair.Value.Size : 0, pair.Value != null ? pair.Value.IsIdentity : false));
                if (pair.Value != null)
                {
                    if (pair.Value.IsPrimaryKey && pks.Length == 1)
                        sb.Append(" PRIMARY KEY");

                    if (pair.Value.IsIdentity)
                        sb.Append(IdentityString);

                    if (pair.Value.IsUnique)
                        sb.Append(" UNIQUE");

                    if (pair.Value.DefaultValue != null)
                    {
                        sb.Append(" DEFAULT ");
                        sb.Append(pair.Value.DefaultValue.ToString());
                    }

                    if (!pair.Value.IsNullable || pair.Value.IsPrimaryKey || pair.Value.IsIdentity)
                        sb.Append(" NOT");
                }
                sb.Append(" NULL");
            }
            if (pks.Length > 1)
            {
                sb.Append(',');
                sb.Append(GetMutiPrimaryKeyBegin(table));
                sb.Append("PRIMARY KEY (");
                int j = 0;
                foreach (string key in pks)
                {
                    if (j++ > 0) sb.Append(',');
                    sb.Append(EscapeName(key));
                }
                sb.Append(")");
            }
            sb.Append(")");
            sb.Append(CreateSqlEnd);
            sb.Append(";");
            return sb.ToString();
        }
        public string GetDropTableSql(string table)
        {
            return string.Concat("DROP TABLE ", EscapeName(table), ";");
        }

        protected string GetIndexNameImpl(string table, string name)
        {
            return string.Concat("IX_", GetIndexName(table, name));
        }
        public string GetCreateIndexSql(string table, string name, string[] columns)
        {
            StringBuilder sb = new StringBuilder("CREATE INDEX ");
            sb.Append(EscapeName(GetIndexNameImpl(table, name)));
            sb.Append(" ON ");
            sb.Append(EscapeName(table));
            sb.Append(" (");
            int i = 0;
            foreach (string key in columns)
            {
                if (i++ > 0) sb.Append(',');
                sb.Append(EscapeName(key));
            }
            sb.Append(");");
            return sb.ToString();
        }
        public string GetDropIndexSql(string table, string name)
        {
            StringBuilder sb = new StringBuilder("DROP INDEX ");
            sb.Append(EscapeName(GetIndexNameImpl(table, name)));
            sb.Append(GetDropIndexSqlEnd(table));
            sb.Append(';');
            return sb.ToString();
        }

        public static InsertBucket GetInsertSql(DataSource ds, object instance, ColumnMode mode, DataColumn[] keys, Dictionary<string, KeyValuePair<FieldInfo, DataColumnAttribute>> fields, KeyValuePair<string, bool>[] pks)
        {
            StringBuilder names = new StringBuilder();
            StringBuilder values = new StringBuilder();
            List<DataParameter> list = new List<DataParameter>();

            KeyValuePair<FieldInfo, DataColumnAttribute> pair;
            if (mode == ColumnMode.Include)
            {
                if (keys == null)
                    throw new ArgumentNullException("keys");
                if (keys.Length == 0)
                    throw new ArgumentException();

                int i = 0;
                foreach (DataColumn key in keys)
                {
                    pair = fields[key.Column];
                    if (pair.Value == null || !pair.Value.IsIdentity)
                    {
                        if (i++ > 0)
                        {
                            names.Append(',');
                            values.Append(',');
                        }
                        names.Append(ds.Provider.EscapeName(key.Column));
                        DataParameter dp;
                        //if (key is DataValueColumn)
                        //    dp = (DataValueColumn)key;
                        //else
                        dp = new DataParameter(key.Column, pair.Key.GetValue(instance));
                        values.Append(dp.GetParameterName());
                        list.Add(dp);
                    }
                }
            }
            else
            {
                Dictionary<string, DataColumn> dict = null;
                if (keys != null)
                {
                    dict = new Dictionary<string, DataColumn>(keys.Length);
                    foreach (DataColumn key in keys)
                        dict.Add(key.Column, key);
                }

                int i = 0;
                DataColumn value;
                foreach (string key in fields.Keys)
                {
                    pair = fields[key];
                    if (pair.Value == null || !pair.Value.IsIdentity)
                    {
                        if (dict == null || dict.Count == 0 || !dict.TryGetValue(key, out value))
                        {
                            if (i++ > 0)
                            {
                                names.Append(',');
                                values.Append(',');
                            }
                            names.Append(ds.Provider.EscapeName(key));
                            DataParameter dp = new DataParameter(key, pair.Key.GetValue(instance));
                            values.Append(dp.GetParameterName());
                            list.Add(dp);
                        }
                    }
                }
            }

            string id = null;
            if (pks != null && pks.Length == 1)
            {
                KeyValuePair<string, bool> kvp = pks[0];
                if (kvp.Value)
                    id = kvp.Key;
            }
            return new InsertBucket(names.ToString(), values.ToString(), list.ToArray(), id);
        }
        public static UpdateBucket GetUpdateSql(DataSource ds, object instance, ColumnMode mode, DataColumn[] keys, DataWhereQueue ps, Dictionary<string, KeyValuePair<FieldInfo, DataColumnAttribute>> fields, string[] pks)
        {
            StringBuilder sets = new StringBuilder();
            StringBuilder wheres = new StringBuilder();
            List<DataParameter> list = new List<DataParameter>();

            KeyValuePair<FieldInfo, DataColumnAttribute> pair;
            if (mode == ColumnMode.Include)
            {
                if (keys == null)
                    throw new ArgumentNullException("keys or columns");
                if (keys.Length == 0)
                    throw new ArgumentException();

                int i = 0;
                if (keys != null && keys.Length > 0)
                {
                    foreach (DataColumn key in keys)
                    {
                        pair = fields[key.Column];
                        if (pair.Value == null || (!pair.Value.IsPrimaryKey && !pair.Value.IsIdentity))
                        {
                            if (i++ > 0)
                                sets.Append(',');
                            DataParameter dp;
                            //if (key is DataValueColumn)
                            //{
                            //    dp = (DataValueColumn)key;
                            //    sets.Append(dp.GetSqlString(this));
                            //}
                            //else 
                            if (key is DataActionColumn)
                            {
                                dp = (DataActionColumn)key;
                                sets.Append(key.GetSqlString(ds, false, false));
                            }
                            else
                            {
                                dp = new DataParameter(key.Column, pair.Key.GetValue(instance));
                                sets.Append(dp.GetSqlString(ds, false, false));
                            }
                            list.Add(dp);
                        }
                    }
                }
            }
            else
            {
                Dictionary<string, bool> dict = null;
                if (keys != null)
                {
                    dict = new Dictionary<string, bool>(keys.Length);
                    foreach (DataColumn key in keys)
                        dict.Add(key.Column, true);
                }

                int i = 0;
                bool value;
                foreach (string key in fields.Keys)
                {
                    pair = fields[key];
                    if (pair.Value == null || !pair.Value.IsPrimaryKey)
                    {
                        if (dict == null || dict.Count == 0 || !dict.TryGetValue(key, out value))
                        {
                            if (i++ > 0)
                                sets.Append(',');
                            DataParameter dp = new DataParameter(key, pair.Key.GetValue(instance));
                            sets.Append(dp.GetSqlString(ds, false, false));
                            list.Add(dp);
                        }
                    }
                }
            }

            int j = 0;
            if (ps != null)
            {
                wheres.Append(ps.GetSqlString(ds, false, false));
                DataParameter[] dp = ps.Parameters;
                if (dp.Length > 0)
                    list.AddRange(dp);
                j = dp.Length;
            }
            else
            {
                foreach (string key in pks)
                {
                    pair = fields[key];
                    if (j++ > 0)
                        wheres.Append(" AND ");
                    DataParameter dp = new DataParameter(key, pair.Key.GetValue(instance));
                    wheres.Append(dp.GetSqlString(ds, false, false));
                    list.Add(dp);
                }
            }
            return new UpdateBucket(sets.ToString(), wheres.ToString(), list.ToArray());
        }
        public static DeleteBucket GetDeleteSql(DataSource ds, object instance, DataColumn[] keys, DataWhereQueue ps, Dictionary<string, KeyValuePair<FieldInfo, DataColumnAttribute>> fields, string[] pks)
        {
            StringBuilder wheres = new StringBuilder();
            List<DataParameter> list = new List<DataParameter>();

            KeyValuePair<FieldInfo, DataColumnAttribute> pair;
            int i = 0;
            bool setted = false;
            if (keys != null && keys.Length > 0)
            {
                foreach (DataColumn key in keys)
                {
                    pair = fields[key.Column];
                    if (i++ > 0)
                        wheres.Append(" AND ");
                    DataParameter dp;
                    //if(key is DataValueColumn)
                    //    dp = (DataValueColumn)key;
                    //else
                    dp = new DataParameter(key.Column, pair.Key.GetValue(instance));
                    wheres.Append(dp.GetSqlString(ds, false, false));
                    list.Add(dp);
                }
                setted = true;
            }
            if (ps != null)
            {
                wheres.Append(ps.GetSqlString(ds, false, false));
                DataParameter[] dp = ps.Parameters;
                if (dp.Length > 0)
                    list.AddRange(dp);
                i = dp.Length;
                setted = true;
            }
            if (!setted)
            {
                foreach (string key in pks)
                {
                    pair = fields[key];
                    if (i++ > 0)
                        wheres.Append(" AND ");
                    DataParameter dp = new DataParameter(key, pair.Key.GetValue(instance));
                    wheres.Append(dp.GetSqlString(ds, false, false));
                    list.Add(dp);
                }
            }

            return new DeleteBucket(wheres.ToString(), list.ToArray());
        }

        public KeyValuePair<string, string> GetTopOrLimit(int size, long index)
        {
            long top = index * size;
            if (top > 0)
            {
                if (SupperLimit)
                    return new KeyValuePair<string, string>(string.Empty, string.Concat("LIMIT ", size, " OFFSET ", (index - 1) * size));
                if (SupperTop)
                    return new KeyValuePair<string, string>(string.Concat("TOP ", top, ' '), string.Empty);
                throw new NotSupportedException();
            }
            return new KeyValuePair<string, string>(string.Empty, string.Empty);
        }

        internal string BuildSelectSqlImpl(string top, string select, string table_and_join, string where, string group, string order, string limit, bool append)
        {
            StringBuilder sb = new StringBuilder("SELECT");

            if (!string.IsNullOrEmpty(top))
                sb.Append(' ').Append(top);

            sb.Append(' ').Append(select);

            sb.Append(" FROM ").Append(table_and_join);

            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE ").Append(where);

            if (!string.IsNullOrEmpty(group))
                sb.Append(" GROUP BY ").Append(group);

            if (!string.IsNullOrEmpty(order))
                sb.Append(" ORDER BY ").Append(order);

            if (!string.IsNullOrEmpty(limit))
                sb.Append(' ').Append(limit);

            if (!append)
                sb.Append(';');

            return sb.ToString();
        }
        internal string BuildSelectCountSqlImpl(string table_and_join, string where, string group)
        {
            if (string.IsNullOrEmpty(group))
                return BuildSelectSqlImpl(null, "COUNT(*)", table_and_join, where, group, null, null, false);
            return string.Concat("SELECT COUNT(*) FROM (", BuildSelectSqlImpl(null, "COUNT(*) AS C", table_and_join, where, group, null, null, true), ") AS T;");
        }
        internal string BuildSplitPageSqlImpl(string select, string table_and_join, string where, string group, string order, long half, long lower, long upper, long count)
        {
            long top = count - lower;
            if (top <= 0)
                return null;

            if (string.IsNullOrEmpty(where))
                where = string.Empty;
            else
                where = string.Concat("WHERE ", where);

            if (string.IsNullOrEmpty(group))
                group = string.Empty;
            else
                group = string.Concat("GROUP BY ", group);

            if (string.IsNullOrEmpty(order))
                order = string.Empty;

            string torder = order;
            if (lower > half)
            {
                if (torder.Length > 0)
                {
                    torder = MSSQLProvider.ORDER_REGEX.Replace(torder, new MatchEvaluator((m) =>
                    {
                        return "A".Equals(m.Groups[1].Value, StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
                    }));
                }
            }
            if (order.Length > 0)
                order = string.Concat("ORDER BY ", FormatCteOrder(order));
            if (torder.Length > 0)
                torder = string.Concat("ORDER BY ", torder);

            StringBuilder sb = new StringBuilder();
            sb.Append("WITH CTE AS(SELECT TOP ");
            if (lower > half)
                sb.Append(top);
            else
                sb.Append(upper);
            sb.Append(" ROW_NUMBER() OVER(");
            sb.Append(torder);
            sb.Append(")AS _RowNumber,");
            sb.Append(select);
            sb.Append(" FROM ").Append(table_and_join);

            if (where.Length > 0)
                sb.Append(' ').Append(where);
            if (group.Length > 0)
                sb.Append(' ').Append(group);
            if (torder.Length > 0)
                sb.Append(' ').Append(torder);

            sb.Append(")SELECT * FROM CTE WHERE _RowNumber>");
            if (lower > half)
                sb.Append(count - upper);
            else
                sb.Append(lower);

            if (order.Length > 0)
                sb.Append(' ').Append(order);

            sb.Append(';');
            return sb.ToString();
        }
        internal string BuildSelectSqlImpl(string top, string select, string table, string join, string where, string group, string order, string limit, bool append)
        {
            StringBuilder sb = new StringBuilder("SELECT");

            if (!string.IsNullOrEmpty(top))
                sb.Append(' ').Append(top);

            sb.Append(' ').Append(select);

            sb.Append(" FROM ").Append(table);

            if (!string.IsNullOrEmpty(join))
                sb.Append(' ').Append(join);

            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE ").Append(where);

            if (!string.IsNullOrEmpty(group))
                sb.Append(" GROUP BY ").Append(group);

            if (!string.IsNullOrEmpty(order))
                sb.Append(" ORDER BY ").Append(order);

            if (!string.IsNullOrEmpty(limit))
                sb.Append(' ').Append(limit);

            if (!append)
                sb.Append(';');

            return sb.ToString();
        }
        internal string BuildSelectCountSqlImpl(string table, string join, string where, string group)
        {
            if (string.IsNullOrEmpty(group))
                return BuildSelectSqlImpl(null, "COUNT(*)", table, join, where, group, null, null, false);
            return string.Concat("SELECT COUNT(*) FROM (", BuildSelectSqlImpl(null, "COUNT(*) AS C", table, join, where, group, null, null, true), ") AS T;");
        }
        internal string BuildSplitPageSqlImpl(string select, string table, string join, string where, string group, string order, long half, long lower, long upper, long count)
        {
            long top = count - lower;
            if (top <= 0)
                return null;

            if (string.IsNullOrEmpty(where))
                where = string.Empty;
            else
                where = string.Concat("WHERE ", where);

            if (string.IsNullOrEmpty(group))
                group = string.Empty;
            else
                group = string.Concat("GROUP BY ", group);

            if (string.IsNullOrEmpty(order))
                order = string.Empty;

            string torder = order;
            if (lower > half)
            {
                if (torder.Length > 0)
                {
                    torder = MSSQLProvider.ORDER_REGEX.Replace(torder, new MatchEvaluator((m) =>
                    {
                        return "A".Equals(m.Groups[1].Value, StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
                    }));
                }
            }
            if (order.Length > 0)
                order = string.Concat("ORDER BY ", FormatCteOrder(order));
            if (torder.Length > 0)
                torder = string.Concat("ORDER BY ", torder);

            StringBuilder sb = new StringBuilder();
            sb.Append("WITH CTE AS(SELECT TOP ");
            if (lower > half)
                sb.Append(top);
            else
                sb.Append(upper);
            sb.Append(" ROW_NUMBER() OVER(");
            sb.Append(torder);
            sb.Append(")AS _RowNumber,");
            sb.Append(select);
            sb.Append(" FROM ").Append(table);
            if (!string.IsNullOrEmpty(join))
                sb.Append(join);

            if (where.Length > 0)
                sb.Append(' ').Append(where);
            if (group.Length > 0)
                sb.Append(' ').Append(group);
            if (torder.Length > 0)
                sb.Append(' ').Append(torder);

            sb.Append(")SELECT * FROM CTE WHERE _RowNumber>");
            if (lower > half)
                sb.Append(count - upper);
            else
                sb.Append(lower);

            if (order.Length > 0)
                sb.Append(' ').Append(order);

            sb.Append(';');
            return sb.ToString();
        }

        public string BuildSelectSql(string table, string select = null, string where = null, string order = null, string group = null, int size = 0, long index = 1, bool append = false)
        {
            KeyValuePair<string, string> pair = GetTopOrLimit(size, index);
            if (string.IsNullOrEmpty(select))
                select = "*";
            return BuildSelectSqlImpl(pair.Key, select, EscapeName(table), null, where, group, order, pair.Value, append);
        }
        public string BuildSelectJoinSql(string tableA, string tableB, DataJoinType type, string aId, string bId, string select = null, string where = null, string order = null, string group = null, int size = 0, long index = 1, bool append = false)
        {
            string ta = EscapeName(tableA);
            string tb = EscapeName(tableB);
            KeyValuePair<string, string> pair = GetTopOrLimit(size, index);
            if (string.IsNullOrEmpty(select))
                select = string.Concat(ta, ".*,", tb, ".*");
            string join = string.Concat(type.ToString().ToUpper(), " JOIN ", tb, " ON ", ta, '.', EscapeName(aId), '=', tb, '.', EscapeName(bId));
            return BuildSelectSqlImpl(pair.Key, select, ta, join, where, group, order, pair.Value, append);
        }

        public string BuildSelectCountSql(string table, string where, string group = null)
        {
            return BuildSelectCountSqlImpl(EscapeName(table), null, where, group);
        }
        public string BuildSelectCountSql(string tableA, string tableB, DataJoinType type, string aId, string bId, string where, string group = null)
        {
            string ta = EscapeName(tableA);
            string tb = EscapeName(tableB);
            string join = string.Concat(type.ToString().ToUpper(), " JOIN ", tb, " ON ", ta, '.', EscapeName(aId), '=', tb, '.', EscapeName(bId));
            return BuildSelectCountSqlImpl(ta, join, where, group);
        }

        private static string FormatCteOrderEx(string order)
        {
            string[] array = order.Split('.');
            if (array.Length > 1)
                return array[array.Length - 1];
            return order;
        }
        private static string FormatCteOrder(string order)
        {
            string[] array = order.Split(',');
            for (int i = 0; i < array.Length; ++i)
                array[i] = FormatCteOrderEx(array[i]);
            return string.Join(",", array);
        }

        public string BuildInsertSql(string table, string names, string values, string id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(EscapeName(table));
            sb.Append(" (");
            sb.Append(names);
            sb.Append(") VALUES (");
            sb.Append(values);
            sb.Append(")");
            sb.Append(GetInsertSqlEnd(id));
            sb.Append(';');
            return sb.ToString();
        }
        public string BuildInsertOrReplaceSql(string table, string names, string values, string id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(InsertOrReplaceSql).Append(' ');
            sb.Append(EscapeName(table));
            sb.Append(" (");
            sb.Append(names);
            sb.Append(") VALUES (");
            sb.Append(values);
            sb.Append(")");
            if (!SupperReplace)
                sb.Append(GetInsertSqlEnd(id));
            sb.Append(';');
            return sb.ToString();
        }
        public string BuildUpdateSql(string table, string set, string where)
        {
            StringBuilder sb = new StringBuilder("UPDATE ");
            sb.Append(EscapeName(table));
            sb.Append(" SET ");
            sb.Append(set);
            if (!string.IsNullOrEmpty(where))
            {
                sb.Append(" WHERE ");
                sb.Append(where);
            }
            sb.Append(';');
            return sb.ToString();
        }
        public string BuildDeleteSql(string table, string where)
        {
            StringBuilder sb = new StringBuilder("DELETE FROM ");
            sb.Append(EscapeName(table));
            if (!string.IsNullOrEmpty(where))
            {
                sb.Append(" WHERE ");
                sb.Append(where);
            }
            sb.Append(';');
            return sb.ToString();
        }
    }
}
