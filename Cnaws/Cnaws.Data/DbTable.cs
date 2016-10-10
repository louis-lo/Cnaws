using Cnaws.ExtensionMethods;
using Cnaws.Templates;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Reflection;

namespace Cnaws.Data
{
    public enum DataJoinType
    {
        Inner,
        Left,
        Right
    }
    public enum DbAction
    {
        Insert,
        Update,
        Delete
    }

    public interface IDbReader
    {
        void ReadRow(DbDataReader reader);
    }
    //public interface IDbAction
    //{
    //    bool OnActionBefor(DataSource ds, DbAction action, ColumnMode mode, ref DataColumn[] columns);
    //    bool OnActionAfter(DataSource ds, DbAction action);
    //    void OnActionFailed(DataSource ds, DbAction action);
    //}
    
    public delegate string KeyHandler<T>(T value);
    
    [Serializable]
    public abstract partial class DbTable : IDbReader
    {
        #region Table Name
        private static class DbTableName<T>
        {
            public static readonly string Name;

            static DbTableName()
            {
                Name = TType<T>.Type.Name;
                DataTableAttribute att = Attribute.GetCustomAttribute(TType<T>.Type, TType<DataTableAttribute>.Type) as DataTableAttribute;
                if (att != null)
                {
                    if (!string.IsNullOrEmpty(att.Name))
                        Name = att.Name;
                }
            }
        }
        public string GetTableName()
        {
            return GetTableName(GetType());
        }
        internal static string GetTableName(Type type)
        {
            Type gtype = typeof(DbTableName<>).MakeGenericType(type);
            FieldInfo gfield = gtype.GetField("Name", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
            return (string)gfield.GetValue(null);
        }
        public static string GetTableName<T>()
        {
            return DbTableName<T>.Name;
        }
        #endregion

        #region Load From NameValueCollection
        public virtual void Load(NameValueCollection data)
        {
            object v;
            FieldInfo f;
            Dictionary<string, FieldInfo> fs = GetType().GetStaticAllNameSetFields<DataColumnAttribute>();
            foreach (string key in data.Keys)
            {
                if (fs.TryGetValue(key, out f))
                {
                    try
                    {
                        v = Types.GetObjectFromString(f.FieldType, data[key]);
                        f.SetValue(this, v);
                    }
                    catch (Exception) { }
                }
            }
        }
        public static T Load<T>(NameValueCollection data) where T : DbTable, new()
        {
            T ins = new T();
            Load(ins, data);
            return ins;
        }
        public static void Load<T>(T ins, NameValueCollection data) where T : DbTable
        {
            object v;
            FieldInfo f;
            Dictionary<string, FieldInfo> fs = TAllNameSetFields<T, DataColumnAttribute>.Fields;
            foreach (string key in data.Keys)
            {
                if (fs.TryGetValue(key, out f))
                {
                    try
                    {
                        v = Types.GetObjectFromString(f.FieldType, data[key]);
                        f.SetValue(ins, v);
                    }
                    catch (Exception) { }
                }
            }
        }
        #endregion

        #region Data Bind
        protected virtual void OnDataBind(DbDataReader reader)
        {
            object v;
            FieldInfo f;
            Dictionary<string, FieldInfo> fs = GetType().GetStaticAllNameSetFields<DataColumnAttribute>();
            foreach (string key in fs.Keys)
            {
                try
                {
                    f = fs[key];
                    v = DataUtility.FromDataType(reader[key], f.FieldType);
                    f.SetValue(this, v);
                }
                catch (IndexOutOfRangeException)
                {
                    try
                    {
                        f = fs[key];
                        v = DataUtility.FromDataType(reader[string.Concat(GetTableName(),'_', key)], f.FieldType);
                        f.SetValue(this, v);
                    }
                    catch (IndexOutOfRangeException) { }
                }
            }
        }
        void IDbReader.ReadRow(DbDataReader reader)
        {
            OnDataBind(reader);
        }
        #endregion

        private InsertBucket GetInsertSql(DataSource ds, ColumnMode mode, DataColumn[] keys)
        {
            Type gtype = typeof(TDbTable<>).MakeGenericType(GetType());
            MethodInfo method = gtype.GetMethod("GetInsertSql", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic);
            return (InsertBucket)method.Invoke(null, new object[] { ds, this, mode, keys });
        }
        private UpdateBucket GetUpdateSql(DataSource ds, ColumnMode mode, DataColumn[] keys, DataWhereQueue ps)
        {
            Type gtype = typeof(TDbTable<>).MakeGenericType(GetType());
            MethodInfo method = gtype.GetMethod("GetUpdateSql", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic);
            return (UpdateBucket)method.Invoke(null, new object[] { ds, this, mode, keys, ps });
        }
        private DeleteBucket GetDeleteSql(DataSource ds, DataColumn[] keys, DataWhereQueue ps)
        {
            Type gtype = typeof(TDbTable<>).MakeGenericType(GetType());
            MethodInfo method = gtype.GetMethod("GetDeleteSql", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic);
            return (DeleteBucket)method.Invoke(null, new object[] { ds, this, keys, ps });
        }
        private KeyValuePair<string, FieldInfo>[] GetPrimaryKeys()
        {
            Type gtype = typeof(TDbTable<>).MakeGenericType(GetType());
            FieldInfo field = gtype.GetField("PrimaryKeys", BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic);
            return (KeyValuePair<string, FieldInfo>[])field.GetValue(null);
        }

        public static T[] Exclude<T>(T[] keys, ColumnMode mode, T[] columns, KeyHandler<T> action)
        {
            if (columns == null)
                throw new ArgumentNullException("columns");
            if (columns.Length == 0)
                throw new ArgumentException();

            if (keys != null && keys.Length > 0)
            {
                Dictionary<string, T> dict = new Dictionary<string, T>(keys.Length);
                foreach (T key in keys)
                    dict.Add(action(key), key);

                string temp;
                T value;
                if (mode == ColumnMode.Include)
                {
                    foreach (T column in columns)
                    {
                        temp = action(column);
                        if (dict.TryGetValue(temp, out value))
                            dict.Remove(temp);
                    }
                }
                else
                {
                    foreach (T column in columns)
                    {
                        temp = action(column);
                        if (!dict.TryGetValue(temp, out value))
                            dict.Add(temp, column);
                    }
                }

                T[] array = new T[dict.Count];
                dict.Values.CopyTo(array, 0);
                return array;
            }
            else
            {
                if (mode == ColumnMode.Include)
                    return null;
                else
                    return columns;
            }
        }
        public static string[] Exclude(string[] keys, ColumnMode mode, params string[] columns)
        {
            return Exclude<string>(keys, mode, columns, (value) =>
            {
                return value;
            });
        }
        public static DataColumn[] Exclude(DataColumn[] keys, ColumnMode mode, params DataColumn[] columns)
        {
            return Exclude<DataColumn>(keys, mode, columns, (value) =>
            {
                return value.Column;
            });
        }
        public static bool Include<T>(T[] keys, ColumnMode mode, T column, KeyHandler<T> action)
        {
            if (column == null)
                throw new ArgumentNullException("column");

            if (keys != null && keys.Length > 0)
            {
                Dictionary<string, T> dict = new Dictionary<string, T>(keys.Length);
                foreach (T key in keys)
                    dict.Add(action(key), key);

                T value;
                bool result = dict.TryGetValue(action(column), out value);
                if (mode == ColumnMode.Include)
                    return result;
                else
                    return !result;
            }
            else
            {
                if (mode == ColumnMode.Include)
                    return false;
                else
                    return true;
            }
        }
        public static bool Include(string[] keys, ColumnMode mode, string column)
        {
            return Include<string>(keys, mode, column, (value) =>
            {
                return value;
            });
        }
        public static bool Include(DataColumn[] keys, ColumnMode mode, DataColumn column)
        {
            return Include<DataColumn>(keys, mode, column, (value) =>
            {
                return value.Column;
            });
        }

        public static int CreateTable<T>(DataSource ds) where T : DbTable
        {
            return ds.ExecuteNonQuery(TDbTable<T>.GetCreateTableSql(ds.Provider, GetTableName<T>()));
        }
        public static int CreateTable(DataSource ds, string table, Dictionary<string, KeyValuePair<FieldInfo, DataColumnAttribute>> fields, string[] pks)
        {
            return ds.ExecuteNonQuery(ds.Provider.GetCreateTableSql(table, fields, pks));
        }
        public static int DropTable<T>(DataSource ds, bool throwError = false) where T : DbTable
        {
            if (throwError)
            {
                return ds.ExecuteNonQuery(ds.Provider.GetDropTableSql(GetTableName<T>()));
            }
            else
            {
                try { return ds.ExecuteNonQuery(ds.Provider.GetDropTableSql(GetTableName<T>())); }
                catch (Exception) { return 0; }
            }
        }
        public int DropTable(DataSource ds, bool throwError = false)
        {
            if (throwError)
            {
                return ds.ExecuteNonQuery(ds.Provider.GetDropTableSql(GetTableName()));
            }
            else
            {
                try { return ds.ExecuteNonQuery(ds.Provider.GetDropTableSql(GetTableName())); }
                catch (Exception) { return 0; }
            }
        }
        public static int DropTable(DataSource ds, string table, bool throwError = false)
        {
            if (throwError)
            {
                return ds.ExecuteNonQuery(ds.Provider.GetDropTableSql(table));
            }
            else
            {
                try { return ds.ExecuteNonQuery(ds.Provider.GetDropTableSql(table)); }
                catch (Exception) { return 0; }
            }
        }
        public static int CreateIndex<T>(DataSource ds, string name, params string[] columns) where T : DbTable
        {
            return ds.ExecuteNonQuery(ds.Provider.GetCreateIndexSql(GetTableName<T>(), name, columns));
        }
        public int CreateIndex(DataSource ds, string name, params string[] columns)
        {
            return ds.ExecuteNonQuery(ds.Provider.GetCreateIndexSql(GetTableName(), name, columns));
        }
        public static int CreateIndex(DataSource ds, string table, string name, string[] columns)
        {
            return ds.ExecuteNonQuery(ds.Provider.GetCreateIndexSql(table, name, columns));
        }
        public static int DropIndex<T>(DataSource ds, string name, bool throwError = false) where T : DbTable
        {
            if (throwError)
            {
                return ds.ExecuteNonQuery(ds.Provider.GetDropIndexSql(GetTableName<T>(), name));
            }
            else
            {
                try { return ds.ExecuteNonQuery(ds.Provider.GetDropIndexSql(GetTableName<T>(), name)); }
                catch (Exception) { return 0; }
            }
        }
        public int DropIndex(DataSource ds, string name, bool throwError = false)
        {
            if (throwError)
            {
                return ds.ExecuteNonQuery(ds.Provider.GetDropIndexSql(GetTableName(), name));
            }
            else
            {
                try { return ds.ExecuteNonQuery(ds.Provider.GetDropIndexSql(GetTableName(), name)); }
                catch (Exception) { return 0; }
            }
        }
        public static int DropIndex(DataSource ds, string table, string name, bool throwError = false)
        {
            if (throwError)
            {
                return ds.ExecuteNonQuery(ds.Provider.GetDropIndexSql(table, name));
            }
            else
            {
                try { return ds.ExecuteNonQuery(ds.Provider.GetDropIndexSql(table, name)); }
                catch (Exception) { return 0; }
            }
        }

        public static int TruncateTable<T>(DataSource ds) where T : DbTable
        {
            return ds.ExecuteNonQuery(ds.Provider.GetTruncateSql(GetTableName<T>()));
        }
        public int TruncateTable(DataSource ds)
        {
            return ds.ExecuteNonQuery(ds.Provider.GetTruncateSql(GetTableName()));
        }
        public static int TruncateTable(DataSource ds, string table)
        {
            return ds.ExecuteNonQuery(ds.Provider.GetTruncateSql(table));
        }

        public static bool Vacuum(DataSource ds)
        {
            string sql = ds.Provider.GetVacuumSql();
            if (string.IsNullOrEmpty(sql))
                return false;
            ds.ExecuteNonQuery(ds.Provider.GetVacuumSql());
            return true;
        }

        protected abstract void SetId(long id);

        protected static int DeleteImpl<T>(T instance, DataSource ds, params DataColumn[] columns) where T : DbTable
        {
            DeleteBucket bucket = TDbTable<T>.GetDeleteSql(ds, instance, columns, null);
            return ds.ExecuteNonQuery(ds.Provider.BuildDeleteSql(GetTableName<T>(), bucket.Wheres), bucket.Parameters);
        }
        protected int DeleteImpl(DataSource ds, params DataColumn[] columns)
        {
            DeleteBucket bucket = GetDeleteSql(ds, columns, null);
            return ds.ExecuteNonQuery(ds.Provider.BuildDeleteSql(GetTableName(), bucket.Wheres), bucket.Parameters);
        }
        protected static int DeleteImpl<T>(T instance, DataSource ds, DataWhereQueue ps = null) where T : DbTable
        {
            DeleteBucket bucket = TDbTable<T>.GetDeleteSql(ds, instance, null, ps);
            return ds.ExecuteNonQuery(ds.Provider.BuildDeleteSql(GetTableName<T>(), bucket.Wheres), bucket.Parameters);
        }
        protected int DeleteImpl(DataSource ds, DataWhereQueue ps = null)
        {
            DeleteBucket bucket = GetDeleteSql(ds, null, ps);
            return ds.ExecuteNonQuery(ds.Provider.BuildDeleteSql(GetTableName(), bucket.Wheres), bucket.Parameters);
        }

        internal static bool InsertImpl(DataSource ds, string sql, params DataParameter[] ps)
        {
            return ds.ExecuteNonQuery(sql, ps) > 0;
        }
        internal static bool InsertImpl(DataSource ds, string sql, DbTable instance, params DataParameter[] ps)
        {
            if (ds.Provider.IsInsertReturnId(sql))
            {
                long result = ds.ExecuteScalar<long>(sql, ps);
                if (result > 0)
                {
                    instance.SetId(result);
                    return true;
                }
                instance.SetId(0L);
                return false;
            }
            int rows = ds.ExecuteNonQuery(sql, ps);
            if (rows > 0)
            {
#if(SQLite)
                if (ds.Type == DataSourceType.SQLite)
                {
                    instance.SetId(((System.Data.SQLite.SQLiteConnection)ds.Connection).LastInsertRowId);
                    return true;
                }
#endif
                instance.SetId(0L);
                return true;
            }
            instance.SetId(0L);
            return false;
        }
        internal static bool InsertImpl(DataSource ds, string sql, out long id, params DataParameter[] ps)
        {
            if (ds.Provider.IsInsertReturnId(sql))
            {
                long result = ds.ExecuteScalar<long>(sql, ps);
                if (result > 0)
                {
                    id = result;
                    return true;
                }
                id = 0L;
                return false;
            }
            int rows = ds.ExecuteNonQuery(sql, ps);
            if (rows > 0)
            {
#if(SQLite)
                if (ds.Type == DataSourceType.SQLite)
                {
                    id = ((System.Data.SQLite.SQLiteConnection)ds.Connection).LastInsertRowId;
                    return true;
                }
#endif
                id = 0L;
                return true;
            }
            id = 0L;
            return false;
        }

        protected static bool InsertImpl<T>(T instance, DataSource ds, ColumnMode mode, params DataColumn[] columns) where T : DbTable
        {
            InsertBucket bucket = TDbTable<T>.GetInsertSql(ds, instance, mode, columns);
            string sql = ds.Provider.BuildInsertSql(GetTableName<T>(), bucket.Names, bucket.Values, bucket.Id);
            return InsertImpl(ds, sql, instance, bucket.Parameters);
        }
        protected bool InsertImpl(DataSource ds, ColumnMode mode, params DataColumn[] columns)
        {
            InsertBucket bucket = GetInsertSql(ds, mode, columns);
            string sql = ds.Provider.BuildInsertSql(GetTableName(), bucket.Names, bucket.Values, bucket.Id);
            return InsertImpl(ds, sql, this, bucket.Parameters);
        }
        protected static bool InsertOrReplaceImpl<T>(T instance, DataSource ds, ColumnMode mode, params DataColumn[] columns) where T : DbTable
        {
            if (ds.Provider.SupperReplace)
            {
                InsertBucket bucket = TDbTable<T>.GetInsertSql(ds, instance, mode, columns);
                string sql = ds.Provider.BuildInsertOrReplaceSql(GetTableName<T>(), bucket.Names, bucket.Values, bucket.Id);
                return ds.ExecuteNonQuery(sql, bucket.Parameters) > 0;
            }

            DataWhereQueue ps = null;
            KeyValuePair<string, FieldInfo>[] pks = TDbTable<T>.PrimaryKeys;
            foreach (KeyValuePair<string, FieldInfo> pair in pks)
            {
                if (ps == null)
                    ps = new DataParameter(pair.Key, pair.Value.GetValue(instance));
                else
                    ps &= new DataParameter(pair.Key, pair.Value.GetValue(instance));
            }
            if (ExecuteCount<T>(ds, ps) > 0)
                return UpdateImpl<T>(instance, ds, mode, columns, ps) > 0;

            return InsertImpl<T>(instance, ds, mode, columns);
        }
        protected bool InsertOrReplaceImpl(DataSource ds, ColumnMode mode, params DataColumn[] columns)
        {
            if (ds.Provider.SupperReplace)
            {
                InsertBucket bucket = GetInsertSql(ds, mode, columns);
                string sql = ds.Provider.BuildInsertOrReplaceSql(GetTableName(), bucket.Names, bucket.Values, bucket.Id);
                return ds.ExecuteNonQuery(sql, bucket.Parameters) > 0;
            }

            DataWhereQueue ps = null;
            KeyValuePair<string, FieldInfo>[] pks = GetPrimaryKeys();
            foreach (KeyValuePair<string, FieldInfo> pair in pks)
            {
                if (ps == null)
                    ps = new DataParameter(pair.Key, pair.Value.GetValue(this));
                else
                    ps &= new DataParameter(pair.Key, pair.Value.GetValue(this));
            }
            if (ExecuteCount(ds, ps) > 0)
                return UpdateImpl(ds, mode, columns, ps) > 0;

            return InsertImpl(ds, mode, columns);
        }

        protected static int UpdateImpl<T>(T instance, DataSource ds, ColumnMode mode, DataColumn[] columns, DataWhereQueue ps = null) where T : DbTable
        {
            UpdateBucket bucket = TDbTable<T>.GetUpdateSql(ds, instance, mode, columns, ps);
            return ds.ExecuteNonQuery(ds.Provider.BuildUpdateSql(GetTableName<T>(), bucket.Sets, bucket.Wheres), bucket.Parameters);
        }
        protected int UpdateImpl(DataSource ds, ColumnMode mode, DataColumn[] columns, DataWhereQueue ps = null)
        {
            UpdateBucket bucket = GetUpdateSql(ds, mode, columns, ps);
            return ds.ExecuteNonQuery(ds.Provider.BuildUpdateSql(GetTableName(), bucket.Sets, bucket.Wheres), bucket.Parameters);
        }
    }
}
