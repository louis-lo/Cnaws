using System;
using System.Collections;
using System.Collections.Generic;

namespace Cnaws.Data.Query
{
    //ExpressionType
    public enum DbWhereType
    {
        Unkown,
        Default,
        /// <summary>
        /// 等于
        /// </summary>
        Equal,
        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual,
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 大于或等于
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// 小于
        /// </summary>
        LessThan,
        /// <summary>
        /// 小于或等于
        /// </summary>
        LessThanOrEqual,
        Like,
        LikeBegin,
        LikeEnd,
        In
    }

    public class DbWhere : IDbSubQueryParent<DbWhere>
    {
        private string _name;
        private object _value;
        private DbWhereType _type;

        internal DbWhere()
        {
            _name = null;
            _value = null;
            _type = DbWhereType.Default;
        }
        public DbWhere(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            _name = name;
            _value = null;
            _type = DbWhereType.Unkown;
        }
        public DbWhere(string name, object value, DbWhereType type = DbWhereType.Equal)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            _name = name;
            _value = value;
            _type = type;
        }

        public string Name
        {
            get { return _name; }
        }
        public object Value
        {
            get { return _value; }
        }
        public DbWhereType Type
        {
            get { return _type; }
        }

        public override bool Equals(object obj)
        {
            DbWhere where = obj as DbWhere;
            if (ReferenceEquals(where, null))
                return false;
            if (!string.Equals(_name, where._name))
                return false;
            if (!Equals(_value, where._value))
                return false;
            if (_type != where._type)
                return false;
            return true;
        }
        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0) ^ (_value != null ? _value.GetHashCode() : 0) ^ _type.GetHashCode();
        }

        public void Like(object value)
        {
            if (_type != DbWhereType.Unkown)
                throw new ArgumentException();
            _value = value;
            _type = DbWhereType.Like;
        }
        public void LikeBegin(object value)
        {
            if (_type != DbWhereType.Unkown)
                throw new ArgumentException();
            _value = value;
            _type = DbWhereType.LikeBegin;
        }
        public void LikeEnd(object value)
        {
            if (_type != DbWhereType.Unkown)
                throw new ArgumentException();
            _value = value;
            _type = DbWhereType.LikeEnd;
        }
        public void In(IEnumerable value)
        {
            if (_type != DbWhereType.Unkown)
                throw new ArgumentException();
            _value = value;
            _type = DbWhereType.In;
        }
        public DbSubSelectQuery<T, DbWhere> InSelect<T>(params DbSelect[] select) where T : IDbReader
        {
            if (_type != DbWhereType.Unkown)
                throw new ArgumentException();
            _type = DbWhereType.In;
            return new DbSubSelectQuery<T, DbWhere>(this, select);
        }
        internal void Refresh(IDbSubQuery<DbWhere> value)
        {
            _value = value;
        }
        void IDbSubQueryParent<DbWhere>.Refresh(IDbSubQuery<DbWhere> value)
        {
            Refresh(value);
        }

        protected virtual string GetName(DataSource ds)
        {
            return ds.Provider.EscapeName(_name);
        }
        private DataParameter GetParameter(DataSource ds)
        {
            return GetParameter(ds, _value);
        }
        private DataParameter GetParameter(DataSource ds, object value)
        {
            return DbQuery.BuildParameter(ds, value);
        }
        internal DbQueryBuilder Build(DataSource ds)
        {
            if (_type == DbWhereType.Unkown)
                throw new ArgumentException();
            switch (_type)
            {
                case DbWhereType.Default:
                    {
                        return new DbQueryBuilder("1=1", null);
                    }
                case DbWhereType.Equal:
                    {
                        if (_value == null)
                            return new DbQueryBuilder(string.Concat(GetName(ds), " IS NULL"), null);
                        DataParameter p = GetParameter(ds);
                        return new DbQueryBuilder(string.Concat(GetName(ds), '=', p.GetParameterName()), p);
                    }
                case DbWhereType.NotEqual:
                    {
                        if (_value == null)
                            return new DbQueryBuilder(string.Concat("NOT ", GetName(ds), " IS NULL"), null);
                        DataParameter p = GetParameter(ds);
                        return new DbQueryBuilder(string.Concat(GetName(ds), "<>", p.GetParameterName()), p);
                    }
                case DbWhereType.GreaterThan:
                    {
                        if (_value == null)
                            throw new ArgumentNullException();
                        DataParameter p = GetParameter(ds);
                        return new DbQueryBuilder(string.Concat(GetName(ds), '>', p.GetParameterName()), p);
                    }
                case DbWhereType.GreaterThanOrEqual:
                    {
                        if (_value == null)
                            throw new ArgumentNullException();
                        DataParameter p = GetParameter(ds);
                        return new DbQueryBuilder(string.Concat(GetName(ds), ">=", p.GetParameterName()), p);
                    }
                case DbWhereType.LessThan:
                    {
                        if (_value == null)
                            throw new ArgumentNullException();
                        DataParameter p = GetParameter(ds);
                        return new DbQueryBuilder(string.Concat(GetName(ds), '<', p.GetParameterName()), p);
                    }
                case DbWhereType.LessThanOrEqual:
                    {
                        if (_value == null)
                            throw new ArgumentNullException();
                        DataParameter p = GetParameter(ds);
                        return new DbQueryBuilder(string.Concat(GetName(ds), "<=", p.GetParameterName()), p);
                    }
                case DbWhereType.Like:
                    {
                        if (_value == null)
                            throw new ArgumentNullException();
                        DataParameter p = GetParameter(ds, string.Concat('%', Convert.ToString(_value), '%'));
                        return new DbQueryBuilder(string.Concat(GetName(ds), " LIKE ", p.GetParameterName()), p);
                    }
                case DbWhereType.LikeBegin:
                    {
                        if (_value == null)
                            throw new ArgumentNullException();
                        DataParameter p = GetParameter(ds, string.Concat(Convert.ToString(_value), '%'));
                        return new DbQueryBuilder(string.Concat(GetName(ds), " LIKE ", p.GetParameterName()), p);
                    }
                case DbWhereType.LikeEnd:
                    {
                        if (_value == null)
                            throw new ArgumentNullException();
                        DataParameter p = GetParameter(ds, string.Concat('%', Convert.ToString(_value)));
                        return new DbQueryBuilder(string.Concat(GetName(ds), " LIKE ", p.GetParameterName()), p);
                    }
                case DbWhereType.In:
                    {
                        if (_value == null)
                            throw new ArgumentNullException();
                        IEnumerable e = _value as IEnumerable;
                        if (e != null)
                        {
                            DataParameter p;
                            List<string> vs = new List<string>();
                            List<DataParameter> ps = new List<DataParameter>();
                            IEnumerator array = e.GetEnumerator();
                            while (array.MoveNext())
                            {
                                p = GetParameter(ds, array.Current);
                                vs.Add(p.GetParameterName());
                                ps.Add(p);
                            }
                            return new DbQueryBuilder(string.Concat(GetName(ds), " IN (", string.Join(",", vs.ToArray()), ')'), ps.ToArray());
                        }
                        IDbSubQuery<DbWhere> q = _value as IDbSubQuery<DbWhere>;
                        if (q != null)
                        {
                            DbQueryBuilder builder = q.Build(ds, 0, false);
                            return new DbQueryBuilder(string.Concat(GetName(ds), " IN (", builder.Sql, ')'), builder.Parameters);
                        }
                        throw new NotSupportedException();
                    }
            }
            throw new NotSupportedException();
        }

        public static implicit operator DbWhere(string name)
        {
            return new DbWhere(name);
        }

        public static DbWhere operator ==(DbWhere where, object value)
        {
            if (where._type != DbWhereType.Unkown)
                throw new ArgumentException();
            where._value = value;
            where._type = DbWhereType.Equal;
            return where;
        }
        public static DbWhere operator !=(DbWhere where, object value)
        {
            if (where._type != DbWhereType.Unkown)
                throw new ArgumentException();
            where._value = value;
            where._type = DbWhereType.NotEqual;
            return where;
        }
        public static DbWhere operator >(DbWhere where, object value)
        {
            if (where._type != DbWhereType.Unkown)
                throw new ArgumentException();
            where._value = value;
            where._type = DbWhereType.GreaterThan;
            return where;
        }
        public static DbWhere operator >=(DbWhere where, object value)
        {
            if (where._type != DbWhereType.Unkown)
                throw new ArgumentException();
            where._value = value;
            where._type = DbWhereType.GreaterThanOrEqual;
            return where;
        }
        public static DbWhere operator <(DbWhere where, object value)
        {
            if (where._type != DbWhereType.Unkown)
                throw new ArgumentException();
            where._value = value;
            where._type = DbWhereType.LessThan;
            return where;
        }
        public static DbWhere operator <=(DbWhere where, object value)
        {
            if (where._type != DbWhereType.Unkown)
                throw new ArgumentException();
            where._value = value;
            where._type = DbWhereType.LessThanOrEqual;
            return where;
        }


        public static implicit operator DbWhereQueue(DbWhere value)
        {
            return new DbWhereQueue(value);
        }

        public static DbWhereQueue operator &(DbWhere l, DbWhere r)
        {
            return new DbWhereQueue(l, r, DbWhereUnionType.And);
        }
        public static DbWhereQueue operator &(DbWhere l, DbWhereQueue r)
        {
            return new DbWhereQueue(l, r, DbWhereUnionType.And);
        }
        public static DbWhereQueue operator |(DbWhere l, DbWhere r)
        {
            return new DbWhereQueue(l, r, DbWhereUnionType.Or);
        }
        public static DbWhereQueue operator |(DbWhere l, DbWhereQueue r)
        {
            return new DbWhereQueue(l, r, DbWhereUnionType.Or);
        }
    }

    public sealed class DbWhere<T> : DbWhere where T : IDbReader
    {
        public DbWhere(string name)
            : base(name)
        {
        }
        public DbWhere(string name, object value, DbWhereType type = DbWhereType.Equal)
            : base(name, value, type)
        {
        }

        protected override string GetName(DataSource ds)
        {
            return string.Concat(ds.Provider.EscapeName(DbTable.GetTableName<T>()), '.', base.GetName(ds));
        }
    }
}
