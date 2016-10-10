using System;

namespace Cnaws.Data.Query
{
    public sealed class DbUpdateExecuteQuery<T> where T : IDbUpdateQuery
    {
        private T _query;

        internal DbUpdateExecuteQuery(T query)
        {
            _query = query;
        }

        public int Execute()
        {
            int count = 0;
            DbQueryBuilder builder = _query.Build(_query.Query.DataSource, ref count);
            return _query.Query.DataSource.ExecuteNonQuery(builder.Sql, builder.Parameters);
        }
    }
}
