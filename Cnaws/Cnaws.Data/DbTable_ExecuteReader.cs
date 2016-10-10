using System;
using System.Collections.Generic;
using System.Text;

namespace Cnaws.Data
{
    public abstract partial class DbTable : IDbReader
    {
        public static IList<T> ExecuteReader<T>(DataSource ds, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), null, DataProvider.GetSqlString(ps, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, int top, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), null, DataProvider.GetSqlString(ps, ds, false, false), null, null, top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, string[] columns, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, string[] columns, string[] group, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), null, DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, int top, string[] columns, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), null, null, top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, int top, string[] columns, string[] group, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), null, DataProvider.GetSqlString(group, ds, false, false), top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, DataColumn[] columns, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, DataColumn[] columns, DataColumn[] group, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), null, DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, int top, DataColumn[] columns, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), null, null, top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, int top, DataColumn[] columns, DataColumn[] group, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), null, DataProvider.GetSqlString(group, ds, false, false), top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), null, DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, int top, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), null, DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), null, top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, string[] columns, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, string[] columns, string[] group, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, int top, string[] columns, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), null, top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, int top, string[] columns, string[] group, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false), top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, DataColumn[] columns, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, DataColumn[] columns, DataColumn[] group, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, int top, DataColumn[] columns, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), null, top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, int top, DataColumn[] columns, DataColumn[] group, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false), top), DataWhereQueue.GetParameters(ps));
        }

        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteReader<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, null, DataProvider.GetSqlString(ps, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, int top, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteReader<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, null, DataProvider.GetSqlString(ps, ds, true, false), null, null, top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, DataColumn[] columns, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteReader<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, DataColumn[] columns, DataColumn[] group, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteReader<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), null, DataProvider.GetSqlString(group, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, int top, DataColumn[] columns, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteReader<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), null, null, top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, int top, DataColumn[] columns, DataColumn[] group, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteReader<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), null, DataProvider.GetSqlString(group, ds, true, false), top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, DataOrder[] order, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteReader<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, null, DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, int top, DataOrder[] order, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteReader<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, null, DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false), null, top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, DataColumn[] columns, DataOrder[] order, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteReader<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, DataColumn[] columns, DataColumn[] group, DataOrder[] order, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteReader<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false), DataProvider.GetSqlString(group, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, int top, DataColumn[] columns, DataOrder[] order, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteReader<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false), null, top), DataWhereQueue.GetParameters(ps));
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, int top, DataColumn[] columns, DataColumn[] group, DataOrder[] order, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteReader<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false), DataProvider.GetSqlString(group, ds, true, false), top), DataWhereQueue.GetParameters(ps));
        }
    }
}
