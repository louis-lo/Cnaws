using System;

namespace Cnaws.Data.Query
{
    public sealed class DbQueryRowNumberBuilder : DbQueryBuilder
    {
        private string _order;

        public DbQueryRowNumberBuilder()
            : base()
        {
            _order = null;
        }
        internal DbQueryRowNumberBuilder(string sql, params DataParameter[] ps)
            : base(sql, ps)
        {
            _order = null;
        }

        public string OrderBy
        {
            get { return _order; }
            set { _order = value; }
        }
    }
}
