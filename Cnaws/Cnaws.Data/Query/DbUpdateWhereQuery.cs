using System;

namespace Cnaws.Data.Query
{
    public sealed class DbUpdateWhereQuery<T> : IDbUpdateQuery where T : IDbUpdateQuery
    {
        private T _query;
        private DbWhereQueue _where;

        internal DbUpdateWhereQuery(T query, DbWhereQueue queue)
        {
            _query = query;
            _where = queue;
        }

        DbQuery IDbUpdateQuery.Query
        {
            get { return _query.Query; }
        }
        DbQueryBuilder IDbUpdateQuery.Build(DataSource ds, ref int count)
        {
            DbQueryBuilder builder = _query.Build(ds, ref count);
            if (_where != null)
                builder.Append(" WHERE ").Append(_where.Build(ds));
            return builder;
        }

        public int Execute()
        {
            return (new DbUpdateExecuteQuery<DbUpdateWhereQuery<T>>(this)).Execute();
        }
    }
}
