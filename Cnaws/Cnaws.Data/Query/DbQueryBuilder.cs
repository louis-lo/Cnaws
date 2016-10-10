using System;
using System.Collections.Generic;
using System.Text;

namespace Cnaws.Data.Query
{
    public class DbQueryBuilder
    {
        private StringBuilder _sql;
        private List<DataParameter> _parameters;

        public DbQueryBuilder()
        {
            _sql = new StringBuilder();
            _parameters = new List<DataParameter>();
        }
        internal DbQueryBuilder(string sql, params DataParameter[] ps)
            : this()
        {
            Append(sql);
            Append(ps);
        }

        public string Sql
        {
            get { return _sql.ToString(); }
        }
        public DataParameter[] Parameters
        {
            get { return _parameters.ToArray(); }
        }

        internal DbQueryBuilder Append(DbQueryBuilder builder)
        {
            if (builder._sql.Length > 0)
                _sql.Append(builder._sql.ToString());
            if (builder._parameters.Count > 0)
                _parameters.AddRange(builder._parameters);
            return this;
        }
        internal DbQueryBuilder Append(char value)
        {
            _sql.Append(value);
            return this;
        }
        internal DbQueryBuilder Append(int value)
        {
            _sql.Append(value);
            return this;
        }
        internal DbQueryBuilder Append(long value)
        {
            _sql.Append(value);
            return this;
        }
        internal DbQueryBuilder Append(string sql)
        {
            if (sql != null && sql.Length > 0)
                _sql.Append(sql);
            return this;
        }
        internal DbQueryBuilder Insert(string sql)
        {
            if (sql != null && sql.Length > 0)
                _sql.Insert(0, sql);
            return this;
        }
        internal DbQueryBuilder Append(params DataParameter[] ps)
        {
            if (ps != null && ps.Length > 0)
                _parameters.AddRange(ps);
            return this;
        }
    }
}
