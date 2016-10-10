using System;

namespace Cnaws.Data.Query
{
    //public static class Db
    //{
    //    public static DbQuery Query(DataSource ds)
    //    {
    //        if (ds == null)
    //            throw new ArgumentNullException("ds");
    //        return new DbQuery(ds);
    //    }
    //}

    public static class Db<T> where T : IDbReader
    {
        public static DbQuery<T> Query(DataSource ds)
        {
            if (ds == null)
                throw new ArgumentNullException("ds");
            return new DbQuery<T>(ds);
        }
    }
}
