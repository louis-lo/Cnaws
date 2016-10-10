using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;
using System.Collections.Generic;

namespace Cnaws.Statistic.Modules
{
    [Serializable]
    public class StatisticData : NoIdentityModule
    {
        [DataColumn(true)]
        public int Type = 0;
        [DataColumn(true)]
        public long TargetId = 0L;
        public long Count = 0L;
        public long Day = 0L;
        public long Week = 0L;
        public long Month = 0L;
        public long Year = 0L;
        public DateTime LastTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        public static void Add(DataSource ds, int type, long targetId)
        {
            ds.Begin();
            try
            {
                StatisticData sd = ExecuteSingleRow<StatisticData>(ds, P("Type", type) & P("TargetId", targetId));
                DateTime now = DateTime.Now;
                if (sd != null)
                {
                    DataColumn cd, cw, cm, cy;
                    if (sd.LastTime.DateDiff(now, DateDiffType.Day) != 0)
                    {
                        cd = C("Day");
                        sd.Day = 0L;
                    }
                    else
                    {
                        cd = MODC("Day", 1);
                    }
                    if (sd.LastTime.DateDiff(now, DateDiffType.Week) != 0)
                    {
                        cw = C("Week");
                        sd.Week = 0L;
                    }
                    else
                    {
                        cw = MODC("Week", 1);
                    }
                    if (sd.LastTime.DateDiff(now, DateDiffType.Month) != 0)
                    {
                        cm = C("Month");
                        sd.Month = 0L;
                    }
                    else
                    {
                        cm = MODC("Month", 1);
                    }
                    if (sd.LastTime.DateDiff(now, DateDiffType.Year) != 0)
                    {
                        cy = C("Year");
                        sd.Year = 0L;
                    }
                    else
                    {
                        cy = MODC("Year", 1);
                    }
                    sd.Update(ds, ColumnMode.Include, MODC("Count", 1), cd, cw, cm, cy);
                }
                else
                {
                    sd = new StatisticData()
                    {
                        Type = type,
                        TargetId = targetId,
                        Count = 1L,
                        Day = 1L,
                        Week = 1L,
                        Month = 1L,
                        Year = 1L,
                        LastTime = now
                    };
                    sd.Insert(ds);
                }
                ds.Commit();
            }
            catch (Exception)
            {
                ds.Rollback();
            }
        }

        public static IList<DataJoin<T, StatisticData>> GetPage<T>(DataSource ds, int type, DataSortType sort, DataColumn[] columns, DataOrder[] order, long index, int size, out long count, string idName, DataWhereQueue where) where T : DbTable, new()
        {
            List<DataColumn> list = new List<DataColumn>();
            if (columns == null)
                list.Add(C<T>("*"));
            else
                list.AddRange(columns);
            List<DataOrder> olist = new List<DataOrder>();
            if (type > 0)
                olist.Add(new DataOrder<StatisticData>("Count", sort));
            if (order != null)
                olist.AddRange(order);
            DataWhereQueue queue = WD;
            if (type > 0)
                queue &= P<StatisticData>("Type", type);
            if (queue != null)
                queue &= where;
            return ExecuteReader<T, StatisticData>(ds, list.ToArray(), olist.ToArray(), index, size, out count, idName, "TargetId", DataJoinType.Inner, queue);
        }
        public static IList<DataJoin<T, StatisticData>> GetTop<T>(DataSource ds, int type, int count, DataColumn[] columns, string idName, DataWhereQueue where) where T : DbTable, new()
        {
            List<DataColumn> list = new List<DataColumn>();
            if (columns == null)
                list.Add(C<T>("*"));
            else
                list.AddRange(columns);
            list.Add(C<StatisticData>("Count"));
            DataWhereQueue queue = P<StatisticData>("Type", type);
            if (queue != null)
                queue &= where;
            return ExecuteReader<T, StatisticData>(ds, count, list.ToArray(), Os(Od<StatisticData>("Count"), Od<StatisticData>("Year"), Od<StatisticData>("Month"), Od<StatisticData>("Week"), Od<StatisticData>("Day")), idName, "TargetId", DataJoinType.Inner, queue);
        }
        public static IList<DataJoin<T, StatisticData>> GetTopByYear<T>(DataSource ds, int type, int count, DataColumn[] columns, string idName, DataWhereQueue where) where T : DbTable, new()
        {
            List<DataColumn> list = new List<DataColumn>();
            if (columns == null)
                list.Add(C<T>("*"));
            else
                list.AddRange(columns);
            list.Add(C<StatisticData>("Year"));
            DataWhereQueue queue = P<StatisticData>("Type", type);
            if (queue != null)
                queue &= where;
            return ExecuteReader<T, StatisticData>(ds, count, list.ToArray(), Os(Od<StatisticData>("Year"), Od<StatisticData>("Count"), Od<StatisticData>("Month"), Od<StatisticData>("Week"), Od<StatisticData>("Day")), idName, "TargetId", DataJoinType.Inner, queue);
        }
        public static IList<DataJoin<T, StatisticData>> GetTopByMonth<T>(DataSource ds, int type, int count, DataColumn[] columns, string idName, DataWhereQueue where) where T : DbTable, new()
        {
            List<DataColumn> list = new List<DataColumn>();
            if (columns == null)
                list.Add(C<T>("*"));
            else
                list.AddRange(columns);
            list.Add(C<StatisticData>("Month"));
            DataWhereQueue queue = P<StatisticData>("Type", type);
            if (queue != null)
                queue &= where;
            return ExecuteReader<T, StatisticData>(ds, count, list.ToArray(), Os(Od<StatisticData>("Month"), Od<StatisticData>("Count"), Od<StatisticData>("Year"), Od<StatisticData>("Week"), Od<StatisticData>("Day")), idName, "TargetId", DataJoinType.Inner, queue);
        }
        public static IList<DataJoin<T, StatisticData>> GetTopByWeek<T>(DataSource ds, int type, int count, DataColumn[] columns, string idName, DataWhereQueue where) where T : DbTable, new()
        {
            List<DataColumn> list = new List<DataColumn>();
            if (columns == null)
                list.Add(C<T>("*"));
            else
                list.AddRange(columns);
            list.Add(C<StatisticData>("Week"));
            DataWhereQueue queue = P<StatisticData>("Type", type);
            if (queue != null)
                queue &= where;
            return ExecuteReader<T, StatisticData>(ds, count, list.ToArray(), Os(Od<StatisticData>("Week"), Od<StatisticData>("Count"), Od<StatisticData>("Year"), Od<StatisticData>("Month"), Od<StatisticData>("Day")), idName, "TargetId", DataJoinType.Inner, queue);
        }
        public static IList<DataJoin<T, StatisticData>> GetTopByDay<T>(DataSource ds, int type, int count, DataColumn[] columns, string idName, DataWhereQueue where) where T : DbTable, new()
        {
            List<DataColumn> list = new List<DataColumn>();
            if (columns == null)
                list.Add(C<T>("*"));
            else
                list.AddRange(columns);
            list.Add(C<StatisticData>("Day"));
            DataWhereQueue queue = P<StatisticData>("Type", type);
            if (queue != null)
                queue &= where;
            return ExecuteReader<T, StatisticData>(ds, count, list.ToArray(), Os(Od<StatisticData>("Day"), Od<StatisticData>("Count"), Od<StatisticData>("Year"), Od<StatisticData>("Month"), Od<StatisticData>("Week")), idName, "TargetId", DataJoinType.Inner, queue);
        }
    }
}
