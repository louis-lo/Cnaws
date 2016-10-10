using System;

namespace Cnaws.Data.Query
{
    public sealed class DbUpdateQuery<T> : IDbUpdateQuery where T : IDbReader
    {
        private DbQuery<T> _query;

        internal DbUpdateQuery(DbQuery<T> query)
        {
            _query = query;
        }

        DbQuery IDbUpdateQuery.Query
        {
            get { return _query; }
        }
        DbQueryBuilder IDbUpdateQuery.Build(DataSource ds, ref int count)
        {
            DbQueryBuilder builder = new DbQueryBuilder("UPDATE ");
            builder.Append(ds.Provider.EscapeName(DbTable.GetTableName<T>()));
            builder.Append(" SET ");
            return builder;
        }

        public DbSetSelectQuery<DbUpdateQuery<T>> Set(DbColumn column)
        {
            return new DbSetSelectQuery<DbUpdateQuery<T>>(this, column);
        }
        public DbSetQuery<DbUpdateQuery<T>> Set(DbColumn column, object value)
        {
            return new DbSetQuery<DbUpdateQuery<T>>(this, column, value);
        }
        public DbSetAddQuery<DbUpdateQuery<T>, V> SetAdd<V>(DbColumn column, V value)
        {
            return new DbSetAddQuery<DbUpdateQuery<T>, V>(this, column, value);
        }
    }
}
