using System;

namespace Cnaws.Data.Query
{
    public interface IDbJoinQuery<T, A, B> where T : IDbSelectQuery where A : IDbReader where B : IDbReader
    {
        IDbJoinQuery<T, A, B> Query { get; }
        DbJoinQuery<T, A, B> Parent { get; }
        DbQueryBuilder Build(DataSource ds, long top, bool join);
    }

    public interface IDbJoinQueryParent<T, A, B> where T : IDbSelectQuery where A : IDbReader where B : IDbReader
    {
        void Refresh(IDbJoinQuery<T, A, B> value);
    }
}
