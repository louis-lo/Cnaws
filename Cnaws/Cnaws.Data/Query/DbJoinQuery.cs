using System;
using System.Collections.Generic;
using System.Text;

namespace Cnaws.Data.Query
{
    internal enum DbJoinType
    {
        Inner,
        Left,
        Right
    }

    public enum DbJoinMode
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    public enum DbJoinUnionMode
    {
        And,
        Or
    }

    internal sealed class JoinEntry<A, B> where A : IDbReader where B : IDbReader
    {
        private DbColumn<A> Key = null;
        private DbColumn<B> Value = null;
        private DbJoinMode Mode = DbJoinMode.Equal;
        public DbJoinUnionMode UnionMode = DbJoinUnionMode.Or;

        public JoinEntry(DbColumn<A> a, DbColumn<B> b, DbJoinMode m, DbJoinUnionMode um)
        {
            Key = a;
            Value = b;
            Mode = m;
            UnionMode = um;
        }

        public string Build(DataSource ds, bool hasu)
        {
            string m;
            StringBuilder sb = new StringBuilder();
            if (hasu)
                sb.Append(UnionMode == DbJoinUnionMode.Or ? " OR" : " AND");
            sb.Append(' ');
            sb.Append(Key.Build(ds));
            switch (Mode)
            {
                case DbJoinMode.NotEqual: m = "<>"; break;
                case DbJoinMode.GreaterThan: m = ">"; break;
                case DbJoinMode.GreaterThanOrEqual: m = ">="; break;
                case DbJoinMode.LessThan: m = "<"; break;
                case DbJoinMode.LessThanOrEqual: m = "<="; break;
                default: m = "="; break;
            }
            sb.Append(m);
            sb.Append(Value.Build(ds));
            return sb.ToString();
        }
    }

    public sealed class DbJoinQuery<T, A, B> : IDbSelectQuery, IDbJoinQueryParent<T, A, B> where T : IDbSelectQuery where A : IDbReader where B : IDbReader
    {
        private T _query;
        private DbJoinType _type;
        private List<JoinEntry<A, B>> _on;
        private IDbJoinQuery<T, A, B> _sub;

        internal DbJoinQuery(T query, DbJoinType type, DbColumn<A> a, DbColumn<B> b, DbJoinMode m, DbJoinUnionMode um = DbJoinUnionMode.And)
        {
            _on = new List<JoinEntry<A, B>>();
            _query = query;
            _type = type;
            _on.Add(new JoinEntry<A, B>(a, b, m, um));
            _sub = null;
        }

        DbQuery IDbSelectQuery.Query
        {
            get { return _query.Query; }
        }

        private string GetTyypeString()
        {
            switch (_type)
            {
                case DbJoinType.Left: return " LEFT JOIN ";
                case DbJoinType.Right: return " RIGHT JOIN ";
                default: return " INNER JOIN ";
            }
        }
        private R Build<R>(DataSource ds, R builder) where R : DbQueryBuilder
        {
            int i = 0;
            builder.Append(GetTyypeString());
            if (_sub != null)
                builder.Append('(').Append(_sub.Build(ds, 0, false)).Append(") AS ");
            builder.Append(ds.Provider.EscapeName(DbTable.GetTableName<B>())).Append(" ON ");
            foreach (JoinEntry<A, B> on in _on)
                builder.Append(on.Build(ds, i++ > 0));
            return builder;
        }
        DbQueryBuilder IDbSelectQuery.Build(DataSource ds, long top, bool join)
        {
            return Build(ds, _query.Build(ds, top, true));
        }
        DbQueryBuilder IDbSelectQuery.BuildCount(DataSource ds, long top, bool join, ref bool group)
        {
            return Build(ds, _query.BuildCount(ds, top, join, ref group));
        }
        DbQueryRowNumberBuilder IDbSelectQuery.BuildRowNumber(DataSource ds, long top, bool join, string order, bool reverse)
        {
            return Build(ds, _query.BuildRowNumber(ds, top, true, order, reverse));
        }

        internal void Refresh(IDbJoinQuery<T, A, B> value)
        {
            _sub = value;
        }
        void IDbJoinQueryParent<T, A, B>.Refresh(IDbJoinQuery<T, A, B> value)
        {
            Refresh(value);
        }
        public DbJoinQuery<T, A, B> And(DbColumn<A> a, DbColumn<B> b, DbJoinMode m = DbJoinMode.Equal, DbJoinUnionMode um = DbJoinUnionMode.And)
        {
            _on.Add(new JoinEntry<A, B>(a, b, m, um));
            return this;
        }
        public DbJoinQuery<T, A, B> Or(DbColumn<A> a, DbColumn<B> b, DbJoinMode m = DbJoinMode.Equal, DbJoinUnionMode um = DbJoinUnionMode.And)
        {
            _on.Add(new JoinEntry<A, B>(a, b, m, um));
            return this;
        }
        public DbJoinSelectQuery<T, A, B> Select(params DbSelect[] select)
        {
            return new DbJoinSelectQuery<T, A, B>(this, select);
        }

