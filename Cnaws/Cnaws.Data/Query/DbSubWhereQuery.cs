using System;

namespace Cnaws.Data.Query
{
    public sealed class DbSubWhereQuery<T, R> : IDbSubQuery<R> where T : IDbSubQuery<R> where R : IDbSubQueryParent<R>
    {
        private T _query;
        private DbWhereQueue _where;

        internal DbSubWhereQuery(T query, DbWhereQueue queue)
        {
            _query = query;
            _where = queue;
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
            if (_where != null)
                builder.Append(" WHERE ").Append(_where.Build(ds));
            return builder;
        }

        public DbSubGroupByQuery<DbSubWhereQuery<T, R>, R> GroupBy(params DbGroupBy[] group)
        {
            return new DbSubGroupByQuery<DbSubWhereQuery<T, R>, R>(this, group);
        }
        public R Result()
        {
            return _query.Parent;
        }
    }
}
