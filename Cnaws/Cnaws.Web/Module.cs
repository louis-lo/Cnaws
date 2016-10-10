using System;
using Cnaws.Data;
using Cnaws.Data.Query;
using System.ComponentModel;
using System.Reflection;
using Cnaws.Templates;

namespace Cnaws.Web
{
    public enum DataStatus
    {
        Exist = -1,
        ExistOther = -2,
        Rollback = -3,
        Success = -200,
        Failed = -500
    }

    [Serializable]
    public abstract class Module : DbTable
    {
        internal static void Install<T>(DataSource ds) where T : Module, new()
        {
            try { DropTable<T>(ds); }
            catch (Exception) { }
            T ins = new T();
            if (ins.CanInstall)
            {
                ins.OnInstallBefor(ds);
                CreateTable<T>(ds);
                ins.OnInstallAfter(ds);

                //string name;
                //DataFunctionAttribute att;
                //foreach (MethodInfo m in TType<T>.Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod))
                //{
                //    att = Attribute.GetCustomAttribute(m, TType<T>.Type) as DataFunctionAttribute;
                //    if (att != null)
                //    {
                //        name = m.Name;
                //        if (!string.IsNullOrEmpty(att.Name))
                //            name = att.Name;

                //    }
                //}
            }
        }
        internal static void InstallInit<T>(DataSource ds) where T : Module, new()
        {
            T ins = new T();
            ins.OnInstallInit(ds);
        }
        internal static void Upgrade<T>(DataSource ds, Version ver) where T : Module, new()
        {
            T ins = new T();
            ins.OnUpgrade(ds, ver);
        }

        public virtual bool CanInstall
        {
            get { return true; }
        }
        protected virtual void OnInstallBefor(DataSource ds)
        {
        }
        protected virtual void OnInstallAfter(DataSource ds)
        {
        }
        protected virtual void OnInstallInit(DataSource ds)
        {
        }
        protected virtual void OnUpgrade(DataSource ds, Version ver)
        {
        }

        protected virtual DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Success;
        }
        protected virtual DataStatus OnDeleteAfter(DataSource ds)
        {
            return DataStatus.Success;
        }
        protected virtual void OnDeleteFailed(DataSource ds)
        {

        }
        public DataStatus Delete(DataSource ds, params DataColumn[] columns)
        {
            DataStatus status = OnDeleteBefor(ds, ref columns);
            if (status == DataStatus.Success)
            {
                try
                {
                    DeleteImpl(ds, columns);
                    status = OnDeleteAfter(ds);
                    //if (DeleteImpl(ds, columns) > 0)
                    //{
                    //    status = OnDeleteAfter(ds);
                    //}
                    //else
                    //{
                    //    OnDeleteFailed(ds);
                    //    status = DataStatus.Failed;
                    //}
                }
                catch (Exception)
                {
                    OnDeleteFailed(ds);
                    status = DataStatus.Failed;
                    //throw;
                }
            }
            return status;
        }
        //public DataStatus Delete(DataSource ds, DataWhereQueue ps)
        //{
        //    DataStatus status = OnDeleteBefor(ds, ref columns);
        //    if (status == DataStatus.Success)
        //    {
        //        if (DeleteImpl(ds, ps) > 0)
        //        {
        //            status = OnDeleteAfter(ds);
        //        }
        //        else
        //        {
        //            status = DataStatus.Failed;
        //        }
        //    }
        //    return status;
        //}