        public DbSelectWhereQuery<DbJoinQuery<T, A, B>> Where(DbWhereQueue queue)
        {
            return new DbSelectWhereQuery<DbJoinQuery<T, A, B>>(this, queue);
        }
        public DbGroupByQuery<DbJoinQuery<T, A, B>> GroupBy(params DbGroupBy[] group)
        {
            return new DbGroupByQuery<DbJoinQuery<T, A, B>>(this, group);
        }
        public DbJoinQuery<DbJoinQuery<T, A, B>, C, D> InnerJoin<C, D>(DbColumn<C> left, DbColumn<D> right, DbJoinMode mode = DbJoinMode.Equal) where C : IDbReader where D : IDbReader
        {
            return new DbJoinQuery<DbJoinQuery<T, A, B>, C, D>(this, DbJoinType.Inner, left, right, mode);
        }
        public DbJoinQuery<DbJoinQuery<T, A, B>, C, D> LeftJoin<C, D>(DbColumn<C> left, DbColumn<D> right, DbJoinMode mode = DbJoinMode.Equal) where C : IDbReader where D : IDbReader
        {
            return new DbJoinQuery<DbJoinQuery<T, A, B>, C, D>(this, DbJoinType.Left, left, right, mode);
        }
        public DbJoinQuery<DbJoinQuery<T, A, B>, C, D> RightJoin<C, D>(DbColumn<C> left, DbColumn<D> right, DbJoinMode mode = DbJoinMode.Equal) where C : IDbReader where D : IDbReader
        {
            return new DbJoinQuery<DbJoinQuery<T, A, B>, C, D>(this, DbJoinType.Right, left, right, mode);
        }
        public DbOrderByQuery<DbJoinQuery<T, A, B>> OrderBy(params DbOrderBy[] order)
        {
            return new DbOrderByQuery<DbJoinQuery<T, A, B>>(this, order);
        }

        public long Count()
        {
            return (new DbCountQuery<DbJoinQuery<T, A, B>>(this)).Execute();
        }
        public object Single()
        {
            return (new DbSingleQuery<DbJoinQuery<T, A, B>>(this)).Execute();
        }
        public R Single<R>()
        {
            return (new DbSingleQuery<DbJoinQuery<T, A, B>>(this)).Execute<R>();
        }
        public dynamic First()
        {
            return (new DbFirstQuery<DbJoinQuery<T, A, B>>(this)).Execute();
        }
        public R First<R>() where R : IDbReader, new()
        {
            return (new DbFirstQuery<DbJoinQuery<T, A, B>>(this)).Execute<R>();
        }
        public IList<dynamic> ToList()
        {
            return (new DbToListQuery<DbJoinQuery<T, A, B>>(this)).Execute();
        }
        public IList<R> ToList<R>() where R : IDbReader, new()
        {
            return (new DbToListQuery<DbJoinQuery<T, A, B>>(this)).Execute<R>();
        }
        public object[] ToArray()
        {
            return (new DbToArrayQuery<DbJoinQuery<T, A, B>>(this)).Execute();
        }
        public R[] ToArray<R>()
        {
            return (new DbToArrayQuery<DbJoinQuery<T, A, B>>(this)).Execute<R>();
        }
        public IList<dynamic> ToList(int size, long page = 1)
        {
            return (new DbToListQuery<DbJoinQuery<T, A, B>>(this)).Execute(size, page);
        }
        public IList<R> ToList<R>(int size, long page = 1) where R : IDbReader, new()
        {
            return (new DbToListQuery<DbJoinQuery<T, A, B>>(this)).Execute<R>(size, page);
        }
        public IList<dynamic> ToList(int size, long page, out long count)
        {
            return (new DbToListQuery<DbJoinQuery<T, A, B>>(this)).Execute(size, page, out count);
        }
        public IList<R> ToList<R>(int size, long page, out long count) where R : IDbReader, new()
        {
            return (new DbToListQuery<DbJoinQuery<T, A, B>>(this)).Execute<R>(size, page, out count);
        }
    }
}
