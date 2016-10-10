using System;

namespace Cnaws.Data.Query
{
    internal abstract class DbQueryBase
    {
        private DbQuery _query;

        protected DbQueryBase(DbQuery query)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            _query = query;
        }

        protected DbQuery Query
        {
            get { return _query; }
        }
    }

    internal abstract class DbQueryBase<T> : DbQueryBase where T : IDbReader
    {
        protected DbQueryBase(DbQuery<T> query)
            : base(query)
        {
        }
    }
}
