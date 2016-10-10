using System;
using System.Collections.Generic;
using System.Text;

namespace Cnaws.Data.Query
{
    public sealed class DbOrderByQuery<T> : IDbSelectQuery where T : IDbSelectQuery
    {
        private T _query;
        private DbOrderBy[] _order;

        internal DbOrderByQuery(T query, DbOrderBy[] order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            if (order.Length == 0)
                throw new ArgumentException();
            _query = query;
            _order = order;
        }

        DbQuery IDbSelectQuery.Query
        {
            get { return _query.Query; }
        }
        DbQueryBuilder IDbSelectQuery.Build(DataSource ds, long top, bool join)
        {
            DbQueryBuilder builder = _query.Build(ds, top, join);
            builder.Append(" ORDER BY ");
            for (int i = 0; i < _order.Length; ++i)
            {
                if (i > 0)
                    builder.Append(',');
                builder.Append(_order[i].Build(ds));
            }
            return builder;
        }
        DbQueryBuilder IDbSelectQuery.BuildCount(DataSource ds, long top, bool join, ref bool group)
        {
            return _query.BuildCount(ds, top, join, ref group);
        }
        DbQueryRowNumberBuilder IDbSelectQuery.BuildRowNumber(DataSource ds, long top, bool join, string order, bool reverse)
        {
            StringBuilder osb = new StringBuilder(" ORDER BY ");
            for (int i = 0; i < _order.Length; ++i)
            {
                if (i > 0)
                    osb.Append(',');
                osb.Append(_order[i].BuildBase(ds));
            }
            StringBuilder tsb = new StringBuilder(osb.Length);
            tsb.Append("ORDER BY ");
            if (reverse)
            {
                for (int i = 0; i < _order.Length; ++i)
                {
                    if (i > 0)
                        tsb.Append(',');
                    tsb.Append(_order[i].BuildReverse(ds));
                }
            }
            else
            {
                for (int i = 0; i < _order.Length; ++i)
                {
                    if (i > 0)
                        tsb.Append(',');
                    tsb.Append(_order[i].Build(ds));
                }
            }
            string torder = tsb.ToString();
            DbQueryRowNumberBuilder builder = _query.BuildRowNumber(ds, top, join, torder, reverse);
            builder.OrderBy = osb.ToString();
            builder.Append(' ').Append(torder);
            return builder;
        }

        public object Single()
        {
            return (new DbSingleQuery<DbOrderByQuery<T>>(this)).Execute();
        }
        public R Single<R>()
        {
            return (new DbSingleQuery<DbOrderByQuery<T>>(this)).Execute<R>();
        }
        public dynamic First()
        {
            return (new DbFirstQuery<DbOrderByQuery<T>>(this)).Execute();
        }
        public R First<R>() where R : IDbReader, new()
        {
            return (new DbFirstQuery<DbOrderByQuery<T>>(this)).Execute<R>();
        }
        public IList<dynamic> ToList()
        {
            return (new DbToListQuery<DbOrderByQuery<T>>(this)).Execute();
        }
        public IList<R> ToList<R>() where R : IDbReader, new()
        {
            return (new DbToListQuery<DbOrderByQuery<T>>(this)).Execute<R>();
        }
        public object[] ToArray()
        {
            return (new DbToArrayQuery<DbOrderByQuery<T>>(this)).Execute();
        }
        public R[] ToArray<R>()
        {
            return (new DbToArrayQuery<DbOrderByQuery<T>>(this)).Execute<R>();
        }
        public IList<dynamic> ToList(int size, long page = 1)
        {
            return (new DbToListQuery<DbOrderByQuery<T>>(this)).Execute(size, page);
        }
        public IList<R> ToList<R>(int size, long page = 1) where R : IDbReader, new()
        {
            return (new DbToListQuery<DbOrderByQuery<T>>(this)).Execute<R>(size, page);
        }
        public IList<dynamic> ToList(int size, long page, out long count)
        {
            return (new DbToListQuery<DbOrderByQuery<T>>(this)).Execute(size, page, out count);
        }
        public IList<R> ToList<R>(int size, long page, out long count) where R : IDbReader, new()
        {
            return (new DbToListQuery<DbOrderByQuery<T>>(this)).Execute<R>(size, page, out count);
        }
    }
}
