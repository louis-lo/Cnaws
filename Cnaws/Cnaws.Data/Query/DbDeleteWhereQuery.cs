using System;
using System.Collections.Generic;
using System.Text;

namespace Cnaws.Data.Query
{
    public sealed class DbDeleteWhereQuery<T> where T : IDbReader
    {
        private DbDeleteQuery<T> _query;
        private DbWhereQueue _where;

        internal DbDeleteWhereQuery(DbDeleteQuery<T> query, DbWhereQueue queue)
        {
            _query = query;
            _where = queue;
        }

        public int Execute()
        {
            StringBuilder sb = new StringBuilder();
            List<DataParameter> ps = new List<DataParameter>();
            sb.Append("DELETE FROM ");
            sb.Append(_query.Query.Provider.EscapeName(DbTable.GetTableName<T>()));
            if (_where != null)
            {
                DbQueryBuilder builder = _where.Build(_query.Query.DataSource);
                sb.Append(" WHERE ").Append(builder.Sql);
                ps.AddRange(builder.Parameters);
            }
            sb.Append(';');
            return _query.Query.DataSource.ExecuteNonQuery(sb.ToString(), ps.ToArray());
        }
    }
}
