using System;
using Cnaws.Data;
using M = Cnaws.Web.Modules;
using Cnaws.Json;
using System.Collections.Generic;
using Cnaws.Data.Query;
//using Cnaws.Data.Linq;
//using System.Linq;

namespace Cnaws.Web.Controllers
{
#if (DEBUG)
    public class DataTest : DataController
    {
        public void Index()
        {
            Render(@"<html>
<head>
<meta charset=""utf-8""/>
<style type=""text/css"">
*{margin:0;border:0;padding:0;}
p{height:14px;line-height:14px;font-size:12px;}
a{font-size:14px;}
</style>
</head>
<body>
<p><a href=""$url('/datatest/create')"">初始化数据</a></p>
<p><a href=""$url('/datatest/execsingle')"">单个查询</a></p>
<p><a href=""$url('/datatest/execreader')"">列表查询</a></p>
<p><a href=""$url('/datatest/execpage')"">分页查询</a></p>
<p><a href=""$url('/datatest/exectime')"">速度查询</a></p>
<p><a href=""$url('/datatest/insert')"">插入测试</a></p>
<p><a href=""$url('/datatest/update')"">更新测试</a></p>
<p><a href=""$url('/datatest/delete')"">删除测试</a></p>
</body>
</html>", "index.html");
        }

        private const string Id = "Id";
        private int X = 0;
        private DataColumn column = null;
        private DataColumn tcolumn = null;
        private DataColumn[] group = null;
        private DataColumn[] tgroup = null;
        private DataColumn[] columns = null;
        private DataColumn[] tcolumns = null;
        private DataParameter ps = null;
        private DataParameter tps = null;
        private DataOrder[] orders = null;
        private DataOrder[] torders = null;

        private DbSelect qcolumn = null;
        private DbSelect tqcolumn = null;
        private DbGroupBy[] qgroup = null;
        private DbGroupBy[] tqgroup = null;
        private DbSelect[] qcolumns = null;
        private DbSelect[] tqcolumns = null;
        private DbWhereQueue qps = null;
        private DbWhereQueue tqps = null;
        private DbOrderBy[] qorders = null;
        private DbOrderBy[] tqorders = null;
        private DbColumn<M.DataTestA> ca = null;
        private DbColumn<M.DataTestB> cb = null;

        private string FormatSql(string sql, params string[] names)
        {
            return string.Format(sql, Array.ConvertAll(names, new Converter<string, string>((x) =>
            {
                return DataSource.Provider.EscapeName(x);
            })));
        }
        private string FormatSql(string sql, int size, params string[] names)
        {
            KeyValuePair<string, string> pair = DataSource.Provider.GetTopOrLimit(size, 1);
            string[] array = new string[names.Length + 2];
            Array.Copy(Array.ConvertAll(names, new Converter<string, string>((x) =>
            {
                return DataSource.Provider.EscapeName(x);
            })), 0, array, 0, names.Length);
            array[names.Length] = pair.Key;
            array[names.Length + 1] = pair.Value;
            return string.Format(sql, array);
        }

        private void InitId()
        {
            X = Convert.ToInt32(DataSource.ExecuteScalar<int>(FormatSql("SELECT {0} FROM {1} WHERE {2}=512", "Int32", "DataTestA", "Id")));
            column = new DataColumn("Int32");
            tcolumn = new DataColumn<M.DataTestA>("Int32");
            group = new DataColumn[] { "Int32" };
            tgroup = new DataColumn[] { new DataColumn<M.DataTestA>("Int32") };
            columns = new DataColumn[] { "Int32" };
            tcolumns = new DataColumn[] { new DataColumn<M.DataTestA>("Int32") };
            ps = new DataParameter("Int32", X);
            tps = new DataParameter<M.DataTestA>("Int32", X);
            orders = new DataOrder[] { new DataOrder("Int32", DataSortType.Desc) };
            torders = new DataOrder[] { new DataOrder<M.DataTestA>("Int32", DataSortType.Desc) };

            qcolumn = new DbSelect("Int32");
            tqcolumn = new DbSelect<M.DataTestA>("Int32");
            qgroup = new DbGroupBy[] { "Int32" };
            tqgroup = new DbGroupBy[] { new DbGroupBy<M.DataTestA>("Int32") };
            qcolumns = new DbSelect[] { "Int32" };
            tqcolumns = new DbSelect[] { new DbSelect<M.DataTestA>("Int32") };
            qps = (new DbWhere("Int32")) == X;
            tqps = (new DbWhere<M.DataTestA>("Int32")) == X;
            qorders = new DbOrderBy[] { new DbOrderBy("Int32", DbOrderByType.Desc) };
            tqorders = new DbOrderBy[] { new DbOrderBy<M.DataTestA>("Int32", DbOrderByType.Desc) };
            ca = new DbColumn<M.DataTestA>(Id);
            cb = new DbColumn<M.DataTestB>(Id);
        }

        public void Create()
        {
            try
            {
                Module.Install<M.DataTestA>(DataSource);
                Module.Install<M.DataTestB>(DataSource);
                DataSource.Begin();
                try
                {
                    M.DataTestA a = new M.DataTestA();
                    M.DataTestB b = new M.DataTestB();
                    for (int i = 0; i < 1024; ++i)
                    {
                        a.Guid = Guid.NewGuid();
                        a.String = a.Guid.ToString("N");
                        a.Boolean = true;
                        a.DateTime = DateTime.Now;
                        a.Int32 = BitConverter.ToInt32(a.Guid.ToByteArray(), 0);
                        a.Int64 = BitConverter.ToInt64(a.Guid.ToByteArray(), 0);
                        a.Double = a.DateTime.Minute + a.DateTime.Second / 100.0;
                        a.Money = 99;
                        if (a.Insert(DataSource) != DataStatus.Success)
                            throw new Exception("a insert error");
                        b.Id = a.Id;
                        b.Guid = Guid.NewGuid();
                        b.String = a.Guid.ToString("N");
                        b.Boolean = true;
                        b.DateTime = DateTime.Now;
                        b.Int32 = BitConverter.ToInt32(a.Guid.ToByteArray(), 0);
                        b.Int64 = BitConverter.ToInt64(a.Guid.ToByteArray(), 0);
                        b.Double = a.DateTime.Minute + a.DateTime.Second / 100.0;
                        b.Money = 99;
                        if (b.Insert(DataSource) != DataStatus.Success)
                            throw new Exception("b insert error");
                    }
                    DataSource.Commit();
                    this["Content"] = "安装成功";
                }
                catch (Exception ex)
                {
                    DataSource.Rollback();
                    while (ex.InnerException != null)
                        ex = ex.InnerException;
                    this["Content"] = ex.Message + ex.StackTrace;
                }
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                this["Content"] = e.Message + e.StackTrace;
            }
            RenderComm();
        }
        public void ExecSingle()
        {
            try
            {
                InitId();

                this["count1"] = DbTable.ExecuteCount<M.DataTestA>(DataSource) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) select x.Id).Count();
                //Convert.ToInt64(DataSource.ExecuteScalar(FormatSql("SELECT COUNT(*) FROM {0}", "DataTestA")));
                //DataQuery.Select<M.DataTestA>(DataSource).Count();
                Db<M.DataTestA>.Query(DataSource).Select().Count();

                this["count2"] = DbTable.ExecuteCount<M.DataTestA>(DataSource, group) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) group x by x.Int32).Count();
                //Convert.ToInt64(DataSource.ExecuteScalar(FormatSql("SELECT COUNT(*) FROM (SELECT {0} FROM {1} GROUP BY {0}) AS T", "Int32", "DataTestA")));
                //DataQuery.Select<M.DataTestA>(DataSource).GroupBy(group).Count();
                Db<M.DataTestA>.Query(DataSource).Select().GroupBy(qgroup).Count();

