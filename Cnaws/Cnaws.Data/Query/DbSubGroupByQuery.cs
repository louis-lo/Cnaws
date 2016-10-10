using System;

namespace Cnaws.Data.Query
{
    public sealed class DbSubGroupByQuery<T, R> : IDbSubQuery<R> where T : IDbSubQuery<R> where R : IDbSubQueryParent<R>
    {
        private T _query;
        private DbGroupBy[] _group;

        internal DbSubGroupByQuery(T query, DbGroupBy[] group)
        {
            if (group == null)
                throw new ArgumentNullException("group");
            if (group.Length == 0)
                throw new ArgumentException();
            _query = query;
            _group = group;
            _query.Parent.Refresh(this);
        }

        IDbSubQuery<R> IDbSubQuery<R>.Query
        {
            get { return _query.Query; }
        }
        R IDbSubQuery<R>.Parent
        {
            get { return _query.Parent; }
        }
        DbQueryBuilder IDbSubQuery<R>.Build(DataSource ds, long top, bool join)
        {
            DbQueryBuilder builder = _query.Build(ds, top, join);
            builder.Append(" GROUP BY ");
            for (int i = 0; i < _group.Length; ++i)
            {
                if (i > 0)
                    builder.Append(',');
                builder.Append(_group[i].Build(ds));
            }
            return builder;
        }

        public R Result()
        {
            return _query.Parent;
        }
    }
}
