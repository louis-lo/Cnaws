using System;
using System.Text;

namespace Cnaws.Data.Query
{
    public sealed class DbDeleteQuery<T> where T : IDbReader
    {
        private DbQuery<T> _query;

        internal DbDeleteQuery(DbQuery<T> query)
        {
            _query = query;
        }

        internal DbQuery<T> Query
        {
            get { return _query; }
        }

        public DbDeleteWhereQuery<T> Where(DbWhereQueue queue)
        {
            return new DbDeleteWhereQuery<T>(this, queue);
        }
        public int Execute()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM ");
            sb.Append(_query.Provider.EscapeName(DbTable.GetTableName<T>()));
            sb.Append(';');
            return _query.DataSource.ExecuteNonQuery(sb.ToString());
        }
    }
}