                this["count3"] = DbTable.ExecuteCount<M.DataTestA>(DataSource, ps) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X select x.Id).Count();
                //Convert.ToInt64(DataSource.ExecuteScalar(FormatSql("SELECT COUNT(*) FROM {0} WHERE {1}=" + X, "DataTestA", "Int32")));
                //DataQuery.Select<M.DataTestA>(DataSource).Where(ps).Count();
                Db<M.DataTestA>.Query(DataSource).Select().Where(qps).Count();

                this["count4"] = DbTable.ExecuteCount<M.DataTestA>(DataSource, group, ps) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X group x by x.Int32).Count();
                //Convert.ToInt64(DataSource.ExecuteScalar(FormatSql("SELECT COUNT(*) FROM (SELECT {0} FROM {1} WHERE {0}=" + X + " GROUP BY {0}) AS T", "Int32", "DataTestA")));
                //DataQuery.Select<M.DataTestA>(DataSource).Where(ps).GroupBy(group).Count();
                Db<M.DataTestA>.Query(DataSource).Select().Where(qps).GroupBy(qgroup).Count();

                this["count5"] = DbTable.ExecuteCount<M.DataTestA, M.DataTestB>(DataSource, Id, Id, DataJoinType.Inner) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id select x.Id).Count();
                //Convert.ToInt64(DataSource.ExecuteScalar(FormatSql("SELECT COUNT(*) FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2}", "DataTestA", "DataTestB", "Id")));
                //DataQuery.Select<M.DataTestA>(DataSource).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Count();
                Db<M.DataTestA>.Query(DataSource).Select().InnerJoin(ca, cb).Count();

                this["count6"] = DbTable.ExecuteCount<M.DataTestA, M.DataTestB>(DataSource, tgroup, Id, Id, DataJoinType.Inner) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id group x by x.Int32).Count();
                //Convert.ToInt64(DataSource.ExecuteScalar(FormatSql("SELECT COUNT(*) FROM (SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} GROUP BY {0}.{3}) AS T", "DataTestA", "DataTestB", "Id", "Int32")));
                //DataQuery.Select<M.DataTestA>(DataSource).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).GroupBy(tgroup).Count();
                Db<M.DataTestA>.Query(DataSource).Select().InnerJoin(ca, cb).GroupBy(tqgroup).Count();

                this["count7"] = DbTable.ExecuteCount<M.DataTestA, M.DataTestB>(DataSource, Id, Id, DataJoinType.Inner, tps) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X select x.Id).Count();
                //Convert.ToInt64(DataSource.ExecuteScalar(FormatSql("SELECT COUNT(*) FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X, "DataTestA", "DataTestB", "Id", "Int32")));
                //DataQuery.Select<M.DataTestA>(DataSource).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).Count();
                Db<M.DataTestA>.Query(DataSource).Select().InnerJoin(ca, cb).Where(tqps).Count();

                this["count8"] = DbTable.ExecuteCount<M.DataTestA, M.DataTestB>(DataSource, tgroup, Id, Id, DataJoinType.Inner, tps) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X group x by x.Int32).Count();
                //Convert.ToInt64(DataSource.ExecuteScalar(FormatSql("SELECT COUNT(*) FROM (SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X + " GROUP BY {0}.{3}) AS T", "DataTestA", "DataTestB", "Id", "Int32")));
                //DataQuery.Select<M.DataTestA>(DataSource).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).GroupBy(tgroup).Count();
                Db<M.DataTestA>.Query(DataSource).Select().InnerJoin(ca, cb).Where(tqps).GroupBy(tqgroup).Count();


                this["scalar1"] = DbTable.ExecuteScalar<M.DataTestA, int>(DataSource, column) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) select x.Int32).First();
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0} FROM {1}", "Int32", "DataTestA")));
                //DataQuery.Select<M.DataTestA>(DataSource, column).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(qcolumn).Single<int>();

                this["scalar2"] = DbTable.ExecuteScalar<M.DataTestA, int>(DataSource, column, group) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) group x by x.Int32 into z select z.Int32).First();
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0} FROM {1} GROUP BY {0}", "Int32", "DataTestA")));
                //DataQuery.Select<M.DataTestA>(DataSource, column).GroupBy(group).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(qcolumn).GroupBy(qgroup).Single<int>();

                this["scalar3"] = DbTable.ExecuteScalar<M.DataTestA, int>(DataSource, column, orders) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) orderby x.Int32 descending select x.Int32).First();
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0} FROM {1} ORDER BY {0} DESC", "Int32", "DataTestA")));
                //DataQuery.Select<M.DataTestA>(DataSource, column).OrderBy(orders).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(qcolumn).OrderBy(qorders).Single<int>();

                this["scalar4"] = DbTable.ExecuteScalar<M.DataTestA, int>(DataSource, column, group, orders) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) orderby x.Int32 descending group x by x.Int32 into z select z.Int32).First();
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0} FROM {1} GROUP BY {0} ORDER BY {0} DESC", "Int32", "DataTestA")));
                //DataQuery.Select<M.DataTestA>(DataSource, column).GroupBy(group).OrderBy(orders).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(qcolumn).GroupBy(qgroup).OrderBy(qorders).Single<int>();

                this["scalar5"] = DbTable.ExecuteScalar<M.DataTestA, int>(DataSource, column, ps) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X select x.Int32).First();
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0} FROM {1} WHERE {0}=" + X, "Int32", "DataTestA")));
                //DataQuery.Select<M.DataTestA>(DataSource, column).Where(ps).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(qcolumn).Where(qps).Single<int>();

                this["scalar6"] = DbTable.ExecuteScalar<M.DataTestA, int>(DataSource, column, group, ps) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X group x by x.Int32 into z select z.Int32).First();
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0} FROM {1} WHERE {0}=" + X + " GROUP BY {0}", "Int32", "DataTestA")));
                //DataQuery.Select<M.DataTestA>(DataSource, column).Where(ps).GroupBy(group).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(qcolumn).Where(qps).GroupBy(qgroup).Single<int>();

                this["scalar7"] = DbTable.ExecuteScalar<M.DataTestA, int>(DataSource, column, orders, ps) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X orderby x.Int32 descending select x.Int32).First();
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0} FROM {1} WHERE {0}=" + X + " ORDER BY {0} DESC", "Int32", "DataTestA")));
                //DataQuery.Select<M.DataTestA>(DataSource, column).Where(ps).OrderBy(orders).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(qcolumn).Where(qps).OrderBy(qorders).Single<int>();

                this["scalar8"] = DbTable.ExecuteScalar<M.DataTestA, int>(DataSource, column, group, orders, ps) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X orderby x.Int32 descending group x by x.Int32 into z select z.Int32).First();
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0} FROM {1} WHERE {0}=" + X + " GROUP BY {0} ORDER BY {0} DESC", "Int32", "DataTestA")));
                //DataQuery.Select<M.DataTestA>(DataSource, column).Where(ps).GroupBy(group).OrderBy(orders).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(qcolumn).Where(qps).GroupBy(qgroup).OrderBy(qorders).Single<int>();

                this["scalar9"] = DbTable.ExecuteScalar<M.DataTestA, M.DataTestB, int>(DataSource, tcolumn, Id, Id, DataJoinType.Inner) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id select x.Int32).First();
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2}", "DataTestA", "DataTestB", "Id", "Int32")));
                //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).Single<int>();

                this["scalar10"] = DbTable.ExecuteScalar<M.DataTestA, M.DataTestB, int>(DataSource, tcolumn, tgroup, Id, Id, DataJoinType.Inner) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id group x by x.Int32).First().x.Int32;
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} GROUP BY {0}.{3}", "DataTestA", "DataTestB", "Id", "Int32")));
                //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).GroupBy(tgroup).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).GroupBy(tqgroup).Single<int>();

                this["scalar11"] = DbTable.ExecuteScalar<M.DataTestA, M.DataTestB, int>(DataSource, tcolumn, torders, Id, Id, DataJoinType.Inner) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id orderby x.Int32 descending select x.Int32).First();
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} ORDER BY {0}.{3} DESC", "DataTestA", "DataTestB", "Id", "Int32")));
                //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).OrderBy(torders).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).OrderBy(tqorders).Single<int>();

                this["scalar12"] = DbTable.ExecuteScalar<M.DataTestA, M.DataTestB, int>(DataSource, tcolumn, tgroup, torders, Id, Id, DataJoinType.Inner) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id orderby x.Int32 descending group x by x.Int32).First().x.Int32;
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} GROUP BY {0}.{3} ORDER BY {0}.{3} DESC", "DataTestA", "DataTestB", "Id", "Int32")));
                //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).GroupBy(tgroup).OrderBy(torders).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).GroupBy(tqgroup).OrderBy(tqorders).Single<int>();

                this["scalar13"] = DbTable.ExecuteScalar<M.DataTestA, M.DataTestB, int>(DataSource, tcolumn, Id, Id, DataJoinType.Inner, tps) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X select x.Int32).First();
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X, "DataTestA", "DataTestB", "Id", "Int32")));
                //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).Where(tqps).Single<int>();

                this["scalar14"] = DbTable.ExecuteScalar<M.DataTestA, M.DataTestB, int>(DataSource, tcolumn, tgroup, Id, Id, DataJoinType.Inner, tps) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X group x by x.Int32).First().x.Int32;
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X + " GROUP BY {0}.{3}", "DataTestA", "DataTestB", "Id", "Int32")));
                //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).GroupBy(tgroup).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).Where(tqps).GroupBy(tqgroup).Single<int>();

                this["scalar15"] = DbTable.ExecuteScalar<M.DataTestA, M.DataTestB, int>(DataSource, tcolumn, torders, Id, Id, DataJoinType.Inner, tps) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X orderby x.Int32 descending select x.Int32).First();
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X + " ORDER BY {0}.{3} DESC", "DataTestA", "DataTestB", "Id", "Int32")));
                //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).OrderBy(torders).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).Where(tqps).OrderBy(tqorders).Single<int>();

                this["scalar16"] = DbTable.ExecuteScalar<M.DataTestA, M.DataTestB, int>(DataSource, tcolumn, tgroup, torders, Id, Id, DataJoinType.Inner, tps) ==
                //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X orderby x.Int32 descending group x by x.Int32).First().x.Int32;
                //Convert.ToInt32(DataSource.ExecuteScalar(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X + " GROUP BY {0}.{3} ORDER BY {0}.{3} DESC", "DataTestA", "DataTestB", "Id", "Int32")));
                //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).GroupBy(tgroup).OrderBy(torders).Single<int>();
                Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).Where(tqps).GroupBy(tqgroup).OrderBy(tqorders).Single<int>();

                this["single1"] = R(DbTable.ExecuteSingleRow<M.DataTestA>(DataSource),
                    //(from x in Query<M.DataTestA>.Table(DataSource) select x).First()
                    //DataSource.ExecuteSingleRow<M.DataTestA>(FormatSql("SELECT * FROM {0}", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource).First<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select().First<M.DataTestA>()
                    );

                this["single2"] = R(DbTable.ExecuteSingleRow<M.DataTestA>(DataSource, columns, group),
                    //(from x in Query<M.DataTestA>.Table(DataSource) group x by x.Int32).First()
                    //DataSource.ExecuteSingleRow<M.DataTestA>(FormatSql("SELECT {0} FROM {1} GROUP BY {0}", "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource, columns).GroupBy(group).First<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select(qcolumn).GroupBy(qgroup).First<M.DataTestA>()
                    );

                this["single3"] = R(DbTable.ExecuteSingleRow<M.DataTestA>(DataSource, ps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X select x).First()
                    //DataSource.ExecuteSingleRow<M.DataTestA>(FormatSql("SELECT * FROM {0} WHERE {1}=" + X, "DataTestA", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource).Where(ps).First<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select().Where(qps).First<M.DataTestA>()
                    );

                this["single4"] = R(DbTable.ExecuteSingleRow<M.DataTestA>(DataSource, columns, group, ps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X group x by x.Int32).First()
                    //DataSource.ExecuteSingleRow<M.DataTestA>(FormatSql("SELECT {0} FROM {1} WHERE {0}=" + X + " GROUP BY {0}", "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource, columns).Where(ps).GroupBy(group).First<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select(qcolumn).Where(qps).GroupBy(qgroup).First<M.DataTestA>()
                    );
                this["single5"] = R(DbTable.ExecuteSingleRow<M.DataTestA>(DataSource, orders),
                    //(from x in Query<M.DataTestA>.Table(DataSource) orderby x.Int32 descending select x).First()
                    //DataSource.ExecuteSingleRow<M.DataTestA>(FormatSql("SELECT * FROM {1} ORDER BY {0} DESC", "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource).OrderBy(orders).First<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select().OrderBy(qorders).First<M.DataTestA>()
                    );

                this["single6"] = R(DbTable.ExecuteSingleRow<M.DataTestA>(DataSource, columns, group, orders),
                    //(from x in Query<M.DataTestA>.Table(DataSource) orderby x.Int32 descending group x by x.Int32).First()
                    //DataSource.ExecuteSingleRow<M.DataTestA>(FormatSql("SELECT {0} FROM {1} GROUP BY {0} ORDER BY {0} DESC", "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource, columns).GroupBy(group).OrderBy(orders).First<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select(qcolumn).GroupBy(qgroup).OrderBy(qorders).First<M.DataTestA>()
                    );

                this["single7"] = R(DbTable.ExecuteSingleRow<M.DataTestA>(DataSource, orders, ps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X orderby x.Int32 descending select x).First()
                    //DataSource.ExecuteSingleRow<M.DataTestA>(FormatSql("SELECT * FROM {1} WHERE {0}=" + X + " ORDER BY {0} DESC", "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource).Where(ps).OrderBy(orders).First<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select().Where(qps).OrderBy(qorders).First<M.DataTestA>()
                    );

                this["single8"] = R(DbTable.ExecuteSingleRow<M.DataTestA>(DataSource, columns, group, orders, ps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X orderby x.Int32 descending group x by x.Int32).First()
                    //DataSource.ExecuteSingleRow<M.DataTestA>(FormatSql("SELECT {0} FROM {1} WHERE {0}=" + X + " GROUP BY {0} ORDER BY {0} DESC", "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource, columns).Where(ps).GroupBy(group).OrderBy(orders).First<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select(qcolumn).Where(qps).GroupBy(qgroup).OrderBy(qorders).First<M.DataTestA>()
                    );

                this["single9"] = R(DbTable.ExecuteSingleRow<M.DataTestA, M.DataTestB>(DataSource, tcolumns, Id, Id, DataJoinType.Inner),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id select new DataJoin<M.DataTestA, M.DataTestB>(new M.DataTestA { Int32 = x.Int32 }, new M.DataTestB())).First()
                    //DataSource.ExecuteSingleRow<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2}", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    );

                this["single10"] = R(DbTable.ExecuteSingleRow<M.DataTestA, M.DataTestB>(DataSource, tcolumns, tgroup, Id, Id, DataJoinType.Inner),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id group x by x.Int32 into z select new DataJoin<M.DataTestA, M.DataTestB>(z.x, new M.DataTestB())).First()
                    //DataSource.ExecuteSingleRow<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} GROUP BY {0}.{3}", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).GroupBy(tgroup).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).GroupBy(tqgroup).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    );

                this["single11"] = R(DbTable.ExecuteSingleRow<M.DataTestA, M.DataTestB>(DataSource, tcolumns, Id, Id, DataJoinType.Inner, tps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X select new DataJoin<M.DataTestA, M.DataTestB>(new M.DataTestA { Int32 = x.Int32 }, new M.DataTestB())).First()
                    //DataSource.ExecuteSingleRow<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X, "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).Where(tqps).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    );
                this["single12"] = R(DbTable.ExecuteSingleRow<M.DataTestA, M.DataTestB>(DataSource, tcolumns, tgroup, Id, Id, DataJoinType.Inner, tps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X group x by x.Int32 into z select new DataJoin<M.DataTestA, M.DataTestB>(z.x, new M.DataTestB())).First()
                    //DataSource.ExecuteSingleRow<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X + " GROUP BY {0}.{3}", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).GroupBy(tgroup).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).Where(tqps).GroupBy(tqgroup).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    );

                this["single13"] = R(DbTable.ExecuteSingleRow<M.DataTestA, M.DataTestB>(DataSource, tcolumns, torders, Id, Id, DataJoinType.Inner),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id orderby x.Int32 descending select new DataJoin<M.DataTestA, M.DataTestB>(new M.DataTestA { Int32 = x.Int32 }, new M.DataTestB())).First()
                    //DataSource.ExecuteSingleRow<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} ORDER BY {0}.{3} DESC", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).OrderBy(torders).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).OrderBy(tqorders).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    );

                this["single14"] = R(DbTable.ExecuteSingleRow<M.DataTestA, M.DataTestB>(DataSource, tcolumns, tgroup, torders, Id, Id, DataJoinType.Inner),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id orderby x.Int32 descending group x by x.Int32 into z select new DataJoin<M.DataTestA, M.DataTestB>(z.x, new M.DataTestB())).First()
                    //DataSource.ExecuteSingleRow<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} GROUP BY {0}.{3} ORDER BY {0}.{3} DESC", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).GroupBy(tgroup).OrderBy(torders).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).GroupBy(tqgroup).OrderBy(tqorders).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    );

                this["single15"] = R(DbTable.ExecuteSingleRow<M.DataTestA, M.DataTestB>(DataSource, tcolumns, torders, Id, Id, DataJoinType.Inner, tps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X orderby x.Int32 descending select new DataJoin<M.DataTestA, M.DataTestB>(new M.DataTestA { Int32 = x.Int32 }, new M.DataTestB())).First()
                    //DataSource.ExecuteSingleRow<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X + " ORDER BY {0}.{3} DESC", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).OrderBy(torders).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).Where(tqps).OrderBy(tqorders).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    );

                this["single16"] = R(DbTable.ExecuteSingleRow<M.DataTestA, M.DataTestB>(DataSource, tcolumns, tgroup, torders, Id, Id, DataJoinType.Inner, tps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X orderby x.Int32 descending group x by x.Int32 into z select new DataJoin<M.DataTestA, M.DataTestB>(z.x, new M.DataTestB())).First()
                    //DataSource.ExecuteSingleRow<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X + " GROUP BY {0}.{3} ORDER BY {0}.{3} DESC", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).GroupBy(tgroup).OrderBy(torders).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).Where(tqps).GroupBy(tqgroup).OrderBy(tqorders).First<DataJoin<M.DataTestA, M.DataTestB>>()
                    );

                this["Content"] = @"
<p>count1=$count1</p>
<p>count2=$count2</p>
<p>count3=$count3</p>
<p>count4=$count4</p>
<p>count5=$count5</p>
<p>count6=$count6</p>
<p>count7=$count7</p>
<p>count8=$count8</p>

<p>scalar1= $scalar1 </p>
<p>scalar2= $scalar2 </p>
<p>scalar3= $scalar3 </p>
<p>scalar4= $scalar4 </p>
<p>scalar5= $scalar5 </p>
<p>scalar6= $scalar6 </p>
<p>scalar7= $scalar7 </p>
<p>scalar8= $scalar8 </p>
<p>scalar9= $scalar9 </p>
<p>scalar10=$scalar10</p>
<p>scalar11=$scalar11</p>
<p>scalar12=$scalar12</p>
<p>scalar13=$scalar13</p>
<p>scalar14=$scalar14</p>
<p>scalar15=$scalar15</p>
<p>scalar16=$scalar16</p>

<p>single1= $single1 </p>
<p>single2= $single2 </p>
<p>single3= $single3 </p>
<p>single4= $single4 </p>
<p>single5= $single5 </p>
<p>single6= $single6 </p>
<p>single7= $single7 </p>
<p>single8= $single8 </p>
<p>single9= $single9 </p>
<p>single10=$single10</p>
<p>single11=$single11</p>
<p>single12=$single12</p>
<p>single13=$single13</p>
<p>single14=$single14</p>
<p>single15=$single15</p>
<p>single16=$single16</p>
";
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                this["Content"] = ex.Message + ex.StackTrace;
            }
            RenderComm();
        }
        public void ExecReader()
        {
            try
            {
                InitId();

                this["reader1"] = R(DbTable.ExecuteReader<M.DataTestA>(DataSource),
                    //(from x in Query<M.DataTestA>.Table(DataSource) select x).ToList()
                    //DataSource.ExecuteReader<M.DataTestA>(FormatSql("SELECT * FROM {0}", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource).ToList<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select().ToList<M.DataTestA>()
                    );
                this["reader2"] = R(DbTable.ExecuteReader<M.DataTestA>(DataSource, columns, group),
                    //(from x in Query<M.DataTestA>.Table(DataSource) group x by x.Int32).ToList()
                    //DataSource.ExecuteReader<M.DataTestA>(FormatSql("SELECT {0} FROM {1} GROUP BY {0}", "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource, columns).GroupBy(group).ToList<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select(qcolumn).GroupBy(qgroup).ToList<M.DataTestA>()
                    );
                this["reader3"] = R(DbTable.ExecuteReader<M.DataTestA>(DataSource, ps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X select x).ToList()
                    //DataSource.ExecuteReader<M.DataTestA>(FormatSql("SELECT * FROM {1} WHERE {0}=" + X, "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource).Where(ps).ToList<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select().Where(qps).ToList<M.DataTestA>()
                    );
                this["reader4"] = R(DbTable.ExecuteReader<M.DataTestA>(DataSource, columns, group, ps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X group x by x.Int32).ToList()
                    //DataSource.ExecuteReader<M.DataTestA>(FormatSql("SELECT {0} FROM {1} WHERE {0}=" + X + " GROUP BY {0}", "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource, columns).Where(ps).GroupBy(group).ToList<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select(qcolumn).Where(qps).GroupBy(qgroup).ToList<M.DataTestA>()
                    );
                this["reader5"] = R(DbTable.ExecuteReader<M.DataTestA>(DataSource, orders),
                    //(from x in Query<M.DataTestA>.Table(DataSource) orderby x.Int32 descending select x).ToList()
                    //DataSource.ExecuteReader<M.DataTestA>(FormatSql("SELECT * FROM {1} ORDER BY {0} DESC", "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource).OrderBy(orders).ToList<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select().OrderBy(qorders).ToList<M.DataTestA>()
                    );
                this["reader6"] = R(DbTable.ExecuteReader<M.DataTestA>(DataSource, columns, group, orders),
                    //(from x in Query<M.DataTestA>.Table(DataSource) orderby x.Int32 descending group x by x.Int32).ToList()
                    //DataSource.ExecuteReader<M.DataTestA>(FormatSql("SELECT {0} FROM {1} GROUP BY {0} ORDER BY {0} DESC", "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource, columns).GroupBy(group).OrderBy(orders).ToList<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select(qcolumn).GroupBy(qgroup).OrderBy(qorders).ToList<M.DataTestA>()
                    );
                this["reader7"] = R(DbTable.ExecuteReader<M.DataTestA>(DataSource, orders, ps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X orderby x.Int32 descending select x).ToList()
                    //DataSource.ExecuteReader<M.DataTestA>(FormatSql("SELECT * FROM {1} WHERE {0}=" + X + " ORDER BY {0} DESC", "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource).Where(ps).OrderBy(orders).ToList<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select().Where(qps).OrderBy(qorders).ToList<M.DataTestA>()
                    );
                this["reader8"] = R(DbTable.ExecuteReader<M.DataTestA>(DataSource, columns, group, orders, ps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X orderby x.Int32 descending group x by x.Int32).ToList()
                    //DataSource.ExecuteReader<M.DataTestA>(FormatSql("SELECT {0} FROM {1} WHERE {0}=" + X + " GROUP BY {0} ORDER BY {0} DESC", "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource, columns).Where(ps).GroupBy(group).OrderBy(orders).ToList<M.DataTestA>()
                    Db<M.DataTestA>.Query(DataSource).Select(qcolumn).Where(qps).GroupBy(qgroup).OrderBy(qorders).ToList<M.DataTestA>()
                    );
                this["reader9"] = R(DbTable.ExecuteReader<M.DataTestA, M.DataTestB>(DataSource, tcolumns, Id, Id, DataJoinType.Inner),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id select new DataJoin<M.DataTestA, M.DataTestB>(new M.DataTestA { Int32 = x.Int32 }, new M.DataTestB())).ToList()
                    //DataSource.ExecuteReader<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2}", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    );
                this["reader10"] = R(DbTable.ExecuteReader<M.DataTestA, M.DataTestB>(DataSource, tcolumns, tgroup, Id, Id, DataJoinType.Inner),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id group x by x.Int32 into z select new DataJoin<M.DataTestA, M.DataTestB>(z.x, new M.DataTestB())).ToList()
                    //DataSource.ExecuteReader<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} GROUP BY {0}.{3}", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).GroupBy(tgroup).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).GroupBy(tqgroup).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    );
                this["reader11"] = R(DbTable.ExecuteReader<M.DataTestA, M.DataTestB>(DataSource, tcolumns, Id, Id, DataJoinType.Inner, tps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X select new DataJoin<M.DataTestA, M.DataTestB>(new M.DataTestA { Int32 = x.Int32 }, new M.DataTestB())).ToList()
                    //DataSource.ExecuteReader<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X, "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).Where(tqps).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    );
                this["reader12"] = R(DbTable.ExecuteReader<M.DataTestA, M.DataTestB>(DataSource, tcolumns, tgroup, Id, Id, DataJoinType.Inner, tps),
                    //DataSource.ExecuteReader<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X + " GROUP BY {0}.{3}", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).GroupBy(tgroup).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).Where(tqps).GroupBy(tqgroup).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    );
                this["reader13"] = R(DbTable.ExecuteReader<M.DataTestA, M.DataTestB>(DataSource, tcolumns, torders, Id, Id, DataJoinType.Inner),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id orderby x.Int32 descending select new DataJoin<M.DataTestA, M.DataTestB>(new M.DataTestA { Int32 = x.Int32 }, new M.DataTestB())).ToList()
                    //DataSource.ExecuteReader<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} ORDER BY {0}.{3} DESC", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).OrderBy(torders).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).OrderBy(tqorders).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    );
                this["reader14"] = R(DbTable.ExecuteReader<M.DataTestA, M.DataTestB>(DataSource, tcolumns, tgroup, torders, Id, Id, DataJoinType.Inner),
                    //DataSource.ExecuteReader<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} GROUP BY {0}.{3} ORDER BY {0}.{3} DESC", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).GroupBy(tgroup).OrderBy(torders).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).GroupBy(tqgroup).OrderBy(tqorders).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    );
                this["reader15"] = R(DbTable.ExecuteReader<M.DataTestA, M.DataTestB>(DataSource, tcolumns, torders, Id, Id, DataJoinType.Inner, tps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X orderby x.Int32 descending select new DataJoin<M.DataTestA, M.DataTestB>(new M.DataTestA { Int32 = x.Int32 }, new M.DataTestB())).ToList()
                    //DataSource.ExecuteReader<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X + " ORDER BY {0}.{3} DESC", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).OrderBy(torders).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).Where(tqps).OrderBy(tqorders).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    );
                this["reader16"] = R(DbTable.ExecuteReader<M.DataTestA, M.DataTestB>(DataSource, tcolumns, tgroup, torders, Id, Id, DataJoinType.Inner, tps),
                    //DataSource.ExecuteReader<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X + " GROUP BY {0}.{3} ORDER BY {0}.{3} DESC", "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumn).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).GroupBy(tgroup).OrderBy(torders).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumn).InnerJoin(ca, cb).Where(tqps).GroupBy(tqgroup).OrderBy(tqorders).ToList<DataJoin<M.DataTestA, M.DataTestB>>()
                    );

                this["Content"] = @"
<p>reader1= $reader1 </p>
<p>reader2= $reader2 </p>
<p>reader3= $reader3 </p>
<p>reader4= $reader4 </p>
<p>reader5= $reader5 </p>
<p>reader6= $reader6 </p>
<p>reader7= $reader7 </p>
<p>reader8= $reader8 </p>
<p>reader9= $reader9 </p>
<p>reader10=$reader10</p>
<p>reader11=$reader11</p>
<p>reader12=$reader12</p>
<p>reader13=$reader13</p>
<p>reader14=$reader14</p>
<p>reader15=$reader15</p>
<p>reader16=$reader16</p>
";
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                this["Content"] = ex.Message + ex.StackTrace;
            }
            RenderComm();
        }
        public void ExecPage()
        {
            try
            {
                long cout;
                long index = 1;
                int size = 10;

                InitId();

                this["page1"] = R(DbTable.ExecuteReader<M.DataTestA>(DataSource, orders, index, size, out cout),
                    //(from x in Query<M.DataTestA>.Table(DataSource) orderby x.Int32 descending select x).ToPage(index, size, out cout)
                    //DataSource.ExecuteReader<M.DataTestA>(FormatSql("SELECT {2} * FROM {1} ORDER BY {0} DESC {3}", 10, "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource).OrderBy(orders).Limit(size, index).ToList<M.DataTestA>(out cout)
                    Db<M.DataTestA>.Query(DataSource).Select().OrderBy(qorders).ToList<M.DataTestA>(size, index, out cout)
                    );
                this["page2"] = R(DbTable.ExecuteReader<M.DataTestA>(DataSource, columns, group, orders, index, size, out cout),
                    //(from x in Query<M.DataTestA>.Table(DataSource) orderby x.Int32 descending group x by x.Int32).ToPage(index, size, out cout)
                    //DataSource.ExecuteReader<M.DataTestA>(FormatSql("SELECT {2} {0} FROM {1} GROUP BY {0} ORDER BY {0} DESC {3}", 10, "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource, columns).GroupBy(group).OrderBy(orders).Limit(size, index).ToList<M.DataTestA>(out cout)
                    Db<M.DataTestA>.Query(DataSource).Select(qcolumns).GroupBy(qgroup).OrderBy(qorders).ToList<M.DataTestA>(size, index, out cout)
                    );
                this["page3"] = R(DbTable.ExecuteReader<M.DataTestA>(DataSource, orders, index, size, out cout, ps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X orderby x.Int32 descending select x).ToPage(index, size, out cout)
                    //DataSource.ExecuteReader<M.DataTestA>(FormatSql("SELECT {2} * FROM {1} WHERE {0}=" + X + " ORDER BY {0} DESC {3}", 10, "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource).Where(ps).OrderBy(orders).Limit(size, index).ToList<M.DataTestA>(out cout)
                    Db<M.DataTestA>.Query(DataSource).Select().Where(qps).OrderBy(qorders).ToList<M.DataTestA>(size, index, out cout)
                    );
                this["page4"] = R(DbTable.ExecuteReader<M.DataTestA>(DataSource, columns, group, orders, index, size, out cout, ps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X orderby x.Int32 descending group x by x.Int32).ToPage(index, size, out cout)
                    //DataSource.ExecuteReader<M.DataTestA>(FormatSql("SELECT {2} {0} FROM {1} WHERE {0}=" + X + " GROUP BY {0} ORDER BY {0} DESC {3}", 10, "Int32", "DataTestA"))
                    //DataQuery.Select<M.DataTestA>(DataSource, columns).Where(ps).GroupBy(group).OrderBy(orders).Limit(size, index).ToList<M.DataTestA>(out cout)
                    Db<M.DataTestA>.Query(DataSource).Select(qcolumns).Where(qps).GroupBy(qgroup).OrderBy(qorders).ToList<M.DataTestA>(size, index, out cout)
                    );
                this["page5"] = R(DbTable.ExecuteReader<M.DataTestA, M.DataTestB>(DataSource, tcolumns, torders, index, size, out cout, Id, Id, DataJoinType.Inner),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id orderby x.Int32 descending select new DataJoin<M.DataTestA, M.DataTestB>(new M.DataTestA { Int32 = x.Int32 }, new M.DataTestB())).ToPage(index, size, out cout)
                    //DataSource.ExecuteReader<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {4} {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} ORDER BY {0}.{3} DESC {5}", 10, "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumns).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).OrderBy(torders).Limit(size, index).ToList<DataJoin<M.DataTestA, M.DataTestB>>(out cout)
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumns).InnerJoin(ca, cb).OrderBy(tqorders).ToList<DataJoin<M.DataTestA, M.DataTestB>>(size, index, out cout)
                    );
                this["page6"] = R(DbTable.ExecuteReader<M.DataTestA, M.DataTestB>(DataSource, tcolumns, tgroup, torders, index, size, out cout, Id, Id, DataJoinType.Inner),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id orderby x.Int32 descending group x by x.Int32 into z select new DataJoin<M.DataTestA, M.DataTestB>(z.x, new M.DataTestB())).ToPage(index, size, out cout)
                    //DataSource.ExecuteReader<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {4} {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} GROUP BY {0}.{3} ORDER BY {0}.{3} DESC {5}", 10, "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumns).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).GroupBy(tgroup).OrderBy(torders).Limit(size, index).ToList<DataJoin<M.DataTestA, M.DataTestB>>(out cout)
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumns).InnerJoin(ca, cb).GroupBy(tqgroup).OrderBy(tqorders).ToList<DataJoin<M.DataTestA, M.DataTestB>>(size, index, out cout)
                    );
                this["page7"] = R(DbTable.ExecuteReader<M.DataTestA, M.DataTestB>(DataSource, tcolumns, torders, index, size, out cout, Id, Id, DataJoinType.Inner, tps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X orderby x.Int32 descending select new DataJoin<M.DataTestA, M.DataTestB>(new M.DataTestA { Int32 = x.Int32 }, new M.DataTestB())).ToPage(index, size, out cout)
                    //DataSource.ExecuteReader<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {4} {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X + " ORDER BY {0}.{3} DESC {5}", 10, "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumns).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).OrderBy(torders).Limit(size, index).ToList<DataJoin<M.DataTestA, M.DataTestB>>(out cout)
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumns).InnerJoin(ca, cb).Where(tqps).OrderBy(tqorders).ToList<DataJoin<M.DataTestA, M.DataTestB>>(size, index, out cout)
                    );
                this["page8"] = R(DbTable.ExecuteReader<M.DataTestA, M.DataTestB>(DataSource, tcolumns, tgroup, torders, index, size, out cout, Id, Id, DataJoinType.Inner, tps),
                    //(from x in Query<M.DataTestA>.Table(DataSource) join y in Query<M.DataTestB>.Table(DataSource) on x.Id equals y.Id where x.Int32 == X orderby x.Int32 descending group x by x.Int32 into z select new DataJoin<M.DataTestA, M.DataTestB>(z.x, new M.DataTestB())).ToPage(index, size, out cout)
                    //DataSource.ExecuteReader<DataJoin<M.DataTestA, M.DataTestB>>(FormatSql("SELECT {4} {0}.{3} FROM {0} INNER JOIN {1} ON {0}.{2}={1}.{2} WHERE {0}.{3}=" + X + " GROUP BY {0}.{3} ORDER BY {0}.{3} DESC {5}", 10, "DataTestA", "DataTestB", "Id", "Int32"))
                    //DataQuery.Select<M.DataTestA>(DataSource, tcolumns).Join<M.DataTestA>(Id).On<M.DataTestB>(Id).Where(tps).GroupBy(tgroup).OrderBy(torders).Limit(size, index).ToList<DataJoin<M.DataTestA, M.DataTestB>>(out cout)
                    Db<M.DataTestA>.Query(DataSource).Select(tqcolumns).InnerJoin(ca, cb).Where(tqps).GroupBy(tqgroup).OrderBy(tqorders).ToList<DataJoin<M.DataTestA, M.DataTestB>>(size, index, out cout)
                    );

                this["Content"] = @"
<p>page1= $page1 </p>
<p>page2= $page2 </p>
<p>page3= $page3 </p>
<p>page4= $page4 </p>
<p>page5= $page5 </p>
<p>page6= $page6 </p>
<p>page7= $page7 </p>
<p>page8= $page8 </p>
";
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                this["Content"] = ex.Message + ex.StackTrace;
            }
            RenderComm();
        }

        public void ExecTime()
        {
            try
            {
                long cout;
                long index = 1;
                int size = 10;

                InitId();

                DateTime begin;
                DateTime end;

                begin = DateTime.Now;
                for (int i = 0; i < 1000; ++i)
                    DbTable.ExecuteCount<M.DataTestA>(DataSource, ps);
                end = DateTime.Now;
                this["time1"] = (end - begin).TotalMilliseconds;
                begin = DateTime.Now;
                for (int i = 0; i < 1000; ++i)
                    Db<M.DataTestA>.Query(DataSource).Select().Where(qps).Count();
                //DataQuery.Select<M.DataTestA>(DataSource).Where(ps).Count();
                //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X select x).Count();
                end = DateTime.Now;
                this["time2"] = (end - begin).TotalMilliseconds;

                begin = DateTime.Now;
                for (int i = 0; i < 1000; ++i)
                    DbTable.ExecuteScalar<M.DataTestA, int>(DataSource, column, ps);
                end = DateTime.Now;
                this["time3"] = (end - begin).TotalMilliseconds;
                begin = DateTime.Now;
                for (int i = 0; i < 1000; ++i)
                    Db<M.DataTestA>.Query(DataSource).Select(qcolumn).Where(qps).Single<int>();
                //DataQuery.Select<M.DataTestA>(DataSource, column).Where(ps).Single<int>();
                //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X select x.Int32).First();
                end = DateTime.Now;
                this["time4"] = (end - begin).TotalMilliseconds;

                begin = DateTime.Now;
                for (int i = 0; i < 1000; ++i)
                    DbTable.ExecuteSingleRow<M.DataTestA>(DataSource, ps);
                end = DateTime.Now;
                this["time5"] = (end - begin).TotalMilliseconds;
                begin = DateTime.Now;
                for (int i = 0; i < 1000; ++i)
                    Db<M.DataTestA>.Query(DataSource).Select().Where(qps).First();
                //DataQuery.Select<M.DataTestA>(DataSource).Where(ps).First();
                //DataQuery.Select<M.DataTestA>(DataSource).Where(ps).First<M.DataTestA>();
                //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X select x).First();
                end = DateTime.Now;
                this["time6"] = (end - begin).TotalMilliseconds;

                begin = DateTime.Now;
                for (int i = 0; i < 1000; ++i)
                    DbTable.ExecuteReader<M.DataTestA>(DataSource, 50, ps);
                end = DateTime.Now;
                this["time7"] = (end - begin).TotalMilliseconds;
                begin = DateTime.Now;
                for (int i = 0; i < 1000; ++i)
                    Db<M.DataTestA>.Query(DataSource).Select().Where(qps).ToList(50);
                //DataQuery.Select<M.DataTestA>(DataSource).Where(ps).Limit(50).ToList();
                //DataQuery.Select<M.DataTestA>(DataSource).Where(ps).Limit(50).ToList<M.DataTestA>();
                //(from x in Query<M.DataTestA>.Table(DataSource) where x.Int32 == X select x).Take(50);
                end = DateTime.Now;
                this["time8"] = (end - begin).TotalMilliseconds;

                this["Content"] = @"
<p>time1= $time1 </p>
<p>time2= $time2 </p>
<p>time3= $time3 </p>
<p>time4= $time4 </p>
<p>time5= $time5 </p>
<p>time6= $time6 </p>
<p>time7= $time7 </p>
<p>time8= $time8 </p>
";
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                this["Content"] = ex.Message + ex.StackTrace;
            }
            RenderComm();
        }

        public void Insert()
        {
            try
            {
                long id;
                M.DataTestA value = new M.DataTestA()
                {
                    Guid = Guid.NewGuid(),
                    String = "String",
                    Boolean = true,
                    Int32 = 1,
                    Int64 = 2L,
                    Double = 3.3,
                    DateTime = DateTime.Now,
                    Money = 4
                };
                this["page1"] = string.Concat(Db<M.DataTestA>.Query(DataSource)
                    .Insert(value), ':', value.Id);
                this["page2"] = string.Concat(Db<M.DataTestA>.Query(DataSource)
                    .Insert("Guid", "String", "Boolean", "Int32", "Int64", "Double", "DateTime", "Money")
                    .Values(Guid.NewGuid(), "String", true, 1, 2L, 3.3, DateTime.Now, 4)
                    .Execute(), ':', 0);
                this["page3"] = string.Concat(Db<M.DataTestA>.Query(DataSource)
                    .Insert("Guid", "String", "Boolean", "Int32", "Int64", "Double", "DateTime", "Money")
                    .Values(Guid.NewGuid(), "String", true, 1, 2L, 3.3, DateTime.Now, 4)
                    .Execute("Id", out id), ':', id);
                this["page4"] = string.Concat(Db<M.DataTestA>.Query(DataSource)
                    .Insert()
                    .Select<M.DataTestA>("Guid", "String", "Boolean", "Int32", "Int64", "Double", "DateTime", "Money").Where(new DbWhere("Id", id)).Result()
                    .Execute(), ':', 0);
                this["page5"] = string.Concat(Db<M.DataTestA>.Query(DataSource)
                    .Insert()
                    .Select<M.DataTestA>("Guid", "String", "Boolean", "Int32", "Int64", "Double", "DateTime", "Money").Where(new DbWhere("Id", id)).Result()
                    .Execute("Id", out id), ':', id);
                this["page6"] = string.Concat(Db<M.DataTestA>.Query(DataSource)
                    .Insert("Guid", "String", "Boolean", "Int32", "Int64", "Double", "DateTime", "Money")
                    .Select<M.DataTestA>("Guid", "String", "Boolean", "Int32", "Int64", "Double", "DateTime", "Money").Where(new DbWhere("Id", id)).Result()
                    .Execute(), ':', 0);
                this["page7"] = string.Concat(Db<M.DataTestA>.Query(DataSource)
                    .Insert("Guid", "String", "Boolean", "Int32", "Int64", "Double", "DateTime", "Money")
                    .Select<M.DataTestA>("Guid", "String", "Boolean", "Int32", "Int64", "Double", "DateTime", "Money").Where(new DbWhere("Id", id)).Result()
                    .Execute("Id", out id), ':', id);

                this["Content"] = @"
<p>page1= $page1 </p>
<p>page2= $page2 </p>
<p>page3= $page3 </p>
<p>page4= $page4 </p>
<p>page5= $page5 </p>
<p>page6= $page6 </p>
<p>page7= $page7 </p>
";
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                this["Content"] = ex.Message + ex.StackTrace;
            }
            RenderComm();
        }
        public void Update()
        {
            try
            {
                M.DataTestA value = new M.DataTestA()
                {
                    Id = 1,
                    Guid = Guid.NewGuid(),
                    String = "String",
                    Boolean = true,
                    Int32 = 1,
                    Int64 = 2L,
                    Double = 3.3,
                    DateTime = DateTime.Now,
                    Money = 4
                };
                this["page1"] = Db<M.DataTestA>.Query(DataSource)
                    .Update(value);
                this["page2"] = Db<M.DataTestA>.Query(DataSource)
                    .Update(value, "Guid", "DateTime");
                this["page3"] = Db<M.DataTestA>.Query(DataSource)
                    .Update(value, ColumnMode.Exclude, "Int32", "Int64");
                this["page4"] = Db<M.DataTestA>.Query(DataSource)
                    .Update()
                    .Set("Int32", 1)
                    .Set("Double").Select<M.DataTestA>("Double").Where(new DbWhere("Id", 1)).Result()
                    .SetAdd("Int64", 1L)
                    .Where(new DbWhere("Id", 2))
                    .Execute();

                this["Content"] = @"
<p>page1= $page1 </p>
<p>page2= $page2 </p>
<p>page3= $page3 </p>
<p>page4= $page4 </p>
";
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                this["Content"] = ex.Message + ex.StackTrace;
            }
            RenderComm();
        }
        public void Delete()
        {
            try
            {
                long id;
                M.DataTestA value = new M.DataTestA()
                {
                    Id = 0,
                    Guid = Guid.NewGuid(),
                    String = "String",
                    Boolean = true,
                    Int32 = 1,
                    Int64 = 2L,
                    Double = 3.3,
                    DateTime = DateTime.Now,
                    Money = 4
                };
                this["page1"] = Db<M.DataTestA>.Query(DataSource)
                    .Delete(value);
                this["page2"] = Db<M.DataTestA>.Query(DataSource)
                    .Delete()
                    .Where(new DbWhere("Id", 0))
                    .Execute();

                this["Content"] = @"
<p>page1= $page1 </p>
<p>page2= $page2 </p>
";
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                this["Content"] = ex.Message + ex.StackTrace;
            }
            RenderComm();
        }

        private bool R(M.DataTestA m, M.DataTestA n)
        {
            return m == n;
        }
        private bool R(IList<M.DataTestA> m, IList<M.DataTestA> n)
        {
            if (m == null || n == null)
                return false;
            if (m.Count != n.Count)
                return false;
            for (int i = 0; i < m.Count; ++i)
            {
                if (m[i] != n[i])
                    return false;
            }
            return true;
        }
        private bool R(DataJoin<M.DataTestA, M.DataTestB> m, DataJoin<M.DataTestA, M.DataTestB> n)
        {
            if (m == null || n == null)
                return false;
            return m.A == n.A;
        }
        private bool R(IList<DataJoin<M.DataTestA, M.DataTestB>> m, IList<DataJoin<M.DataTestA, M.DataTestB>> n)
        {
            if (m == null || n == null)
                return false;
            if (m.Count != n.Count)
                return false;
            for (int i = 0; i < m.Count; ++i)
            {
                if (m[i].A != n[i].A)
                    return false;
            }
            return true;
        }
        private void RenderComm()
        {
            Render(@"<html>
<head>
<meta charset=""utf-8""/>
<style type=""text/css"">
*{margin:0;border:0;padding:0;}
p{height:14px;line-height:14px;font-size:12px;}
a{font-size:14px;}
</style>
</head>
<body>
<div><a href=""$url('/datatest')"">返回</a></div>
<div>" + this["Content"] + @"</div>
</body>
</html>", "index.html");
        }
    }
#endif
}