        public DataStatus Insert(DataSource ds, params DataColumn[] columns)
        {
            return Insert(ds, ColumnMode.Exclude, columns);
        }
        protected virtual DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            return DataStatus.Success;
        }
        protected virtual DataStatus OnInsertAfter(DataSource ds)
        {
            return DataStatus.Success;
        }
        protected virtual void OnInsertFailed(DataSource ds)
        {

        }
        public DataStatus Insert(DataSource ds, ColumnMode mode, params DataColumn[] columns)
        {
            DataStatus status = OnInsertBefor(ds, mode, ref columns);
            if (status == DataStatus.Success)
            {
                try
                {
                    if (InsertImpl(ds, mode, columns))
                    {
                        status = OnInsertAfter(ds);
                    }
                    else
                    {
                        OnInsertFailed(ds);
                        status = DataStatus.Failed;
                    }
                }
#if (DEBUG)
                catch (Exception ex)
#else
                catch (Exception)
#endif
                {
                    OnInsertFailed(ds);
                    status = DataStatus.Failed;
                    //throw;
                }
            }
            return status;
        }
        public DataStatus InsertOrReplace(DataSource ds, params DataColumn[] columns)
        {
            return InsertOrReplace(ds, ColumnMode.Exclude, columns);
        }
        public DataStatus InsertOrReplace(DataSource ds, ColumnMode mode, params DataColumn[] columns)
        {
            DataStatus status = OnInsertBefor(ds, mode, ref columns);
            if (status == DataStatus.Success)
            {
                try
                {
                    if (InsertOrReplaceImpl(ds, mode, columns))
                    {
                        OnInsertAfter(ds);
                        status = DataStatus.Success;
                    }
                    else
                    {
                        OnInsertFailed(ds);
                        status = DataStatus.Failed;
                    }
                }
                catch (Exception)
                {
                    OnInsertFailed(ds);
                    status = DataStatus.Failed;
                }
            }
            return status;
        }

        public DataStatus Update(DataSource ds, params DataColumn[] columns)
        {
            return Update(ds, ColumnMode.Exclude, columns, null);
        }
        public DataStatus Update(DataSource ds, ColumnMode mode, params DataColumn[] columns)
        {
            return Update(ds, mode, columns, null);
        }
        protected virtual DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            return DataStatus.Success;
        }
        protected virtual DataStatus OnUpdateAfter(DataSource ds)
        {
            return DataStatus.Success;
        }
        protected virtual void OnUpdateFailed(DataSource ds)
        {

        }
        public DataStatus Update(DataSource ds, ColumnMode mode, DataColumn[] columns, DataWhereQueue ps)
        {
            DataStatus status = OnUpdateBefor(ds, mode, ref columns);
            if (status == DataStatus.Success)
            {
                try
                {
                    if (UpdateImpl(ds, mode, columns, ps) > 0)
                    {
                        status = OnUpdateAfter(ds);
                    }
                    else
                    {
                        OnUpdateFailed(ds);
                        status = DataStatus.Failed;
                    }
                }
                catch (Exception)
                {
                    OnUpdateFailed(ds);
                    status = DataStatus.Failed;
                    //throw;
                }
            }
            return status;
        }

        //protected static string[] Cs(params string[] columns)
        //{
        //    return columns;
        //}
        protected static DataOrder Oa(string column)
        {
            return new DataOrder(column, DataSortType.Asc);
        }
        protected static DataOrder Od(string column)
        {
            return new DataOrder(column, DataSortType.Desc);
        }
        protected static DataOrder Oa<T>(string column) where T : DbTable
        {
            return new DataOrder<T>(column, DataSortType.Asc);
        }
        protected static DataOrder Od<T>(string column) where T : DbTable
        {
            return new DataOrder<T>(column, DataSortType.Desc);
        }
        protected static DataOrder[] Os(params DataOrder[] orders)
        {
            return orders;
        }
        protected static DataParameter P(string name, object value)
        {
            return new DataParameter(name, value);
        }
        protected static DataParameter P<T>(string name, object value) where T : DbTable
        {
            return new DataParameter<T>(name, value);
        }
        protected static DataParameter[] Ps(params DataParameter[] ps)
        {
            return ps;
        }

        protected static DataColumn C(string column)
        {
            return new DataColumn(column);
        }
        protected static DataColumn C<T>(string column) where T : DbTable
        {
            return new DataColumn<T>(column);
        }
        protected static DataColumn C(string column, string name)
        {
            return new DataRenameColumn(column, name);
        }
        protected static DataColumn C<T>(string column, string name) where T : DbTable
        {
            return new DataRenameColumn<T>(column, name);
        }
        protected static DataColumn COUNTC(string column, string name = null)
        {
            return new DataCountColumn(column, name);
        }
        protected static DataColumn COUNTC<T>(string column, string name = null) where T : DbTable
        {
            return new DataCountColumn<T>(column, name);
        }
        protected static DataColumn SUMC(string column, string name = null)
        {
            return new DataSumColumn(column, name);
        }
        protected static DataColumn SUMC<T>(string column, string name = null) where T : DbTable
        {
            return new DataSumColumn<T>(column, name);
        }
        protected static DataColumn MAXC(string column, string name = null)
        {
            return new DataMaxColumn(column, name);
        }
        protected static DataColumn MAXC<T>(string column, string name = null) where T : DbTable
        {
            return new DataMaxColumn<T>(column, name);
        }
        protected static DataColumn MINC(string column, string name = null)
        {
            return new DataMinColumn(column, name);
        }
        protected static DataColumn MINC<T>(string column, string name = null) where T : DbTable
        {
            return new DataMinColumn<T>(column, name);
        }
        protected static DataColumn[] Cs(params DataColumn[] cs)
        {
            return cs;
        }
        //protected static DataColumn VALC(string column, object value)
        //{
        //    return new DataValueColumn(column, value);
        //}
        protected static DataColumn MODC(string column, long value)
        {
            return new DataModifiedColumn<long>(column, value);
        }
        protected static DataColumn MODC(string column, decimal value)
        {
            return new DataModifiedColumn<decimal>(column, value);
        }
        protected static DataColumn MODC(string column, double value)
        {
            return new DataModifiedColumn<double>(column, value);
        }

        //protected static DataParameter W(string name, object value, string op = "=")
        //{
        //    return new DataWhere(name, value, op);
        //}
        protected static DataParameter W<T>(string name, object value, string op) where T : DbTable
        {
            return new DataWhere<T>(name, value, op);
        }
        protected static DataParameter WN(string name, object value, string pname, string op = "=")
        {
            return new DataNameWhere(name, value, pname, op);
        }
        protected static DataParameter WN<T>(string name, object value, string pname, string op = "=") where T : DbTable
        {
            return new DataNameWhere<T>(name, value, pname, op);
        }
        protected static DataParameter WD
        {
            get { return DataFormatWhere.Default; }
        }
        protected static DataParameter WF(string name, object value, string format)
        {
            return new DataFormatWhere(name, value, format);
        }
        protected static DataParameter WF<T>(string name, object value, string format) where T : DbTable
        {
            return new DataFormatWhere<T>(name, value, format);
        }

