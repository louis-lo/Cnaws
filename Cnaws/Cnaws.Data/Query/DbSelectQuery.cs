using System;
using System.Collections.Generic;

namespace Cnaws.Data.Query
{
    //public sealed class DbSelectQuery : IDbQuery
    //{
    //    private DbQuery _query;
    //    private DbSelect[] _select;
    //    private IDbSubQuery _sub;

    //    internal DbSelectQuery(DbQuery query, DbSelect[] select)
    //    {
    //        _query = query;
    //        _select = select;
    //        _sub = null;
    //    }

    //    DbQuery IDbQuery.Query
    //    {
    //        get { return _query; }
    //    }
    //    private static string GetTableName(DataSource ds)
    //    {
    //        return ds.Provider.EscapeName(string.Concat('T', ds.PsCount));
    //    }
    //    DbQueryBuilder IDbQuery.Build(DataSource ds, long top, bool join)
    //    {
    //        string table = GetTableName(ds);
    //        DbQueryBuilder builder = new DbQueryBuilder("SELECT");
    //        if (top > 0)
    //            builder.Append(" TOP ").Append(top);
    //        builder.Append(' ');
    //        if (_select == null || _select.Length == 0)
    //        {
    //            if (join)
    //                builder.Append(table).Append(".*");
    //            else
    //                builder.Append((new DbSelect()).Build(ds));
    //        }
    //        else {
    //            for (int i = 0; i < _select.Length; ++i)
    //            {
    //                if (i > 0)
    //                    builder.Append(',');
    //                builder.Append(_select[i].Build(ds));
    //            }
    //        }
    //        builder.Append(" FROM (");
    //        _sub.Build(ds, builder);
    //        builder.Append(") AS ").Append(table);
    //        return builder;
    //    }
    //    DbQueryBuilder IDbQuery.BuildCount(DataSource ds, long top, bool join, ref bool group)
    //    {
    //        string table = GetTableName(ds);
    //        DbQueryBuilder builder = new DbQueryBuilder("SELECT COUNT(*) FROM ");
    //        if (group)
    //            builder.Append("(SELECT COUNT(*) AS C").Append(ds.PsCount).Append(" FROM ");
    //        builder.Append('(');
    //        _sub.Build(ds, builder);
    //        builder.Append(") AS ").Append(table);
    //        return builder;
    //    }
    //    DbQueryRowNumberBuilder IDbQuery.BuildRowNumber(DataSource ds, long top, bool join, string order, bool reverse)
    //    {
    //        string table = GetTableName(ds);
    //        DbQueryRowNumberBuilder builder = new DbQueryRowNumberBuilder("WITH CTE AS(SELECT TOP ");
    //        builder.Append(top).Append(" ROW_NUMBER() OVER(").Append(order).Append(")AS _RowNumber,");
    //        if (_select == null || _select.Length == 0)
    //        {
    //            if (join)
    //                builder.Append(table).Append(".*");
    //            else
    //                builder.Append((new DbSelect()).Build(ds));
    //        }
    //        else
    //        {
    //            for (int i = 0; i < _select.Length; ++i)
    //            {
    //                if (i > 0)
    //                    builder.Append(',');
    //                builder.Append(_select[i].Build(ds));
    //            }
    //        }
    //        builder.Append(" FROM (");
    //        _sub.Build(ds, builder);
    //        builder.Append(") AS ").Append(table);
    //        return builder;
    //    }

    //    internal void SetSub(IDbSubQuery sub)
    //    {
    //        _sub = sub;
    //    }

    //    public object FromSelect()
    //    {

    //    }
    //    public object FromSelect<T>() where T : IDbReader
    //    {

    //    }
    //}

    public sealed class DbSelectQuery<T> : IDbSelectQuery where T : IDbReader
    {
        private DbQuery<T> _query;
        private DbSelect[] _select;

        internal DbSelectQuery(DbQuery<T> query, DbSelect[] select)
        {
            _query = query;
            _select = select;
        }

