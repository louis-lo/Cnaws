using System;
using System.Collections.Generic;

namespace Cnaws.Data.Query
{
    internal enum DbWhereUnionType
    {
        And,
        Or
    }

    public sealed class DbWhereQueue
    {
        private List<object> _queue;

        public DbWhereQueue()
            : this(new DbWhere())
        {
        }
        public DbWhereQueue(string name, object value, DbWhereType type = DbWhereType.Equal)
            : this(new DbWhere(name, value, type))
        {
        }
        public DbWhereQueue(DbWhere where)
        {
            AddQueue(where);
        }
        internal DbWhereQueue(DbWhere l, DbWhere r, DbWhereUnionType type)
        {
            AddQueue(l, r, type);
        }
        internal DbWhereQueue(DbWhere l, DbWhereQueue r, DbWhereUnionType type)
        {
            AddQueue(l, r, type);
        }
        private DbWhereQueue(DbWhereQueue l, DbWhere r, DbWhereUnionType type)
        {
            AddQueue(l, r, type);
        }
        private DbWhereQueue(DbWhereQueue l, DbWhereQueue r, DbWhereUnionType type)
        {
            AddQueue(l, r, type);
        }

        private static string GetUnionTypeString(DbWhereUnionType type)
        {
            if (type == DbWhereUnionType.Or)
                return " OR ";
            return " AND ";
        }

        private void AddQueue(DbWhere where)
        {
            _queue = new List<object>(1);
            _queue.Add(where);
        }
        private void AddQueue(object l, object r, DbWhereUnionType type)
        {
            _queue = new List<object>(5);
            _queue.Add("(");
            _queue.Add(l);
            _queue.Add(GetUnionTypeString(type));
            _queue.Add(r);
            _queue.Add(")");
        }
        private void AddQueue(object value, DbWhereUnionType type)
        {
            _queue.Insert(0, "(");
            _queue.Add(GetUnionTypeString(type));
            _queue.Add(value);
            _queue.Add(")");
        }

        private void Add(DbWhere where, DbWhereUnionType type)
        {
            AddQueue(where, type);
        }
        private void Add(DbWhereQueue queue, DbWhereUnionType type)
        {
            AddQueue(queue, type);
        }

        internal DbQueryBuilder Build(DataSource ds)
        {
            DbQueryBuilder builder = new DbQueryBuilder();
            foreach (object value in _queue)
            {
                if (value is DbWhere)
                    builder.Append(((DbWhere)value).Build(ds));
                else if (value is DbWhereQueue)
                    builder.Append(((DbWhereQueue)value).Build(ds));
                else
                    builder.Append((string)value);
            }
            return builder;
        }

        public static DbWhereQueue operator &(DbWhereQueue l, DbWhere r)
        {
            if (l == null)
                return r;
            l.Add(r, DbWhereUnionType.And);
            return l;
        }
        public static DbWhereQueue operator &(DbWhereQueue l, DbWhereQueue r)
        {
            if (l == null)
                return r;
            l.Add(r, DbWhereUnionType.And);
            return l;
        }
        public static DbWhereQueue operator |(DbWhereQueue l, DbWhere r)
        {
            if (l == null)
                return r;
            l.Add(r, DbWhereUnionType.Or);
            return l;
        }
        public static DbWhereQueue operator |(DbWhereQueue l, DbWhereQueue r)
        {
            if (l == null)
                return r;
            l.Add(r, DbWhereUnionType.Or);
            return l;
        }
    }
}