#region Query
        protected static DbSelect S(string column = null)
        {
            return new DbSelect(column);
        }
        protected static DbSelect S<T>(string column = null) where T : IDbReader
        {
            return new DbSelect<T>(column);
        }
        protected static DbSelect S_AS(string column, string name = null)
        {
            return new DbSelectAs(column, name);
        }
        protected static DbSelect S_AS<T>(string column, string name = null) where T : IDbReader
        {
            return new DbSelectAs<T>(column, name);
        }
        protected static DbSelect S_AS<A, B>(string column, string name = null) where A : IDbReader where B : IDbReader
        {
            return new DbSelectAs<A, B>(column, name);
        }
        protected static DbSelect S_COUNT(string column = null)
        {
            return new DbSelectCount(column);
        }
        protected static DbSelect S_MAX(string column)
        {
            return new DbSelectMax(column);
        }
        protected static DbSelect S_MAX<T>(string column) where T : IDbReader
        {
            return new DbSelectMax<T>(column);
        }
        protected static DbSelect S_MIN(string column)
        {
            return new DbSelectMin(column);
        }
        protected static DbSelect S_MIN<T>(string column) where T : IDbReader
        {
            return new DbSelectMin<T>(column);
        }
        protected static DbSelect S_SUM(string column)
        {
            return new DbSelectSum(column);
        }
        protected static DbSelect S_SUM<T>(string column) where T : IDbReader
        {
            return new DbSelectSum<T>(column);
        }
        protected static DbSelect S_SUM(string column, string other, DbSumExpressionType type)
        {
            return new DbSelectSumExpression(column, other, type);
        }
        protected static DbSelect S_SUM<T>(string column, string other, DbSumExpressionType type) where T : IDbReader
        {
            return new DbSelectSumExpression<T>(column, other, type);
        }
        protected static DbColumn<T> O<T>(string column) where T : IDbReader
        {
            return new DbColumn<T>(column);
        }
        protected static DbWhere W(string column)
        {
            return new DbWhere(column);
        }
        protected static DbWhere W<T>(string column) where T : IDbReader
        {
            return new DbWhere<T>(column);
        }
        protected static DbWhere W(string column, object value, DbWhereType type = DbWhereType.Equal)
        {
            return new DbWhere(column, value, type);
        }
        protected static DbWhere W<T>(string column, object value, DbWhereType type = DbWhereType.Equal) where T : IDbReader
        {
            return new DbWhere<T>(column, value, type);
        }
        protected static DbGroupBy G(string column)
        {
            return new DbGroupBy(column);
        }
        protected static DbGroupBy G<T>(string column) where T : IDbReader
        {
            return new DbGroupBy<T>(column);
        }
        protected static DbOrderBy A(string column)
        {
            return new DbOrderBy(column, DbOrderByType.Asc);
        }
        protected static DbOrderBy A<T>(string column) where T : IDbReader
        {
            return new DbOrderBy<T>(column, DbOrderByType.Asc);
        }
        protected static DbOrderBy D(string column)
        {
            return new DbOrderBy(column, DbOrderByType.Desc);
        }
        protected static DbOrderBy D<T>(string column) where T : IDbReader
        {
            return new DbOrderBy<T>(column, DbOrderByType.Desc);
        }
#endregion
    }

    [Serializable]
    public abstract class IdentityModule : Module
    {
#if (DEBUG)
        [Description("标识")]
#endif
        [DataColumn(true, true)]
        public int Id = 0;

        protected override void SetId(long id)
        {
            Id = (int)id;
        }
    }

    [Serializable]
    public abstract class LongIdentityModule : Module
    {
#if (DEBUG)
        [Description("标识")]
#endif
        [DataColumn(true, true)]
        public long Id = 0;

        protected override void SetId(long id)
        {
            Id = id;
        }
    }

    [Serializable]
    public abstract class NoIdentityModule : Module
    {
        protected override void SetId(long id)
        {
        }
    }
}