        DbQuery IDbSelectQuery.Query
        {
            get { return _query; }
        }
        DbQueryBuilder IDbSelectQuery.Build(DataSource ds, long top, bool join)
        {
            DbQueryBuilder builder = new DbQueryBuilder("SELECT");
            if (top > 0)
                builder.Append(" TOP ").Append(top);
            builder.Append(' ');
            if (_select == null || _select.Length == 0)
            {
                if (join)
                    builder.Append((new DbSelect<T>()).Build(ds));
                else
                    builder.Append((new DbSelect()).Build(ds));
            }
            else {
                for (int i = 0; i < _select.Length; ++i)
                {
                    if (i > 0)
                        builder.Append(',');
                    builder.Append(_select[i].Build(ds));
                }
            }
            builder.Append(" FROM ").Append(ds.Provider.EscapeName(DbTable.GetTableName<T>()));
            return builder;
        }
        DbQueryBuilder IDbSelectQuery.BuildCount(DataSource ds, long top, bool join, ref bool group)
        {
            DbQueryBuilder builder = new DbQueryBuilder("SELECT COUNT(*) FROM ");
            if (group)
                builder.Append("(SELECT COUNT(*) AS C").Append(ds.PsCount).Append(" FROM ");
            builder.Append(ds.Provider.EscapeName(DbTable.GetTableName<T>()));
            return builder;
        }
        DbQueryRowNumberBuilder IDbSelectQuery.BuildRowNumber(DataSource ds, long top, bool join, string order, bool reverse)
        {
            DbQueryRowNumberBuilder builder = new DbQueryRowNumberBuilder("WITH CTE AS(SELECT TOP ");
            builder.Append(top).Append(" ROW_NUMBER() OVER(").Append(order).Append(")AS _RowNumber,");
            if (_select == null || _select.Length == 0)
            {
                if (join)
                    builder.Append((new DbSelect<T>()).Build(ds));
                else
                    builder.Append((new DbSelect()).Build(ds));
            }
            else
            {
                for (int i = 0; i < _select.Length; ++i)
                {
                    if (i > 0)
                        builder.Append(',');
                    builder.Append(_select[i].Build(ds));
                }
            }
            builder.Append(" FROM ").Append(ds.Provider.EscapeName(DbTable.GetTableName<T>()));
            return builder;
        }

        public DbSelectWhereQuery<DbSelectQuery<T>> Where(DbWhereQueue queue)
        {
            return new DbSelectWhereQuery<DbSelectQuery<T>>(this, queue);
        }
        public DbGroupByQuery<DbSelectQuery<T>> GroupBy(params DbGroupBy[] group)
        {
            return new DbGroupByQuery<DbSelectQuery<T>>(this, group);
        }
        public DbJoinQuery<DbSelectQuery<T>, A, B> InnerJoin<A, B>(DbColumn<A> left, DbColumn<B> right, DbJoinMode mode = DbJoinMode.Equal) where A : IDbReader where B : IDbReader
        {
            return new DbJoinQuery<DbSelectQuery<T>, A, B>(this, DbJoinType.Inner, left, right, mode);
        }
        public DbJoinQuery<DbSelectQuery<T>, A, B> LeftJoin<A, B>(DbColumn<A> left, DbColumn<B> right, DbJoinMode mode = DbJoinMode.Equal) where A : IDbReader where B : IDbReader
        {
            return new DbJoinQuery<DbSelectQuery<T>, A, B>(this, DbJoinType.Left, left, right, mode);
        }
        public DbJoinQuery<DbSelectQuery<T>, A, B> RightJoin<A, B>(DbColumn<A> left, DbColumn<B> right, DbJoinMode mode = DbJoinMode.Equal) where A : IDbReader where B : IDbReader
        {
            return new DbJoinQuery<DbSelectQuery<T>, A, B>(this, DbJoinType.Right, left, right, mode);
        }
        public DbOrderByQuery<DbSelectQuery<T>> OrderBy(params DbOrderBy[] order)
        {
            return new DbOrderByQuery<DbSelectQuery<T>>(this, order);
        }

        public long Count()
        {
            return (new DbCountQuery<DbSelectQuery<T>>(this)).Execute();
        }
        public object Single()
        {
            return (new DbSingleQuery<DbSelectQuery<T>>(this)).Execute();
        }
        public R Single<R>()
        {
            return (new DbSingleQuery<DbSelectQuery<T>>(this)).Execute<R>();
        }
        public dynamic First()
        {
            return (new DbFirstQuery<DbSelectQuery<T>>(this)).Execute();
        }
        public R First<R>() where R : IDbReader, new()
        {
            return (new DbFirstQuery<DbSelectQuery<T>>(this)).Execute<R>();
        }
        public IList<dynamic> ToList()
        {
            return (new DbToListQuery<DbSelectQuery<T>>(this)).Execute();
        }
        public IList<R> ToList<R>() where R : IDbReader, new()
        {
            return (new DbToListQuery<DbSelectQuery<T>>(this)).Execute<R>();
        }
        public object[] ToArray()
        {
            return (new DbToArrayQuery<DbSelectQuery<T>>(this)).Execute();
        }
        public R[] ToArray<R>()
        {
            return (new DbToArrayQuery<DbSelectQuery<T>>(this)).Execute<R>();
        }
        public IList<dynamic> ToList(int size, long page = 1)
        {
            return (new DbToListQuery<DbSelectQuery<T>>(this)).Execute(size, page);
        }
        public IList<R> ToList<R>(int size, long page = 1) where R : IDbReader, new()
        {
            return (new DbToListQuery<DbSelectQuery<T>>(this)).Execute<R>(size, page);
        }
        public IList<dynamic> ToList(int size, long page, out long count)
        {
            return (new DbToListQuery<DbSelectQuery<T>>(this)).Execute(size, page, out count);
        }
        public IList<R> ToList<R>(int size, long page, out long count) where R : IDbReader, new()
        {
            return (new DbToListQuery<DbSelectQuery<T>>(this)).Execute<R>(size, page, out count);
        }
    }
}
