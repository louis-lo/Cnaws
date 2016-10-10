using System;

namespace Cnaws.Data.Query
{
    public sealed class DbSubSelectQuery<T, R> : IDbSubQuery<R> where T : IDbReader where R : IDbSubQueryParent<R>
    {
        private R _parent;
        private DbSelect[] _select;

        internal DbSubSelectQuery(R parent, params DbSelect[] select)
        {
            _parent = parent;
            _select = select;
            _parent.Refresh(this);
        }

        IDbSubQuery<R> IDbSubQuery<R>.Query
        {
            get { return this; }
        }
        R IDbSubQuery<R>.Parent
        {
            get { return _parent; }
        }
        DbQueryBuilder IDbSubQuery<R>.Build(DataSource ds, long top, bool join)
        {
            DbQueryBuilder builder = new DbQueryBuilder("SELECT");
            if (top > 0)
                builder.Append(" TOP ").Append(top);
            builder.Append(' ');
            if (_select == null || _select.Length == 0)
            {
                if (join)
                    builder.Append((new DbSelect<T>()).Build(ds));
                else
                    builder.Append((new DbSelect()).Build(ds));
            }
            else {
                for (int i = 0; i < _select.Length; ++i)
                {
                    if (i > 0)
                        builder.Append(',');
                    builder.Append(_select[i].Build(ds));
                }
            }
            builder.Append(" FROM ").Append(ds.Provider.EscapeName(DbTable.GetTableName<T>())).Append(" AS T").Append(ds.PsCount);
            return builder;
        }

        public DbSubWhereQuery<DbSubSelectQuery<T, R>, R> Where(DbWhereQueue queue)
        {
            return new DbSubWhereQuery<DbSubSelectQuery<T, R>, R>(this, queue);
        }
        public DbSubGroupByQuery<DbSubSelectQuery<T, R>, R> GroupBy(params DbGroupBy[] group)
        {
            return new DbSubGroupByQuery<DbSubSelectQuery<T, R>, R>(this, group);
        }
        public R Result()
        {
            return _parent;
        }
    }
}
