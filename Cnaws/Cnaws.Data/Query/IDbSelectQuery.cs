using System;

namespace Cnaws.Data.Query
{
    public interface IDbSelectQuery
    {
        DbQuery Query { get; }
        DbQueryBuilder Build(DataSource ds, long top, bool join);
        DbQueryBuilder BuildCount(DataSource ds, long top, bool join, ref bool group);
        DbQueryRowNumberBuilder BuildRowNumber(DataSource ds, long top, bool join, string order, bool reverse);
    }
}
