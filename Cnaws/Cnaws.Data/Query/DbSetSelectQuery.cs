using System;

namespace Cnaws.Data.Query
{
    public sealed class DbSetSelectQuery<T> : DbSetQueryBase<T> where T : IDbUpdateQuery
    {
        internal DbSetSelectQuery(T query, DbColumn column)
            : base(query, column)
        {
        }

        public DbSubSelectQuery<O, DbSetSelectResultQuery<DbSetSelectQuery<T>>> Select<O>(DbSelect column) where O : IDbReader
        {
            return new DbSubSelectQuery<O, DbSetSelectResultQuery<DbSetSelectQuery<T>>>(new DbSetSelectResultQuery<DbSetSelectQuery<T>>(this), column);
        }
    }
}
