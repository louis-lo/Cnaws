using System;

namespace Cnaws.Data.Query
{
    public sealed class DbJoinWhereQuery<Q, T, A, B> : IDbJoinQuery<T, A, B> where Q : IDbJoinQuery<T, A, B> where T : IDbSelectQuery where A : IDbReader where B : IDbReader
    {
        private Q _query;
        private DbWhereQueue _where;

        internal DbJoinWhereQuery(Q query, DbWhereQueue queue)
        {
            _query = query;
            _where = queue;
            _query.Parent.Refresh(this);
        }

        IDbJoinQuery<T, A, B> IDbJoinQuery<T, A, B>.Query
        {
            get { return _query.Query; }
        }
        DbJoinQuery<T, A, B> IDbJoinQuery<T, A, B>.Parent
        {
            get { return _query.Parent; }
        }
        DbQueryBuilder IDbJoinQuery<T, A, B>.Build(DataSource ds, long top, bool join)
        {
            DbQueryBuilder builder = _query.Build(ds, top, join);
            if (_where != null)
                builder.Append(" WHERE ").Append(_where.Build(ds));
            return builder;
        }

        public DbJoinGroupByQuery<DbJoinWhereQuery<Q, T, A, B>, T, A, B> GroupBy(params DbGroupBy[] group)
        {
            return new DbJoinGroupByQuery<DbJoinWhereQuery<Q, T, A, B>, T, A, B>(this, group);
        }
        public DbJoinQuery<T, A, B> Result()
        {
            return _query.Parent;
        }
    }
}
