using System;

namespace Cnaws.Data.Query
{
    public interface IDbSubQuery<T>
    {
        IDbSubQuery<T> Query { get; }
        T Parent { get; }
        DbQueryBuilder Build(DataSource ds, long top, bool join);
    }
    public interface IDbSubQueryParent<T>
    {
        void Refresh(IDbSubQuery<T> value);
    }
}
