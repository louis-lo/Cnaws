using System;
using System.Collections.Generic;

namespace Cnaws.Data.Query
{
    public sealed class DbSelectWhereQuery<T> : IDbSelectQuery where T : IDbSelectQuery
    {
        private T _query;
        private DbWhereQueue _where;

        internal DbSelectWhereQuery(T query, DbWhereQueue queue)
        {
            _query = query;
            _where = queue;
        }

        DbQuery IDbSelectQuery.Query
        {
            get { return _query.Query; }
        }
        private R Build<R>(DataSource ds, R builder) where R : DbQueryBuilder
        {
            if (_where != null)
                builder.Append(" WHERE ").Append(_where.Build(ds));
            return builder;
        }
        DbQueryBuilder IDbSelectQuery.Build(DataSource ds, long top, bool join)
        {
            return Build(ds, _query.Build(ds, top, join));
        }
        DbQueryBuilder IDbSelectQuery.BuildCount(DataSource ds, long top, bool join, ref bool group)
        {
            return Build(ds, _query.BuildCount(ds, top, join, ref group));
        }
        DbQueryRowNumberBuilder IDbSelectQuery.BuildRowNumber(DataSource ds, long top, bool join, string order, bool reverse)
        {
            return Build(ds, _query.BuildRowNumber(ds, top, join, order, reverse));
        }

        public DbGroupByQuery<DbSelectWhereQuery<T>> GroupBy(params DbGroupBy[] group)
        {
            return new DbGroupByQuery<DbSelectWhereQuery<T>>(this, group);
        }
        public DbOrderByQuery<DbSelectWhereQuery<T>> OrderBy(params DbOrderBy[] order)
        {
            return new DbOrderByQuery<DbSelectWhereQuery<T>>(this, order);
        }

        public long Count()
        {
            return (new DbCountQuery<DbSelectWhereQuery<T>>(this)).Execute();
        }
        public object Single()
        {
            return (new DbSingleQuery<DbSelectWhereQuery<T>>(this)).Execute();
        }
        public R Single<R>()
        {
            return (new DbSingleQuery<DbSelectWhereQuery<T>>(this)).Execute<R>();
        }
        public dynamic First()
        {
            return (new DbFirstQuery<DbSelectWhereQuery<T>>(this)).Execute();
        }
        public R First<R>() where R : IDbReader, new()
        {
            return (new DbFirstQuery<DbSelectWhereQuery<T>>(this)).Execute<R>();
        }
        public IList<dynamic> ToList()
        {
            return (new DbToListQuery<DbSelectWhereQuery<T>>(this)).Execute();
        }
        public IList<R> ToList<R>() where R : IDbReader, new()
        {
            return (new DbToListQuery<DbSelectWhereQuery<T>>(this)).Execute<R>();
        }
        public object[] ToArray()
        {
            return (new DbToArrayQuery<DbSelectWhereQuery<T>>(this)).Execute();
        }
        public R[] ToArray<R>()
        {
            return (new DbToArrayQuery<DbSelectWhereQuery<T>>(this)).Execute<R>();
        }
        public IList<dynamic> ToList(int size, long page = 1)
        {
            return (new DbToListQuery<DbSelectWhereQuery<T>>(this)).Execute(size, page);
        }
        public IList<R> ToList<R>(int size, long page = 1) where R : IDbReader, new()
        {
            return (new DbToListQuery<DbSelectWhereQuery<T>>(this)).Execute<R>(size, page);
        }
        public IList<dynamic> ToList(int size, long page, out long count)
        {
            return (new DbToListQuery<DbSelectWhereQuery<T>>(this)).Execute(size, page, out count);
        }
        public IList<R> ToList<R>(int size, long page, out long count) where R : IDbReader, new()
        {
            return (new DbToListQuery<DbSelectWhereQuery<T>>(this)).Execute<R>(size, page, out count);
        }
    }
}
