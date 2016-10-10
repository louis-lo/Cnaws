using System;

namespace Cnaws.Data.Query
{
    public sealed class DbSetQuery<T> : DbSetQueryBase<T> where T : IDbUpdateQuery
    {
        private object _value;

        internal DbSetQuery(T query, DbColumn column, object value)
            : base(query, column)
        {
            _value = value;
        }

        protected override void OnBuild(DataSource ds, DbQueryBuilder builder, string column)
        {
            DataParameter dp = Query.Query.BuildParameter(_value);
            builder.Append(dp.GetParameterName());
            builder.Append(dp);
        }

        public DbSetSelectQuery<DbSetQuery<T>> Set(DbColumn column)
        {
            return new DbSetSelectQuery<DbSetQuery<T>>(this, column);
        }
        public DbSetQuery<DbSetQuery<T>> Set(DbColumn column, object value)
        {
            return new DbSetQuery<DbSetQuery<T>>(this, column, value);
        }
        public DbSetAddQuery<DbSetQuery<T>, V> SetAdd<V>(DbColumn column, V value)
        {
            return new DbSetAddQuery<DbSetQuery<T>, V>(this, column, value);
        }
        public DbUpdateWhereQuery<DbSetQuery<T>> Where(DbWhereQueue queue)
        {
            return new DbUpdateWhereQuery<DbSetQuery<T>>(this, queue);
        }
        public int Execute()
        {
            return (new DbUpdateExecuteQuery<DbSetQuery<T>>(this)).Execute();
        }
    }
}
