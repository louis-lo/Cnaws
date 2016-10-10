using System;

namespace Cnaws.Data.Query
{
    public sealed class DbJoinSelectQuery<T, A, B> : IDbJoinQuery<T, A, B> where T : IDbSelectQuery where A : IDbReader where B : IDbReader
    {
        private DbJoinQuery<T, A, B> _parent;
        private DbSelect[] _select;

        internal DbJoinSelectQuery(DbJoinQuery<T, A, B> parent, DbSelect[] select)
        {
            _parent = parent;
            _select = select;
            _parent.Refresh(this);
        }

        IDbJoinQuery<T, A, B> IDbJoinQuery<T, A, B>.Query
        {
            get { return this; }
        }
        DbJoinQuery<T, A, B> IDbJoinQuery<T, A, B>.Parent
        {
            get { return _parent; }
        }
        DbQueryBuilder IDbJoinQuery<T, A, B>.Build(DataSource ds, long top, bool join)
        {
            DbQueryBuilder builder = new DbQueryBuilder("SELECT");
            if (top > 0)
                builder.Append(" TOP ").Append(top);
            builder.Append(' ');
            if (_select == null || _select.Length == 0)
            {
                if (join)
                    builder.Append((new DbSelect<B>()).Build(ds));
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
            builder.Append(" FROM ").Append(ds.Provider.EscapeName(DbTable.GetTableName<B>()));
            return builder;
        }

        public DbJoinWhereQuery<DbJoinSelectQuery<T, A, B>, T, A, B> Where(DbWhereQueue queue)
        {
            return new DbJoinWhereQuery<DbJoinSelectQuery<T, A, B>, T, A, B>(this, queue);
        }
        public DbJoinGroupByQuery<DbJoinSelectQuery<T, A, B>, T, A, B> GroupBy(params DbGroupBy[] group)
        {
            return new DbJoinGroupByQuery<DbJoinSelectQuery<T, A, B>, T, A, B>(this, group);
        }
        public DbJoinQuery<T, A, B> Result()
        {
            return _parent;
        }
    }
}
