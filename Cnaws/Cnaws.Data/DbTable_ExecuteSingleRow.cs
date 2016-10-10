namespace Cnaws.Data
{
    public abstract partial class DbTable : IDbReader
    {
        public static T ExecuteSingleRow<T>(DataSource ds, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), null, DataProvider.GetSqlString(ps, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static T ExecuteSingleRow<T>(DataSource ds, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), null, DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static T ExecuteSingleRow<T>(DataSource ds, string[] columns, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static T ExecuteSingleRow<T>(DataSource ds, string[] columns, string[] group, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), null, DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static T ExecuteSingleRow<T>(DataSource ds, string[] columns, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static T ExecuteSingleRow<T>(DataSource ds, string[] columns, string[] group, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static T ExecuteSingleRow<T>(DataSource ds, DataColumn[] columns, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static T ExecuteSingleRow<T>(DataSource ds, DataColumn[] columns, DataColumn[] group, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), null, DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static T ExecuteSingleRow<T>(DataSource ds, DataColumn[] columns, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static T ExecuteSingleRow<T>(DataSource ds, DataColumn[] columns, DataColumn[] group, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }

        public static bool ExecuteSingleRow<T>(T instance, DataSource ds, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(instance, ds.Provider.BuildSelectSql(GetTableName<T>(), null, DataProvider.GetSqlString(ps, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static bool ExecuteSingleRow<T>(T instance, DataSource ds, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(instance, ds.Provider.BuildSelectSql(GetTableName<T>(), null, DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static bool ExecuteSingleRow<T>(T instance, DataSource ds, string[] columns, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(instance, ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static bool ExecuteSingleRow<T>(T instance, DataSource ds, string[] columns, string[] group, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(instance, ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), null, DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static bool ExecuteSingleRow<T>(T instance, DataSource ds, string[] columns, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(instance, ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static bool ExecuteSingleRow<T>(T instance, DataSource ds, string[] columns, string[] group, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(instance, ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static bool ExecuteSingleRow<T>(T instance, DataSource ds, DataColumn[] columns, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(instance, ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static bool ExecuteSingleRow<T>(T instance, DataSource ds, DataColumn[] columns, DataColumn[] group, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(instance, ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), null, DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static bool ExecuteSingleRow<T>(T instance, DataSource ds, DataColumn[] columns, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(instance, ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }
        public static bool ExecuteSingleRow<T>(T instance, DataSource ds, DataColumn[] columns, DataColumn[] group, DataOrder[] order, DataWhereQueue ps = null) where T : DbTable, new()
        {
            return ds.ExecuteSingleRow<T>(instance, ds.Provider.BuildSelectSql(GetTableName<T>(), DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false)), DataWhereQueue.GetParameters(ps));
        }

        public static DataJoin<A, B> ExecuteSingleRow<A, B>(DataSource ds, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteSingleRow<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, null, DataProvider.GetSqlString(ps, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static DataJoin<A, B> ExecuteSingleRow<A, B>(DataSource ds, DataOrder[] order, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteSingleRow<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, null, DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static DataJoin<A, B> ExecuteSingleRow<A, B>(DataSource ds, DataColumn[] columns, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteSingleRow<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static DataJoin<A, B> ExecuteSingleRow<A, B>(DataSource ds, DataColumn[] columns, DataColumn[] group, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteSingleRow<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), null, DataProvider.GetSqlString(group, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static DataJoin<A, B> ExecuteSingleRow<A, B>(DataSource ds, DataColumn[] columns, DataOrder[] order, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteSingleRow<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
        public static DataJoin<A, B> ExecuteSingleRow<A, B>(DataSource ds, DataColumn[] columns, DataColumn[] group, DataOrder[] order, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            return ds.ExecuteSingleRow<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false), DataProvider.GetSqlString(group, ds, true, false)), DataWhereQueue.GetParameters(ps));
        }
    }
}
