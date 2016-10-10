using System;

namespace Cnaws.Data.Query
{
    internal sealed class DbSingleQuery<T> where T : IDbSelectQuery
    {
        private T _query;

        internal DbSingleQuery(T query)
        {
            _query = query;
        }

        public object Execute()
        {
            DbQueryBuilder builder = _query.Build(_query.Query.DataSource, 0, false);
            builder.Append(';');
            return _query.Query.DataSource.ExecuteScalar(builder.Sql, builder.Parameters);
        }
        public R Execute<R>()
        {
            DbQueryBuilder builder = _query.Build(_query.Query.DataSource, 0, false);
            builder.Append(';');
            return _query.Query.DataSource.ExecuteScalar<R>(builder.Sql, builder.Parameters);
        }
    }
}
