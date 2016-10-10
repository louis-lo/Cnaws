using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using Cnaws.Data.Query;
using Cnaws.ExtensionMethods;
using System.Collections.Generic;

namespace Cnaws.Statistic.Modules
{
    [Serializable]
    public sealed class StatisticTag : NoIdentityModule
    {
        [DataColumn(true, 36)]
        public string Name = null;
        public int Length = 0;
        public long Count = 0L;
        public long Day = 0L;
        public long Week = 0L;
        public long Month = 0L;
        public long Year = 0L;
        public DateTime LastTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        public bool DisEnabled = false;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Length");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Length", "Length");
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            Length = Name.Length;
            return DataStatus.Success;
        }

        public static void Add(DataSource ds, string[] keys)
        {
            ds.Begin();
            try
            {
                foreach (string name in keys)
                {
                    StatisticTag sd = Db<StatisticTag>.Query(ds)
                    .Select()
                    .Where(W("Name", name))
                    .First<StatisticTag>();
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
                        sd = new StatisticTag()
                        {
                            Name = name,
                            Length = name.Length,
                            Count = 1L,
                            Day = 1L,
                            Week = 1L,
                            Month = 1L,
                            Year = 1L,
                            LastTime = now
                        };
                        sd.Insert(ds);
                    }
                }
                ds.Commit();
            }
            catch (Exception)
            {
                ds.Rollback();
            }
        }

        public static IList<StatisticTag> GetTop(DataSource ds, int count, int length = 0)
        {
            return Db<StatisticTag>.Query(ds)
                .Select()
                .Where(W("Length", length, length > 0 ? DbWhereType.LessThanOrEqual : DbWhereType.GreaterThan) & W("DisEnabled", false))
                .OrderBy(D("Count"), D("Year"), D("Month"), D("Week"), D("Day"))
                .ToList<StatisticTag>(count);
        }
        public static IList<StatisticTag> GetTopByYear(DataSource ds, int count, int length = 0)
        {
            return Db<StatisticTag>.Query(ds)
                .Select()
                .Where(W("Length", length, length > 0 ? DbWhereType.LessThanOrEqual : DbWhereType.GreaterThan))
                .OrderBy(D("Year"), D("Count"), D("Month"), D("Week"), D("Day"))
                .ToList<StatisticTag>(count);
        }
        public static IList<StatisticTag> GetTopByMonth(DataSource ds, int count, int length = 0)
        {
            return Db<StatisticTag>.Query(ds)
                .Select()
                .Where(W("Length", length, length > 0 ? DbWhereType.LessThanOrEqual : DbWhereType.GreaterThan))
                .OrderBy(D("Month"), D("Count"), D("Year"), D("Week"), D("Day"))
                .ToList<StatisticTag>(count);
        }
        public static IList<StatisticTag> GetTopByWeek(DataSource ds, int count, int length = 0)
        {
            return Db<StatisticTag>.Query(ds)
                .Select()
                .Where(W("Length", length, length > 0 ? DbWhereType.LessThanOrEqual : DbWhereType.GreaterThan))
                .OrderBy(D("Week"), D("Count"), D("Year"), D("Month"), D("Day"))
                .ToList<StatisticTag>(count);
        }
        public static IList<StatisticTag> GetTopByDay(DataSource ds, int count, int length = 0)
        {
            return Db<StatisticTag>.Query(ds)
                .Select()
                .Where(W("Length", length, length > 0 ? DbWhereType.LessThanOrEqual : DbWhereType.GreaterThan))
                .OrderBy(D("Day"), D("Count"), D("Year"), D("Month"), D("Week"))
                .ToList<StatisticTag>(count);
        }
    }
}
