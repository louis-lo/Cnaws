using System;
using System.Collections.Generic;
using System.Text;

namespace Cnaws.Data.Query
{
    public sealed class DbInsertQuery<T> : IDbSubQueryParent<DbInsertQuery<T>> where T : IDbReader
    {
        private DbQuery<T> _query;
        private DbColumn[] _insert;
        private IDbSubQuery<DbInsertQuery<T>> _subQuery;

        internal DbInsertQuery(DbQuery<T> query, params DbColumn[] columns)
        {
            _query = query;
            _insert = columns;
            _subQuery = null;
        }

        internal DbQuery<T> Query
        {
            get { return _query; }
        }

        void IDbSubQueryParent<DbInsertQuery<T>>.Refresh(IDbSubQuery<DbInsertQuery<T>> value)
        {
            _subQuery = value;
        }

        internal string GetNames()
        {
            if (_insert != null && _insert.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('(');
                int i = 0;
                foreach (DbColumn column in _insert)
                {
                    if (i++ > 0) sb.Append(',');
                    sb.Append(column.Build(_query.DataSource));
                }
                sb.Append(") ");
                return sb.ToString();
            }
            return string.Empty;
        }

        public bool Execute()
        {
            DbQueryBuilder builder = _subQuery.Build(_query.DataSource, 0, false);
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(_query.Provider.EscapeName(DbTable.GetTableName<T>()));
            sb.Append(' ');
            sb.Append(GetNames());
            sb.Append(builder.Sql);
            sb.Append(';');
            return DbTable.InsertImpl(_query.DataSource, sb.ToString(), builder.Parameters);
        }
        public bool Execute(string column, out long value)
        {
            DbQueryBuilder builder = _subQuery.Build(_query.DataSource, 0, false);
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(_query.Provider.EscapeName(DbTable.GetTableName<T>()));
            sb.Append(' ');
            sb.Append(GetNames());
            sb.Append(builder.Sql);
            sb.Append(_query.Provider.GetInsertSqlEnd(column));
            sb.Append(';');

            return DbTable.InsertImpl(_query.DataSource, sb.ToString(), out value, builder.Parameters);
        }
        public DbValuesQuery<T> Values(params object[] values)
        {
            return new DbValuesQuery<T>(this, values);
        }
        public DbSubSelectQuery<O, DbInsertQuery<T>> Select<O>(params DbSelect[] columns) where O : IDbReader
        {
            return new DbSubSelectQuery<O, DbInsertQuery<T>>(this, columns);
        }
    }
}
