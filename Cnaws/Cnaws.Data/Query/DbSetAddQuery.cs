using System;

namespace Cnaws.Data.Query
{
    public sealed class DbSetAddQuery<T, V> : DbSetQueryBase<T> where T : IDbUpdateQuery
    {
        private V _value;

        internal DbSetAddQuery(T query, DbColumn column, V value)
            : base(query, column)
        {
            _value = value;
        }

        protected override void OnBuild(DataSource ds, DbQueryBuilder builder, string column)
        {
            DataParameter dp = Query.Query.BuildParameter(_value);
            builder.Append(column);
            builder.Append('+');
            builder.Append(dp.GetParameterName());
            builder.Append(dp);
        }

        public DbSetSelectQuery<DbSetAddQuery<T, V>> Set(DbColumn column)
        {
            return new DbSetSelectQuery<DbSetAddQuery<T, V>>(this, column);
        }
        public DbSetQuery<DbSetAddQuery<T, V>> Set(DbColumn column, object value)
        {
            return new DbSetQuery<DbSetAddQuery<T, V>>(this, column, value);
        }
        public DbSetAddQuery<DbSetAddQuery<T, V>, O> SetAdd<O>(DbColumn column, O value)
        {
            return new DbSetAddQuery<DbSetAddQuery<T, V>, O>(this, column, value);
        }
        public DbUpdateWhereQuery<DbSetAddQuery<T, V>> Where(DbWhereQueue queue)
        {
            return new DbUpdateWhereQuery<DbSetAddQuery<T, V>>(this, queue);
        }
        public int Execute()
        {
            return (new DbUpdateExecuteQuery<DbSetAddQuery<T, V>>(this)).Execute();
        }
    }
}
