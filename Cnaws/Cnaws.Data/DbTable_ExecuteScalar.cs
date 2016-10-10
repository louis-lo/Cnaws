using System;

namespace Cnaws.Data
{
    public abstract partial class DbTable : IDbReader
    {
        public static V ExecuteScalar<T, V>(DataSource ds, string column, DataWhereQueue ps = null) where T : DbTable
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectSql(GetTableName<T>(), ds.Provider.EscapeName(column), DataProvider.GetSqlString(ps, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<T, V>(DataSource ds, string column, string[] group, DataWhereQueue ps = null) where T : DbTable
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectSql(GetTableName<T>(), ds.Provider.EscapeName(column), DataProvider.GetSqlString(ps, ds, false, false), null, DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<T, V>(DataSource ds, string column, DataColumn[] group, DataWhereQueue ps = null) where T : DbTable
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectSql(GetTableName<T>(), ds.Provider.EscapeName(column), DataProvider.GetSqlString(ps, ds, false, false), null, DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<T, V>(DataSource ds, string column, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectSql(GetTableName<T>(), ds.Provider.EscapeName(column), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<T, V>(DataSource ds, string column, string[] group, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectSql(GetTableName<T>(), ds.Provider.EscapeName(column), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<T, V>(DataSource ds, string column, DataColumn[] group, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectSql(GetTableName<T>(), ds.Provider.EscapeName(column), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<T, V>(DataSource ds, DataColumn column, DataWhereQueue ps = null) where T : DbTable
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectSql(GetTableName<T>(), column.GetSqlString(ds, false, true), DataProvider.GetSqlString(ps, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<T, V>(DataSource ds, DataColumn column, string[] group, DataWhereQueue ps = null) where T : DbTable
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectSql(GetTableName<T>(), column.GetSqlString(ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), null, DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<T, V>(DataSource ds, DataColumn column, DataColumn[] group, DataWhereQueue ps = null) where T : DbTable
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectSql(GetTableName<T>(), column.GetSqlString(ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), null, DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<T, V>(DataSource ds, DataColumn column, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectSql(GetTableName<T>(), column.GetSqlString(ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<T, V>(DataSource ds, DataColumn column, string[] group, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectSql(GetTableName<T>(), column.GetSqlString(ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<T, V>(DataSource ds, DataColumn column, DataColumn[] group, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectSql(GetTableName<T>(), column.GetSqlString(ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<A, B, V>(DataSource ds, DataColumn column, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, column.GetSqlString(ds, true, true), DataProvider.GetSqlString(ps, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<A, B, V>(DataSource ds, DataColumn column, DataColumn[] group, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, column.GetSqlString(ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), null, DataProvider.GetSqlString(group, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<A, B, V>(DataSource ds, DataColumn column, DataOrder[] order, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, column.GetSqlString(ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static V ExecuteScalar<A, B, V>(DataSource ds, DataColumn column, DataColumn[] group, DataOrder[] order, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteScalar<V>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, column.GetSqlString(ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false), DataProvider.GetSqlString(group, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
    }
}
