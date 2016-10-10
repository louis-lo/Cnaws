using System;

namespace Cnaws.Data.Query
{
    internal sealed class DbFirstQuery<T> where T : IDbSelectQuery
    {
        private T _query;

        internal DbFirstQuery(T query)
        {
            _query = query;
        }

        public dynamic Execute()
        {
            DbQueryBuilder builder = _query.Build(_query.Query.DataSource, 0, false);
            builder.Append(';');
            return _query.Query.DataSource.ExecuteSingleRow(builder.Sql, builder.Parameters);
        }
        public R Execute<R>() where R : IDbReader, new()
        {
            DbQueryBuilder builder = _query.Build(_query.Query.DataSource, 0, false);
            builder.Append(';');
            return _query.Query.DataSource.ExecuteSingleRow<R>(builder.Sql, builder.Parameters);
        }
    }
}
