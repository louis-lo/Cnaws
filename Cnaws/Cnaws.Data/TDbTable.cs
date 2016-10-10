using Cnaws.ExtensionMethods;
using Cnaws.Templates;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cnaws.Data
{
    public sealed class TDbTable<T> where T : IDbReader
    {
        private static readonly string[] PrimaryKey;
        private static readonly string[] IdentityKey;
        internal static readonly KeyValuePair<string, FieldInfo>[] PrimaryKeys;
        internal static readonly KeyValuePair<string, FieldInfo>[] IdentityKeys;
        internal static readonly KeyValuePair<string, bool>[] PrimaryKeyEx;
        internal static readonly KeyValuePair<string, bool>[] IdentityKeyEx;

        static TDbTable()
        {
            List<string> array = new List<string>();
            List<string> arrayi = new List<string>();
            List<KeyValuePair<string, FieldInfo>> arrays = new List<KeyValuePair<string, FieldInfo>>();
            List<KeyValuePair<string, FieldInfo>> arrayis = new List<KeyValuePair<string, FieldInfo>>();
            List<KeyValuePair<string, bool>> arrayex = new List<KeyValuePair<string, bool>>();
            List<KeyValuePair<string, bool>> arrayiex = new List<KeyValuePair<string, bool>>();
            KeyValuePair<FieldInfo, DataColumnAttribute> pair;
            Dictionary<string, KeyValuePair<FieldInfo, DataColumnAttribute>> fields = TType<T>.Type.GetStaticAllNameGetAttFields<DataColumnAttribute>();
            foreach (string key in fields.Keys)
            {
                pair = fields[key];
                if (pair.Value != null)
                {
                    if (pair.Value.IsPrimaryKey)
                    {
                        array.Add(key);
                        arrays.Add(new KeyValuePair<string, FieldInfo>(key, pair.Key));
                        arrayex.Add(new KeyValuePair<string, bool>(key, pair.Value.IsIdentity));
                    }
                    if (pair.Value.IsIdentity)
                    {
                        arrayi.Add(key);
                        arrayis.Add(new KeyValuePair<string, FieldInfo>(key, pair.Key));
                        arrayiex.Add(new KeyValuePair<string, bool>(key, pair.Value.IsIdentity));
                    }
                }
            }
            PrimaryKey = array.ToArray();
            IdentityKey = arrayi.ToArray();
            PrimaryKeys = arrays.ToArray();
            IdentityKeys = arrayis.ToArray();
            PrimaryKeyEx = arrayex.ToArray();
            IdentityKeyEx = arrayiex.ToArray();
        }

        internal static string GetCreateTableSql(DataProvider provider, string table)
        {
            return provider.GetCreateTableSql(table, TAllNameGetAttFields<T, DataColumnAttribute>.Fields, PrimaryKey);
        }
        internal static InsertBucket GetInsertSql(DataSource ds, T instance, ColumnMode mode, DataColumn[] keys)
        {
            return DataProvider.GetInsertSql(ds, instance, mode, keys, TAllNameGetAttFields<T, DataColumnAttribute>.Fields, IdentityKeyEx);
        }
        internal static UpdateBucket GetUpdateSql(DataSource ds, T instance, ColumnMode mode, DataColumn[] keys, DataWhereQueue ps)
        {
            return DataProvider.GetUpdateSql(ds, instance, mode, keys, ps, TAllNameGetAttFields<T, DataColumnAttribute>.Fields, PrimaryKey);
        }
        internal static DeleteBucket GetDeleteSql(DataSource ds, T instance, DataColumn[] keys, DataWhereQueue ps)
        {
            return DataProvider.GetDeleteSql(ds, instance, keys, ps, TAllNameGetAttFields<T, DataColumnAttribute>.Fields, PrimaryKey);
        }
    }
}
