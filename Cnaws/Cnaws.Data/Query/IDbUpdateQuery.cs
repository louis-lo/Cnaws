using System;

namespace Cnaws.Data.Query
{
    public interface IDbUpdateQuery
    {
        DbQuery Query { get; }
        DbQueryBuilder Build(DataSource ds, ref int count);
    }
}
