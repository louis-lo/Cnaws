using Cnaws.ExtensionMethods;
using Cnaws.Templates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Cnaws.Data.Linq
{
    internal interface IQuery<T>
    {
        void SetParent(Query<T> parent);
    }
    internal interface IQuery<A, B>
    {
        void SetParents(Query<A> a, Query<B> b, Expression expa, Expression expb);
    }

    public class Query<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
    {
        private Expression _expression;
        
        internal Query()
        {
            _expression = Expression.Constant(this);
        }
        internal Query(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (!TType<IQueryable<T>>.Type.IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");
            _expression = expression;
        }
        internal Query(Query<T> value)
        {
            Load(value);
        }

        internal virtual void Load(Query<T> value)
        {
            _expression = value._expression;
        }

        public static Query<T> Table(DataSource ds)
        {
            Query<T> query = (Query<T>)QueryProvider.Instance.CreateQuery<T>(null);
            query.SetDataSource(ds);
            return query;
        }

        public Type ElementType
        {
            get { return TType<T>.Type; }
        }
        public Expression Expression
        {
            get { return _expression; }
        }
        public IQueryProvider Provider
        {
            get { return QueryProvider.Instance; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<T> GetEnumerator()
        {
            throw new ArgumentOutOfRangeException();
        }

        protected static string GetExpressionTypeString(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.AndAlso: return "AND";
                case ExpressionType.OrElse: return "OR";
                case ExpressionType.Equal: return "=";
                case ExpressionType.NotEqual: return "<>";
                case ExpressionType.GreaterThan: return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan: return "<";
                case ExpressionType.LessThanOrEqual: return "<=";
            }
            return string.Empty;
        }
        protected static object GetMemberValue(MemberInfo member, object instance = null, object[] args = null)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field: return ((FieldInfo)member).GetValue(instance);
                case MemberTypes.Property: return ((PropertyInfo)member).GetValue(instance, args);
                default: throw new NotSupportedException("member");
            }
        }
        protected static Type GetMemberType(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field: return ((FieldInfo)member).FieldType;
                case MemberTypes.Property: return ((PropertyInfo)member).PropertyType;
                default: throw new NotSupportedException("member");
            }
        }
        protected static void SetMemberValue(MemberInfo member, object value, object instance = null, object[] args = null)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field: ((FieldInfo)member).SetValue(instance, value); break;
                case MemberTypes.Property: ((PropertyInfo)member).SetValue(instance, value, args); break;
                default: throw new NotSupportedException("member");
            }
        }
        protected static object OnLambda(Expression expression, object value)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    {
                        MemberExpression exp = (MemberExpression)expression;
                        if (exp.Expression.NodeType == ExpressionType.MemberAccess)
                            value = OnLambda(exp.Expression, value);
                        return GetMemberValue(exp.Member, value);
                    }
                case ExpressionType.New:
                    {
                        NewExpression exp = (NewExpression)expression;
                        if (value != null && exp.Type.IsAssignableFrom(value.GetType()))
                            return value;
                        object[] args = new object[exp.Arguments.Count];
                        for (int i = 0; i < exp.Arguments.Count; ++i)
                            args[i] = OnLambda(exp.Arguments[i], value);
                        return exp.Constructor.Invoke(args);
                    }
                case ExpressionType.Lambda:
                    {
                        return OnLambda(((LambdaExpression)expression).Body, value);
                    }
                case ExpressionType.Constant:
                    {
                        return ((ConstantExpression)expression).Value;
                    }
                case ExpressionType.Parameter:
                    {
                        if (value != null && ((ParameterExpression)expression).Type.IsAssignableFrom(value.GetType()))
                            return value;
                        return null;
                    }
                case ExpressionType.MemberInit:
                    {
                        MemberInitExpression exp = (MemberInitExpression)expression;
                        object v = OnLambda(exp.NewExpression, value);
                        foreach (MemberBinding mb in exp.Bindings)
                        {
                            switch (mb.BindingType)
                            {
                                case MemberBindingType.Assignment:
                                    {
                                        MemberAssignment ma = (MemberAssignment)mb;
                                        SetMemberValue(ma.Member, OnLambda(ma.Expression, value), v);
                                    }
                                    break;
                                default:
                                    Type t = mb.GetType();
                                    throw new ArgumentException("expression");
                            }
                        }
                        return v;
                    }
                default:
                    {
                        Type t = expression.GetType();
                        throw new ArgumentException("expression");
                    }
            }
            return value;
        }
        protected T Format<O>(O value)
        {
#if (DEBUG)
            if (value != null)
            {
                object v = OnLambda(Expression, value);
                return (T)v;
            }
#else
            if (value != null)
                return (T)OnLambda(Expression, value);
# endif
            return default(T);
        }
        protected IList<T> Format<O>(IList<O> ilist)
        {
            List<T> list = new List<T>(ilist.Count);
            foreach (O value in ilist)
                list.Add(Format(value));
            return list;
        }

        internal virtual void SetExpression(Expression expression)
        {
            _expression = expression;
        }
        internal virtual void SetDataSource(DataSource ds)
        {
        }
        internal virtual DataSource GetDataSource()
        {
            return null;
        }

        internal virtual Query<R> SelectImpl<R>(Expression<Func<T, R>> selector)
        {
            throw new ArgumentOutOfRangeException();
        }
        internal virtual Query<T> WhereImpl(Expression<Func<T, bool>> predicate)
        {
            throw new ArgumentOutOfRangeException();
        }
        internal virtual Query<T> GroupByImpl<K>(Expression<Func<T, K>> keySelector)
        {
            throw new ArgumentOutOfRangeException();
        }
        internal virtual Query<T> GroupByImpl<K, E>(Expression<Func<T, K>> keySelector, Expression<Func<T, E>> elementSelector)
        {
            throw new ArgumentOutOfRangeException();
        }
        internal virtual Query<T> OrderByImpl<K>(Expression<Func<T, K>> selector, DataSortType sort)
        {
            throw new ArgumentOutOfRangeException();
        }
        internal virtual Query<R> JoinImpl<O, K, R>(IEnumerable<O> inner, Expression<Func<T, K>> outerKey, Expression<Func<O, K>> innerKey, Expression<Func<T, O, R>> result)
        {
            throw new ArgumentOutOfRangeException();
        }

        internal virtual long ExecuteCount()
        {
            throw new ArgumentOutOfRangeException();
        }
        internal virtual T ExecuteSingleRow()
        {
            throw new ArgumentOutOfRangeException();
        }
        internal virtual IList<T> ExecuteReader(int size)
        {
            throw new ArgumentOutOfRangeException();
        }
        internal virtual IList<T> ExecutePage(long index, int size, out long count)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
    internal class DbQuery<T> : Query<T> where T : DbTable, new()
    {
        private const string PARAMNAME = "P";

        private DataSource _ds;
        private StringBuilder _select;
        private StringBuilder _where;
        private StringBuilder _orderby;
        private StringBuilder _groupby;
        private StringBuilder _join;
        private int _size;
        private long _index;
        private List<DataParameter> _params;
        private int _paramcount;

        internal DbQuery()
        {
            _ds = null;
            _select = new StringBuilder();
            _where = new StringBuilder();
            _orderby = new StringBuilder();
            _groupby = new StringBuilder();
            _join = new StringBuilder();
            _size = 0;
            _index = 1;
            _params = new List<DataParameter>();
            _paramcount = 0;
        }
        internal DbQuery(DbQuery<T> value)
        {
            Load(value);
        }

        internal override void Load(Query<T> v)
        {
            DbQuery<T> value = (DbQuery<T>)v;
            _ds = value._ds;
            _select = value._select;
            _where = value._where;
            _orderby = value._orderby;
            _groupby = value._groupby;
            _join = value._join;
            _size = value._size;
            _index = value._index;
            _params = value._params;
            _paramcount = value._paramcount;
        }

        //internal StringBuilder SelectText
        //{
        //    get { return _select; }
        //}
        //internal StringBuilder WhereText
        //{
        //    get { return _where; }
        //}
        //internal StringBuilder GroupByText
        //{
        //    get { return _groupby; }
        //}
        //internal StringBuilder OrderByText
        //{
        //    get { return _orderby; }
        //}
        //internal StringBuilder JoinText
        //{
        //    get { return _join; }
        //}
        
        private static string GetDbTableName(Type type)
        {
            DataTableAttribute att = Attribute.GetCustomAttribute(type, TType<DataTableAttribute>.Type) as DataTableAttribute;
            if (att != null && !string.IsNullOrEmpty(att.Name))
                return att.Name;
            return type.Name;
        }
        private string GetSqlText()
        {
            CheckSelectText();
            KeyValuePair<string, string> pair = _ds.Provider.GetTopOrLimit(_size, _index);
            return _ds.Provider.BuildSelectSqlImpl(pair.Key, _select.ToString(), _ds.Provider.EscapeName(GetDbTableName()), _join.ToString(), _where.ToString(), _groupby.ToString(), _orderby.ToString(), pair.Value, false);
        }
        private void CheckSelectText()
        {
            if (_select.Length == 0)
            {
                if (_groupby.Length > 0)
                    _select.Append(_groupby.ToString());
                else
                    _select.Append(_ds.Provider.EscapeName(GetDbTableName())).Append(".*");
            }
        }
        internal string GetDbTableName()
        {
            return GetDbTableName(TType<T>.Type);
        }
        private static string GetDbColumnName(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo field = (FieldInfo)member;
                DataColumnAttribute att = Attribute.GetCustomAttribute(field, TType<DataColumnAttribute>.Type) as DataColumnAttribute;
                if (att != null && !string.IsNullOrEmpty(att.Name))
                    return att.Name;
                return field.Name;
            }
            return member.Name;
        }
        private DataParameter CreateParameter(object value)
        {
            ++_paramcount;
            return new DataParameter(string.Concat(PARAMNAME, _paramcount), value);
        }

        //private unsafe bool HasColumn(StringBuilder sb, string column)
        //{
        //    string temp = sb.ToString();
        //    int l1 = temp.Length;
        //    int l2 = column.Length;
        //    if (l2 > l1)
        //        return false;
        //    fixed (char* s1 = temp)
        //    {
        //        char* p1 = s1;
        //        fixed (char* s2 = column)
        //        {
        //            char* p2 = s2;

        //            char* p3 = p1 + l1 - l2;
        //            for (char* p4 = p1; p4 != p3; ++p4)
        //            {
        //                if (*p4 == *p2)
        //                {
        //                    bool result = true;
        //                    for (int i = 1; i < l2; ++i)
        //                    {
        //                        if (*(p4 + i) != *(p2 + i))
        //                        {
        //                            result = false;
        //                            break;
        //                        }
        //                    }
        //                    if (result)
        //                        return true;
        //                }
        //            }
        //        }
        //    }
        //    return false;
        //}
        //private void AddColumn(StringBuilder sb, string column)
        //{
        //    if (column != null)// && !HasColumn(sb, column))
        //    {
        //        if (sb.Length > 0)
        //            sb.Append(',');
        //        sb.Append(column);
        //    }
        //}

        internal override void SetDataSource(DataSource ds)
        {
            _ds = ds;
        }
        //internal override DataSource GetDataSource()
        //{
        //    return _ds;
        //}

        internal void OnLambda(StringBuilder sb, Expression expression, object argument = null)
        {
            if (expression != null)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                        {
                            BinaryExpression exp = (BinaryExpression)expression;
                            sb.Append('(');
                            OnLambda(sb, exp.Left);
                            sb.Append(')');
                            sb.Append(' ');
                            sb.Append(GetExpressionTypeString(expression.NodeType));
                            sb.Append(' ');
                            sb.Append('(');
                            OnLambda(sb, exp.Right);
                            sb.Append(')');
                        }
                        break;
                    case ExpressionType.Equal:
                    case ExpressionType.NotEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                        {
                            BinaryExpression exp = (BinaryExpression)expression;
                            OnLambda(sb, exp.Left);
                            sb.Append(GetExpressionTypeString(expression.NodeType));
                            OnLambda(sb, exp.Right);
                        }
                        break;
                    case ExpressionType.MemberAccess:
                        {
                            MemberExpression exp = (MemberExpression)expression;
                            if (exp.Expression != null)
                            {
                                OnLambda(sb, exp.Expression, (argument as MemberInfo) ?? exp.Member);
                            }
                            else
                            {
                                DataParameter param = CreateParameter(GetMemberValue(exp.Member));
                                sb.Append(param.GetParameterName());
                                _params.Add(param);
                            }
                        }
                        break;
                    case ExpressionType.Constant:
                        {
                            ConstantExpression exp = (ConstantExpression)expression;
                            object value = exp.Value;
                            if (argument != null)
                                value = GetMemberValue((MemberInfo)argument, value);
                            DataParameter param = CreateParameter(value);
                            sb.Append(param.GetParameterName());
                            _params.Add(param);
                        }
                        break;
                    case ExpressionType.Parameter:
                        {
                            ParameterExpression exp = (ParameterExpression)expression;
                            Type type = exp.Type;
                            MemberInfo member = null;
                            if (argument != null && TType<MemberInfo>.Type.IsAssignableFrom(argument.GetType()))
                            {
                                member = (MemberInfo)argument;
                                if (type.IsGenericType)
                                    type = member.DeclaringType;
                                if (type.IsGenericType)
                                    type = GetMemberType(member);
                            }
                            if (member != null && member.DeclaringType.IsAssignableFrom(type))
                                sb.Append(_ds.Provider.EscapeName(GetDbTableName(type))).Append('.').Append(_ds.Provider.EscapeName(GetDbColumnName(member)));
                        }
                        break;
                    case ExpressionType.New:
                        {
                            NewExpression exp = (NewExpression)expression;
                            foreach (Expression e in exp.Arguments)
                            {
                                StringBuilder temp = new StringBuilder();
                                OnLambda(temp, e);
                                if (temp.Length > 0)
                                {
                                    if (sb.Length > 0)
                                        sb.Append(',');
                                    sb.Append(temp.ToString());
                                }
                            }
                        }
                        break;
                    case ExpressionType.Lambda:
                        {
                            LambdaExpression exp = (LambdaExpression)expression;
                            OnLambda(sb, exp.Body);
                        }
                        break;
                    case ExpressionType.MemberInit:
                        {
                            MemberInitExpression exp = (MemberInitExpression)expression;
                            OnLambda(sb, exp.NewExpression);
                            foreach (MemberBinding mb in exp.Bindings)
                            {
                                switch (mb.BindingType)
                                {
                                    case MemberBindingType.Assignment:
                                        {
                                            MemberAssignment ma = (MemberAssignment)mb;
                                            OnLambda(sb, ma.Expression);
                                        }
                                        break;
                                    default:
                                        Type t = mb.GetType();
                                        throw new ArgumentException("expression");
                                }
                            }
                        }
                        break;
                    default:
                        {
                            Type t = expression.GetType();
                            throw new ArgumentException("expression");
                        }
                }
            }
        }

        internal Query<A> BuildSelectText<A, B>(Expression exp, Query<B> self)
        {
            OnLambda(_select, exp);
            if (TType<B>.Type != TType<A>.Type)
                return QueryProvider.Instance.CreateQueryEx<A, B>(self, exp, _ds);
            return (Query<A>)(object)self;
        }
        internal override Query<R> SelectImpl<R>(Expression<Func<T, R>> selector)
        {
            return BuildSelectText<R, T>(selector, this);
        }
        internal void BuildWhereText(Expression exp)
        {
            OnLambda(_where, exp);
        }
        internal override Query<T> WhereImpl(Expression<Func<T, bool>> predicate)
        {
            BuildWhereText(predicate);
            return this;
        }
        internal void BuildGroupByText(Expression exp)
        {
            OnLambda(_groupby, exp);
        }
        internal override Query<T> GroupByImpl<K>(Expression<Func<T, K>> keySelector)
        {
            BuildGroupByText(keySelector);
            return this;
        }
        internal void BuildOrderByText(Expression exp, DataSortType sort)
        {
            if (_orderby.Length > 0)
                _orderby.Append(',');
            OnLambda(_orderby, exp);
            _orderby.Append(' ');
            _orderby.Append(sort.ToString().ToUpper());
        }
        internal override Query<T> OrderByImpl<K>(Expression<Func<T, K>> selector, DataSortType sort)
        {
            BuildOrderByText(selector, sort);
            return this;
        }
        internal void BuildJoinText(string table, Expression expa, Expression expb)
        {
            _join.Append("INNER JOIN ");
            _join.Append(_ds.Provider.EscapeName(table));
            _join.Append(" ON ");
            OnLambda(_join, expa);
            _join.Append(GetExpressionTypeString(ExpressionType.Equal));
            OnLambda(_join, expb);
        }
        internal override Query<R> JoinImpl<O, K, R>(IEnumerable<O> inner, Expression<Func<T, K>> outerKey, Expression<Func<O, K>> innerKey, Expression<Func<T, O, R>> result)
        {
            return QueryProvider.Instance.CreateQueryEx<T, O, K, R>(this, (Query<O>)QueryProvider.Instance.CreateQuery<O>(null), outerKey, innerKey, result, _ds);
        }

        private void SetPage(int size = 0, long index = 1)
        {
            _size = size;
            _index = index;
        }
        internal override long ExecuteCount()
        {
            return Convert.ToInt64(_ds.ExecuteScalar(_ds.Provider.BuildSelectCountSqlImpl(_ds.Provider.EscapeName(GetDbTableName()), _join.ToString(), _where.ToString(), _groupby.ToString()), _params.ToArray()));
        }
        internal override T ExecuteSingleRow()
        {
            SetPage();
            return _ds.ExecuteSingleRow<T>(GetSqlText(), _params.ToArray());
        }
        internal override IList<T> ExecuteReader(int size)
        {
            SetPage(size);
            return _ds.ExecuteReader<T>(GetSqlText(), _params.ToArray());
        }
        internal override IList<T> ExecutePage(long index, int size, out long count)
        {
            count = ExecuteCount();

            CheckSelectText();

            if (_ds.Provider.SupperLimit)
            {
                KeyValuePair<string, string> pair = _ds.Provider.GetTopOrLimit(size, index);
                return _ds.ExecuteReader<T>(_ds.Provider.BuildSelectSqlImpl(pair.Key, _select.ToString(), _ds.Provider.EscapeName(GetDbTableName()), _join.ToString(), _where.ToString(), _groupby.ToString(), _orderby.ToString(), pair.Value, false), _params.ToArray());
            }

            long half = count / 2;
            long lower = (index - 1) * size;
            long upper = index * size;

            return _ds.ExecuteReader<T>(_ds.Provider.BuildSplitPageSqlImpl(_select.ToString(), _ds.Provider.EscapeName(GetDbTableName()), _join.ToString(), _where.ToString(), _groupby.ToString(), _orderby.ToString(), half, lower, upper, count), _params.ToArray());
        }
    }
    internal class Query<A, B> : Query<A>, IQuery<B>
    {
        private Query<B> _parent;

        internal Query()
        {
            _parent = null;
        }
        internal Query(Query<A> a, Query<B> b)
            : base(a)
        {
            _parent = b;
        }

        protected Query<B> Parent
        {
            get { return _parent; }
        }

        public virtual void SetParent(Query<B> parent)
        {
            _parent = parent;
        }

        internal override long ExecuteCount()
        {
            return _parent.ExecuteCount();
        }
        internal override A ExecuteSingleRow()
        {
            return Format(_parent.ExecuteSingleRow());
        }
        internal override IList<A> ExecuteReader(int size)
        {
            return Format(_parent.ExecuteReader(size));
        }
        internal override IList<A> ExecutePage(long index, int size, out long count)
        {
            return Format(_parent.ExecutePage(index, size, out count));
        }
    }
    internal class DbQuery<A, B> : DbQuery<A>, IQuery<B> where A : DbTable, new() where B : DbTable, new()
    {
        private DbQuery<B> _parent;

        internal DbQuery()
        {
            _parent = null;
        }
        internal DbQuery(DbQuery<A> a, DbQuery<B> b)
            : base(a)
        {
            _parent = b;
        }

        public virtual void SetParent(Query<B> parent)
        {
            _parent = (DbQuery<B>)parent;
        }
        public DbQuery<B> Parent
        {
            get { return _parent; }
        }
    }
    internal class DbQueryA<A, B> : DbQuery<A>, IQuery<B> where A : DbTable, new()
    {
        protected Query<B> _parent;

        internal DbQueryA()
        {
            _parent = null;
        }
        internal DbQueryA(DbQuery<A> a, Query<B> b)
            : base(a)
        {
            _parent = b;
        }

        public virtual void SetParent(Query<B> parent)
        {
            _parent = parent;
        }
    }
    internal class DbQueryB<A, B> : Query<A>, IQuery<B> where B : DbTable, new()
    {
        protected DbQuery<B> _parent;

        internal DbQueryB()
        {
            _parent = null;
        }
        internal DbQueryB(Query<A> a, DbQuery<B> b)
            : base(a)
        {
            _parent = b;
        }

        public virtual void SetParent(Query<B> parent)
        {
            _parent = (DbQuery<B>)parent;
        }

        internal override Query<R> SelectImpl<R>(Expression<Func<A, R>> selector)
        {
            return _parent.BuildSelectText<R, A>(selector, this);
        }
        internal override Query<A> WhereImpl(Expression<Func<A, bool>> predicate)
        {
            _parent.BuildWhereText(predicate);
            return this;
        }
        internal override Query<A> GroupByImpl<K>(Expression<Func<A, K>> keySelector)
        {
            _parent.BuildGroupByText(keySelector);
            return this;
        }
        internal override Query<A> OrderByImpl<K>(Expression<Func<A, K>> selector, DataSortType sort)
        {
            _parent.BuildOrderByText(selector, sort);
            return this;
        }

        internal override long ExecuteCount()
        {
            return _parent.ExecuteCount();
        }
        internal override A ExecuteSingleRow()
        {
            return Format(_parent.ExecuteSingleRow());
        }
        internal override IList<A> ExecuteReader(int size)
        {
            return Format(_parent.ExecuteReader(size));
        }
        internal override IList<A> ExecutePage(long index, int size, out long count)
        {
            return Format(_parent.ExecutePage(index, size, out count));
        }
    }
    internal class Query<A, B, K, R> : Query<R>, IQuery<A, B>
    {
        protected Query<A, B> _query;

        internal Query()
        {
            _query = null;
        }

        public virtual void SetParents(Query<A> a, Query<B> b, Expression expa, Expression expb)
        {
            _query = new Query<A, B>(a, b);
        }
    }
    internal class DbQuery<A, B, K, R> : Query<R>, IQuery<A, B> where A : DbTable, new() where B : DbTable, new()
    {
        protected DbQuery<A, B> _query;

        internal DbQuery()
        {
            _query = null;
        }

        public virtual void SetParents(Query<A> a, Query<B> b, Expression expa, Expression expb)
        {
            _query = new DbQuery<A, B>((DbQuery<A>)a, (DbQuery<B>)b);
            _query.BuildJoinText(_query.Parent.GetDbTableName(), expa, expb);
        }

        internal override Query<O> SelectImpl<O>(Expression<Func<R, O>> selector)
        {
            return _query.BuildSelectText<O, R>(selector, this);
        }
        internal override Query<R> WhereImpl(Expression<Func<R, bool>> predicate)
        {
            _query.BuildWhereText(predicate);
            return this;
        }
        internal override Query<R> GroupByImpl<Y, E>(Expression<Func<R, Y>> keySelector, Expression<Func<R, E>> elementSelector)
        {
            _query.BuildGroupByText(keySelector);
            return this;
        }
        internal override Query<R> OrderByImpl<Y>(Expression<Func<R, Y>> selector, DataSortType sort)
        {
            _query.BuildOrderByText(selector, sort);
            return this;
        }

        internal override long ExecuteCount()
        {
            return _query.ExecuteCount();
        }
        internal override R ExecuteSingleRow()
        {
            return Format(_query.ExecuteSingleRow());
        }
        internal override IList<R> ExecuteReader(int size)
        {
            return Format(_query.ExecuteReader(size));
        }
        internal override IList<R> ExecutePage(long index, int size, out long count)
        {
            return Format(_query.ExecutePage(index, size, out count));
        }
    }
    internal class DbQueryA<A, B, K, R> : Query<R>, IQuery<A, B> where A : DbTable, new()
    {
        protected DbQueryA<A, B> _query;

        internal DbQueryA()
        {
            _query = null;
        }

        public virtual void SetParents(Query<A> a, Query<B> b, Expression expa, Expression expb)
        {
            _query = new DbQueryA<A, B>((DbQuery<A>)a, b);
        }
    }
    internal class DbQueryB<A, B, K, R> : Query<R>, IQuery<A, B> where B : DbTable, new()
    {
        protected DbQueryB<A, B> _query;

        internal DbQueryB()
        {
            _query = null;
        }

        public virtual void SetParents(Query<A> a, Query<B> b, Expression expa, Expression expb)
        {
            _query = new DbQueryB<A, B>(a, (DbQuery<B>)b);
        }
    }
}
