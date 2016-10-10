using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Cnaws.Data
{
    public abstract partial class DbTable : IDbReader
    {
        private static IList<T> ExecuteReaderWithLimitOrTop<T>(DataSource ds, string select, string where, string order, string group, long index, int size, out long count, DataParameter[] ps) where T : DbTable, new()
        {
            count = ExecuteCount<T>(ds, where, group, ps);
            return ds.ExecuteReader<T>(ds.Provider.BuildSelectSql(GetTableName<T>(), select, where, order, group, size, index), ps);
        }
        private static IList<T> SplitList<T>(IList<T> list, long index, int size) where T : DbTable, new()
        {
            List<T> array = new List<T>();
            for (long i = ((index - 1) * size); i < list.Count; ++i)
                array.Add(list[(int)i]);
            return array;
        }
        private static IList<T> ExecuteReaderWithRowNumber<T>(DataSource ds, string select, string where, string order, string group, long index, int size, out long count, DataParameter[] ps) where T : DbTable, new()
        {
            StringBuilder sb = new StringBuilder();

            count = ExecuteCount<T>(ds, where, group, ps);
            long half = count / 2;
            long lower = (index - 1) * size;
            long upper = index * size;

            if (string.IsNullOrEmpty(select))
                select = "*";

            if (where == null)
                where = string.Empty;
            else
                where = string.Concat("WHERE ", where);

            if (group == null)
                group = string.Empty;
            else
                group = string.Concat("GROUP BY ", group);

            if (order == null)
                order = string.Empty;

            string torder = order;

            if (lower > half)
            {
                if (torder.Length > 0)
                {
                    torder = Providers.MSSQLProvider.ORDER_REGEX.Replace(torder, new MatchEvaluator((m) =>
                    {
                        return "A".Equals(m.Groups[1].Value, StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
                    }));
                }
            }
            if (order.Length > 0)
                order = string.Concat("ORDER BY ", order);
            if (torder.Length > 0)
                torder = string.Concat("ORDER BY ", torder);

            long top = count - lower;
            if (top <= 0)
                return new List<T>();

            sb.Append("WITH CTE AS(SELECT TOP ");
            if (lower > half)
                sb.Append(top);
            else
                sb.Append(upper);
            sb.Append(" ROW_NUMBER() OVER(");
            sb.Append(torder);
            sb.Append(")AS _RowNumber,");
            sb.Append(select);
            sb.Append(" FROM ").Append(ds.Provider.EscapeName(GetTableName<T>()));

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

            return ds.ExecuteReader<T>(sb.ToString(), ps);
        }

        public static IList<T> ExecuteReader<T>(DataSource ds, DataOrder[] order, int index, int size, out int count, DataWhereQueue ps = null) where T : DbTable, new()
        {
            long total;
            IList<T> result = ExecuteReader<T>(ds, order, index, size, out total, ps);
            count = (int)total;
            return result;
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, DataOrder[] order, long index, int size, out long count, DataWhereQueue ps = null) where T : DbTable, new()
        {
            if (ds.Provider.SupperRowNumber)
                return ExecuteReaderWithRowNumber<T>(ds, null, DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), null, index, size, out count, DataWhereQueue.GetParameters(ps));
            IList<T> list = ExecuteReaderWithLimitOrTop<T>(ds, null, DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), null, index, size, out count, DataWhereQueue.GetParameters(ps));
            if (ds.Provider.SupperTop)
                list = SplitList(list, index, size);
            return list;
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, string[] columns, DataOrder[] order, int index, int size, out int count, DataWhereQueue ps = null) where T : DbTable, new()
        {
            long total;
            IList<T> result = ExecuteReader<T>(ds, columns, order, index, size, out total, ps);
            count = (int)total;
            return result;
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, string[] columns, string[] group, DataOrder[] order, int index, int size, out int count, DataWhereQueue ps = null) where T : DbTable, new()
        {
            long total;
            IList<T> result = ExecuteReader<T>(ds, columns, group, order, index, size, out total, ps);
            count = (int)total;
            return result;
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, string[] columns, DataOrder[] order, long index, int size, out long count, DataWhereQueue ps = null) where T : DbTable, new()
        {
            if (ds.Provider.SupperRowNumber)
                return ExecuteReaderWithRowNumber<T>(ds, DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), null, index, size, out count, DataWhereQueue.GetParameters(ps));
            IList<T> list = ExecuteReaderWithLimitOrTop<T>(ds, DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), null, index, size, out count, DataWhereQueue.GetParameters(ps));
            if (ds.Provider.SupperTop)
                list = SplitList(list, index, size);
            return list;
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, string[] columns, string[] group, DataOrder[] order, long index, int size, out long count, DataWhereQueue ps = null) where T : DbTable, new()
        {
            if (ds.Provider.SupperRowNumber)
                return ExecuteReaderWithRowNumber<T>(ds, DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false), index, size, out count, DataWhereQueue.GetParameters(ps));
            IList<T> list = ExecuteReaderWithLimitOrTop<T>(ds, DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false), index, size, out count, DataWhereQueue.GetParameters(ps));
            if (ds.Provider.SupperTop)
                list = SplitList(list, index, size);
            return list;
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, DataColumn[] columns, DataOrder[] order, int index, int size, out int count, DataWhereQueue ps = null) where T : DbTable, new()
        {
            long total;
            IList<T> result = ExecuteReader<T>(ds, columns, order, index, size, out total, ps);
            count = (int)total;
            return result;
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, DataColumn[] columns, DataColumn[] group, DataOrder[] order, int index, int size, out int count, DataWhereQueue ps = null) where T : DbTable, new()
        {
            long total;
            IList<T> result = ExecuteReader<T>(ds, columns, group, order, index, size, out total, ps);
            count = (int)total;
            return result;
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, DataColumn[] columns, DataOrder[] order, long index, int size, out long count, DataWhereQueue ps = null) where T : DbTable, new()
        {
            if (ds.Provider.SupperRowNumber)
                return ExecuteReaderWithRowNumber<T>(ds, DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), null, index, size, out count, DataWhereQueue.GetParameters(ps));
            IList<T> list = ExecuteReaderWithLimitOrTop<T>(ds, DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), null, index, size, out count, DataWhereQueue.GetParameters(ps));
            if (ds.Provider.SupperTop)
                list = SplitList(list, index, size);
            return list;
        }
        public static IList<T> ExecuteReader<T>(DataSource ds, DataColumn[] columns, DataColumn[] group, DataOrder[] order, long index, int size, out long count, DataWhereQueue ps = null) where T : DbTable, new()
        {
            if (ds.Provider.SupperRowNumber)
                return ExecuteReaderWithRowNumber<T>(ds, DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false), index, size, out count, DataWhereQueue.GetParameters(ps));
            IList<T> list = ExecuteReaderWithLimitOrTop<T>(ds, DataProvider.GetSqlString(columns, ds, false, true), DataProvider.GetSqlString(ps, ds, false, false), DataProvider.GetSqlString(order, ds, false, false), DataProvider.GetSqlString(group, ds, false, false), index, size, out count, DataWhereQueue.GetParameters(ps));
            if (ds.Provider.SupperTop)
                list = SplitList(list, index, size);
            return list;
        }

        private static IList<DataJoin<A, B>> ExecuteJoinReaderWithLimitOrTop<A, B>(DataSource ds, string select, string where, string order, string group, long index, int size, out long count, string aId, string bId, DataJoinType type, DataParameter[] ps) where A : DbTable, new() where B : DbTable, new()
        {
            count = ExecuteCount<A, B>(ds, where, group, aId, bId, type, ps);
            return ds.ExecuteReader<DataJoin<A, B>>(ds.Provider.BuildSelectJoinSql(GetTableName<A>(), GetTableName<B>(), type, aId, bId, select, where, order, group, size, index), ps);
        }
        private static IList<DataJoin<A, B>> SplitJoinList<A, B>(IList<DataJoin<A, B>> list, long index, int size) where A : DbTable, new() where B : DbTable, new()
        {
            List<DataJoin<A, B>> array = new List<DataJoin<A, B>>();
            for (long i = ((index - 1) * size); i < list.Count; ++i)
                array.Add(list[(int)i]);
            return array;
        }
        private static IList<DataJoin<A, B>> ExecuteJoinReaderWithRowNumber<A, B>(DataSource ds, string select, string where, DataOrder[] os, string group, long index, int size, out long count, string aId, string bId, DataJoinType type, DataParameter[] ps) where A : DbTable, new() where B : DbTable, new()
        {
            StringBuilder sb = new StringBuilder();

            count = ExecuteCount<A, B>(ds, where, group, aId, bId, type, ps);
            string tableA = GetTableName<A>();
            string tableB = GetTableName<B>();
            string ta = ds.Provider.EscapeName(tableA);
            string tb = ds.Provider.EscapeName(tableB);
            long half = count / 2;
            long lower = (index - 1) * size;
            long upper = index * size;

            if (string.IsNullOrEmpty(select))
                select = string.Concat(ta, ".*,", tb, ".*");
            if (where == null)
                where = string.Empty;
            else
                where = string.Concat("WHERE ", where);
            string order = DataProvider.GetSqlString(os, ds, true, true);
            string torder = DataProvider.GetSqlString(os, ds, true, false);
            if (order == null)
                order = string.Empty;
            if (torder == null)
                torder = string.Empty;

            if (lower > half)
            {
                if (torder.Length > 0)
                {
                    torder = Providers.MSSQLProvider.ORDER_REGEX.Replace(torder, new MatchEvaluator((m) =>
                    {
                        return "A".Equals(m.Groups[1].Value, StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
                    }));
                }
            }
            if (order.Length > 0)
                order = string.Concat("ORDER BY ", order);
            if (torder.Length > 0)
                torder = string.Concat("ORDER BY ", torder);

            long top = count - lower;
            if (top <= 0)
                return new List<DataJoin<A, B>>();

            sb.Append("WITH CTE AS(SELECT TOP ");
            if (lower > half)
                sb.Append(top);
            else
                sb.Append(upper);
            sb.Append(" ROW_NUMBER() OVER(");
            sb.Append(torder);
            sb.Append(")AS _RowNumber,");
            sb.Append(select);
            sb.Append(" FROM ");
            sb.Append(ta);
            sb.Append(" ");
            sb.Append(type.ToString().ToUpper());
            sb.Append(" JOIN ");
            sb.Append(tb);
            sb.Append(" ON ");
            sb.Append(ta);
            sb.Append(".");
            sb.Append(ds.Provider.EscapeName(aId));
            sb.Append("=");
            sb.Append(tb);
            sb.Append(".");
            sb.Append(ds.Provider.EscapeName(bId));
            if (where.Length > 0)
            {
                sb.Append(" ");
                sb.Append(where);
            }
            if (torder.Length > 0)
            {
                sb.Append(" ");
                sb.Append(torder);
            }
            sb.Append(")SELECT * FROM CTE WHERE _RowNumber>");
            if (lower > half)
                sb.Append(count - upper);
            else
                sb.Append(lower);
            if (order.Length > 0)
                sb.Append(' ').Append(order);
            sb.Append(";");
            return ds.ExecuteReader<DataJoin<A, B>>(sb.ToString(), ps);
        }

        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, DataOrder[] order, int index, int size, out int count, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            long total;
            IList<DataJoin<A, B>> result = ExecuteReader<A, B>(ds, order, index, size, out total, aId, bId, type, ps);
            count = (int)total;
            return result;
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, DataOrder[] order, long index, int size, out long count, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            if (ds.Provider.SupperRowNumber)
                return ExecuteJoinReaderWithRowNumber<A, B>(ds, null, DataProvider.GetSqlString(ps, ds, true, false), order, null, index, size, out count, aId, bId, type, DataWhereQueue.GetParameters(ps));
            IList<DataJoin<A, B>> list = ExecuteJoinReaderWithLimitOrTop<A, B>(ds, null, DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false), null, index, size, out count, aId, bId, type, DataWhereQueue.GetParameters(ps));
            if (ds.Provider.SupperTop)
                list = SplitJoinList(list, index, size);
            return list;
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, DataColumn[] columns, DataOrder[] order, int index, int size, out int count, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            long total;
            IList<DataJoin<A, B>> result = ExecuteReader<A, B>(ds, columns, order, index, size, out total, aId, bId, type, ps);
            count = (int)total;
            return result;
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, DataColumn[] columns, DataColumn[] group, DataOrder[] order, int index, int size, out int count, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            long total;
            IList<DataJoin<A, B>> result = ExecuteReader<A, B>(ds, columns, group, order, index, size, out total, aId, bId, type, ps);
            count = (int)total;
            return result;
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, DataColumn[] columns, DataOrder[] order, long index, int size, out long count, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            if (ds.Provider.SupperRowNumber)
                return ExecuteJoinReaderWithRowNumber<A, B>(ds, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), order, null, index, size, out count, aId, bId, type, DataWhereQueue.GetParameters(ps));
            IList<DataJoin<A, B>> list = ExecuteJoinReaderWithLimitOrTop<A, B>(ds, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false), null, index, size, out count, aId, bId, type, DataWhereQueue.GetParameters(ps));
            if (ds.Provider.SupperTop)
                list = SplitJoinList(list, index, size);
            return list;
        }
        public static IList<DataJoin<A, B>> ExecuteReader<A, B>(DataSource ds, DataColumn[] columns, DataColumn[] group, DataOrder[] order, long index, int size, out long count, string aId, string bId, DataJoinType type = DataJoinType.Inner, DataWhereQueue ps = null) where A : DbTable, new() where B : DbTable, new()
        {
            if (ds.Provider.SupperRowNumber)
                return ExecuteJoinReaderWithRowNumber<A, B>(ds, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), order, DataProvider.GetSqlString(group, ds, true, false), index, size, out count, aId, bId, type, DataWhereQueue.GetParameters(ps));
            IList<DataJoin<A, B>> list = ExecuteJoinReaderWithLimitOrTop<A, B>(ds, DataProvider.GetSqlString(columns, ds, true, true), DataProvider.GetSqlString(ps, ds, true, false), DataProvider.GetSqlString(order, ds, true, false), DataProvider.GetSqlString(group, ds, true, false), index, size, out count, aId, bId, type, DataWhereQueue.GetParameters(ps));
            if (ds.Provider.SupperTop)
                list = SplitJoinList(list, index, size);
            return list;
        }
    }
}
