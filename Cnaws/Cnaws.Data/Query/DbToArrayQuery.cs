using System;
using System.Collections.Generic;

namespace Cnaws.Data.Query
{
    internal sealed class DbToArrayQuery<T> where T : IDbSelectQuery
    {
        private T _query;

        internal DbToArrayQuery(T query)
        {
            _query = query;
        }

        public object[] Execute()
        {
            DbQueryBuilder builder = _query.Build(_query.Query.DataSource, 0, false);
            builder.Append(';');
            return _query.Query.DataSource.ExecuteArray(builder.Sql, builder.Parameters);
        }
        public R[] Execute<R>()
        {
            DbQueryBuilder builder = _query.Build(_query.Query.DataSource, 0, false);
            builder.Append(';');
            return _query.Query.DataSource.ExecuteArray<R>(builder.Sql, builder.Parameters);
        }
    }
}
