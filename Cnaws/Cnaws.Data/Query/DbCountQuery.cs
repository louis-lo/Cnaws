using System;

namespace Cnaws.Data.Query
{
    internal sealed class DbCountQuery<T> where T : IDbSelectQuery
    {
        private T _query;

        internal DbCountQuery(T query)
        {
            _query = query;
        }

        public long Execute()
        {
            bool group = false;
            DbQueryBuilder builder = _query.BuildCount(_query.Query.DataSource, 0, false, ref group);
            if (group)
                builder.Append(") AS T").Append(_query.Query.DataSource.PsCount);
            builder.Append(';');
            return Convert.ToInt64(_query.Query.DataSource.ExecuteScalar(builder.Sql, builder.Parameters));
        }
    }
}
