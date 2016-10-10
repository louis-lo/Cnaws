using System;

namespace Cnaws.Data.Query
{
    public sealed class DbSubResultQuery<T, Q> : IDbQuery, IDbSubQuery<Q>, IDbQueryBuilder where T : IDbQuery, IDbSubQuery<Q>, IDbQueryBuilder where Q : IDbReader
    {
        private T _query;

        internal DbSubResultQuery(T query)
        {
            _query = query;
        }

        DbQuery IDbQuery.Query
        {
            get { return _query.Query; }
        }
        DbSelectQuery<Q> IDbSubQuery<Q>.SubQuery
        {
            get { return _query.SubQuery; }
        }

        private R Build<R>(DataSource ds, R builder) where R : DbQueryBuilder
        {
            builder.Append(')');
            return builder;
        }
        DbQueryBuilder IDbQueryBuilder.Build(DataSource ds, long top, bool join)
        {
            return Build(ds, _query.Build(ds, top, join));
        }
        DbQueryBuilder IDbQueryBuilder.BuildCount(DataSource ds, long top, bool join, ref bool group)
        {
            return Build(ds, _query.BuildCount(ds, top, join, ref group));
        }
        DbQueryRowNumberBuilder IDbQueryBuilder.BuildRowNumber(DataSource ds, long top, bool join, string order, bool reverse)
        {
            return Build(ds, _query.BuildRowNumber(ds, top, join, order, reverse));
        }


    }
}
