using System;

namespace Cnaws.Data.Query
{
    public sealed class DbJoinGroupByQuery<Q, T, A, B> : IDbJoinQuery<T, A, B> where Q : IDbJoinQuery<T, A, B> where T : IDbSelectQuery where A : IDbReader where B : IDbReader
    {
        private Q _query;
        private DbGroupBy[] _group;

        internal DbJoinGroupByQuery(Q query, DbGroupBy[] group)
        {
            if (group == null)
                throw new ArgumentNullException("group");
            if (group.Length == 0)
                throw new ArgumentException();
            _query = query;
            _group = group;
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
            builder.Append(" GROUP BY ");
            for (int i = 0; i < _group.Length; ++i)
            {
                if (i > 0)
                    builder.Append(',');
                builder.Append(_group[i].Build(ds));
            }
            return builder;
        }

        public DbJoinQuery<T, A, B> Result()
        {
            return _query.Parent;
        }
    }
}
