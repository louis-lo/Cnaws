using System;
using System.Collections.Generic;

namespace Cnaws.Data.Query
{
    public sealed class DbGroupByQuery<T> : IDbSelectQuery where T : IDbSelectQuery
    {
        private T _query;
        private DbGroupBy[] _group;

        internal DbGroupByQuery(T query, DbGroupBy[] group)
        {
            if (group == null)
                throw new ArgumentNullException("group");
            if (group.Length == 0)
                throw new ArgumentException();
            _query = query;
            _group = group;
        }

        DbQuery IDbSelectQuery.Query
        {
            get { return _query.Query; }
        }
        private R Build<R>(DataSource ds, R builder) where R : DbQueryBuilder
        {
            builder.Append(" GROUP BY ");
            for (int i = 0; i < _group.Length; ++i)
            {
                if (i > 0)
                    builder.Append(',');
                builder.Append(_group[i].Build(ds));
            }
            return builder;
        }
        DbQueryBuilder IDbSelectQuery.Build(DataSource ds, long top, bool join)
        {
            return Build(ds, _query.Build(ds, top, join));
        }
        DbQueryBuilder IDbSelectQuery.BuildCount(DataSource ds, long top, bool join, ref bool group)
        {
            group = true;
            return Build(ds, _query.BuildCount(ds, top, join, ref group));
        }
        DbQueryRowNumberBuilder IDbSelectQuery.BuildRowNumber(DataSource ds, long top, bool join, string order, bool reverse)
        {
            return Build(ds, _query.BuildRowNumber(ds, top, join, order, reverse));
        }
        
        public DbOrderByQuery<DbGroupByQuery<T>> OrderBy(params DbOrderBy[] order)
        {
            return new DbOrderByQuery<DbGroupByQuery<T>>(this, order);
        }

        public long Count()
        {
            return (new DbCountQuery<DbGroupByQuery<T>>(this)).Execute();
        }
        public object Single()
        {
            return (new DbSingleQuery<DbGroupByQuery<T>>(this)).Execute();
        }
        public R Single<R>()
        {
            return (new DbSingleQuery<DbGroupByQuery<T>>(this)).Execute<R>();
        }
        public dynamic First()
        {
            return (new DbFirstQuery<DbGroupByQuery<T>>(this)).Execute();
        }
        public R First<R>() where R : IDbReader, new()
        {
            return (new DbFirstQuery<DbGroupByQuery<T>>(this)).Execute<R>();
        }
        public IList<dynamic> ToList()
        {
            return (new DbToListQuery<DbGroupByQuery<T>>(this)).Execute();
        }
        public IList<R> ToList<R>() where R : IDbReader, new()
        {
            return (new DbToListQuery<DbGroupByQuery<T>>(this)).Execute<R>();
        }
        public object[] ToArray()
        {
            return (new DbToArrayQuery<DbGroupByQuery<T>>(this)).Execute();
        }
        public R[] ToArray<R>()
        {
            return (new DbToArrayQuery<DbGroupByQuery<T>>(this)).Execute<R>();
        }
        public IList<dynamic> ToList(int size, long page = 1)
        {
            return (new DbToListQuery<DbGroupByQuery<T>>(this)).Execute(size, page);
        }
        public IList<R> ToList<R>(int size, long page = 1) where R : IDbReader, new()
        {
            return (new DbToListQuery<DbGroupByQuery<T>>(this)).Execute<R>(size, page);
        }
        public IList<dynamic> ToList(int size, long page, out long count)
        {
            return (new DbToListQuery<DbGroupByQuery<T>>(this)).Execute(size, page, out count);
        }
        public IList<R> ToList<R>(int size, long page, out long count) where R : IDbReader, new()
        {
            return (new DbToListQuery<DbGroupByQuery<T>>(this)).Execute<R>(size, page, out count);
        }
    }
}
