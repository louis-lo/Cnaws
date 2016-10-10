using System;

namespace Cnaws.Data.Query
{
    public sealed class DbSetSelectResultQuery<T> : IDbSubQueryParent<DbSetSelectResultQuery<T>>, IDbUpdateQuery where T : IDbUpdateQuery
    {
        private T _query;
        private IDbSubQuery<DbSetSelectResultQuery<T>> _subQuery;

        internal DbSetSelectResultQuery(T query)
        {
            _query = query;
            _subQuery = null;
        }

        DbQuery IDbUpdateQuery.Query
        {
            get { return _query.Query; }
        }
        DbQueryBuilder IDbUpdateQuery.Build(DataSource ds, ref int count)
        {
            DbQueryBuilder builder = _query.Build(ds, ref count);
            DbQueryBuilder subBuilder = _subQuery.Build(ds, 0, false);
            builder.Append('(');
            builder.Append(subBuilder.Sql);
            builder.Append(')');
            builder.Append(subBuilder.Parameters);
            return builder;
        }
        void IDbSubQueryParent<DbSetSelectResultQuery<T>>.Refresh(IDbSubQuery<DbSetSelectResultQuery<T>> value)
        {
            _subQuery = value;
        }

        public DbSetSelectQuery<DbSetSelectResultQuery<T>> Set(DbColumn column)
        {
            return new DbSetSelectQuery<DbSetSelectResultQuery<T>>(this, column);
        }
        public DbSetQuery<DbSetSelectResultQuery<T>> Set(DbColumn column, object value)
        {
            return new DbSetQuery<DbSetSelectResultQuery<T>>(this, column, value);
        }
        public DbSetAddQuery<DbSetSelectResultQuery<T>, V> SetAdd<V>(DbColumn column, V value)
        {
            return new DbSetAddQuery<DbSetSelectResultQuery<T>, V>(this, column, value);
        }
        public DbUpdateWhereQuery<DbSetSelectResultQuery<T>> Where(DbWhereQueue queue)
        {
            return new DbUpdateWhereQuery<DbSetSelectResultQuery<T>>(this, queue);
        }
        public int Execute()
        {
            return (new DbUpdateExecuteQuery<DbSetSelectResultQuery<T>>(this)).Execute();
        }
    }
}
