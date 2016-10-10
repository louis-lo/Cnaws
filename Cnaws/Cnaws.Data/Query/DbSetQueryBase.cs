using System;

namespace Cnaws.Data.Query
{
    public abstract class DbSetQueryBase<T> : IDbUpdateQuery where T : IDbUpdateQuery
    {
        private T _query;
        private DbColumn _column;

        protected DbSetQueryBase(T query, DbColumn column)
        {
            if (column == null)
                throw new ArgumentNullException("column");
            _query = query;
            _column = column;
        }

        protected IDbUpdateQuery Query
        {
            get { return _query; }
        }

        DbQuery IDbUpdateQuery.Query
        {
            get { return _query.Query; }
        }
        DbQueryBuilder IDbUpdateQuery.Build(DataSource ds, ref int count)
        {
            string name = _column.Build(ds);
            DbQueryBuilder builder = _query.Build(ds, ref count);
            if (count++ > 0) builder.Append(',');
            builder.Append(name);
            builder.Append('=');
            OnBuild(ds, builder, name);
            return builder;
        }

        protected virtual void OnBuild(DataSource ds, DbQueryBuilder builder, string column)
        {
        }
    }
}
