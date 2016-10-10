using System;

namespace Cnaws.Data
{
    public abstract partial class DbTable : IDbReader
    {
        public long ExecuteCount(DataSource ds, DataWhereQueue ps = null)
        {
            return ExecuteCount(ds, DataProvider.GetSqlString(ps, ds, false, false), null, DataWhereQueue.GetParameters(ps));
        }
        public long ExecuteCount(DataSource ds, string[] group, DataWhereQueue ps = null)
        {
            return ExecuteCount(ds, DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(group, ds, false, false), DataWhereQueue.GetParameters(ps));
        }
        public long ExecuteCount(DataSource ds, DataColumn[] group, DataWhereQueue ps = null)
        {
            return ExecuteCount(ds, DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(group, ds, false, false), DataWhereQueue.GetParameters(ps));
        }
        public static long ExecuteCount<T>(DataSource ds, DataWhereQueue ps = null) where T : DbTable
        {
            return ExecuteCount<T>(ds, DataProvider.GetSqlString(ps, ds, false, false), null, DataWhereQueue.GetParameters(ps));
        }
        public static long ExecuteCount<T>(DataSource ds, string[] group, DataWhereQueue ps = null) where T : DbTable
        {
            return ExecuteCount<T>(ds, DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(group, ds, false, false), DataWhereQueue.GetParameters(ps));
        }
        public static long ExecuteCount<T>(DataSource ds, DataColumn[] group, DataWhereQueue ps = null) where T : DbTable
        {
            return ExecuteCount<T>(ds, DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(group, ds, false, false), DataWhereQueue.GetParameters(ps));
        }
        public static long ExecuteCount<A, B>(DataSource ds, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable where B : DbTable
        {
            return ExecuteCount<A, B>(ds, DataProvider.GetSqlString(ps, ds, true, false), null, aId, bId, type, DataWhereQueue.GetParameters(ps));
        }
        public static long ExecuteCount<A, B>(DataSource ds, DataColumn[] group, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable where B : DbTable
        {
            return ExecuteCount<A, B>(ds, DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(group, ds, true, false), aId, bId, type, DataWhereQueue.GetParameters(ps));
        }

        private long ExecuteCount(DataSource ds, string where, string group, DataParameter[] ps)
        {
            return Convert.ToInt64(ds.ExecuteScalar(ds.Provider.BuildSelectCountSql(GetTableName(), where, group), ps));
        }
        private static long ExecuteCount<T>(DataSource ds, string where, string group, DataParameter[] ps) where T : DbTable
        {
            return Convert.ToInt64(ds.ExecuteScalar(ds.Provider.BuildSelectCountSql(GetTableName<T>(), where, group), ps));
        }
        private static long ExecuteCount<A, B>(DataSource ds, string where, string group, string aId, string bId, DataJoinType type, DataParameter[] ps) where A : DbTable where B : DbTable
        {
            return Convert.ToInt64(ds.ExecuteScalar(ds.Provider.BuildSelectCountSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, where, group), ps));
        }
    }
}
