using Cnaws.Templates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cnaws.Data.Linq
{
    internal sealed class QueryProvider : IQueryProvider
    {
        private static readonly QueryProvider _instance;

        static QueryProvider()
        {
            _instance = new QueryProvider();
        }
        private QueryProvider()
        {
        }

        public static QueryProvider Instance
        {
            get { return _instance; }
        }

        //private sealed class TElement<T>
        //{
        //    public static readonly Type Type;

        //    static TElement()
        //    {
        //        Type = GetElementType(TType<T>.Type);
        //    }

        //    private static Type GetElementType(Type type)
        //    {
        //        Type item = FindIEnumerable(type);
        //        if (item != null)
        //            return item.GetGenericArguments()[0];
        //        return type;
        //    }
        //    private static Type FindIEnumerable(Type type)
        //    {
        //        if (type != null && type != TType<string>.Type)
        //        {
        //            if (type.IsArray)
        //                return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());

        //            if (type.IsGenericType)
        //            {
        //                Type item;
        //                foreach (Type arg in type.GetGenericArguments())
        //                {
        //                    item = typeof(IEnumerable<>).MakeGenericType(arg);
        //                    if (item.IsAssignableFrom(type))
        //                        return item;
        //                }
        //            }

        //            Type[] ifaces = type.GetInterfaces();
        //            if (ifaces != null && ifaces.Length > 0)
        //            {
        //                Type item;
        //                foreach (Type iface in ifaces)
        //                {
        //                    item = FindIEnumerable(iface);
        //                    if (item != null)
        //                        return item;
        //                }
        //            }

        //            if (type.BaseType != null && type.BaseType != typeof(object))
        //                return FindIEnumerable(type.BaseType);
        //        }
        //        return null;
        //    }
        //}
        //private static Type GetElementType(Type type)
        //{
        //    Type gtype = typeof(TElement<>).MakeGenericType(type);
        //    FieldInfo gfield = gtype.GetField("Type", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
        //    return (Type)gfield.GetValue(null);
        //}

        public IQueryable CreateQuery(Expression expression)
        {
            //if (expression == null)
            //    throw new ArgumentNullException("expression");
            //Type elementType = GetElementType(expression.Type);
            //try
            //{
            //    return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), new object[] { this, expression });
            //}
            //catch (TargetInvocationException tie)
            //{
            //    throw tie.InnerException;
            //}
            throw new NotSupportedException();
        }
        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            if (TType<DbTable>.Type.IsAssignableFrom(TType<T>.Type))
            {
                if (expression != null)
                    return (Query<T>)Activator.CreateInstance(typeof(DbQuery<>).MakeGenericType(TType<T>.Type), BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, new object[] { expression }, CultureInfo.CurrentCulture);
                return (Query<T>)Activator.CreateInstance(typeof(DbQuery<>).MakeGenericType(TType<T>.Type), true);
            }
            if (expression != null)
                return new Query<T>(expression);
            return new Query<T>();
        }
        internal Query<A> CreateQueryEx<A, B>(Query<B> parent, Expression exp, DataSource ds)
        {
            Query<A> value;
            Type a = TType<A>.Type;
            Type b = TType<B>.Type;
            bool dba = TType<DbTable>.Type.IsAssignableFrom(a);
            bool dbb = TType<DbTable>.Type.IsAssignableFrom(b);
            if (dba && dbb)
                value = (Query<A>)Activator.CreateInstance(typeof(DbQuery<,>).MakeGenericType(a, b), true);
            else if (dba)
                value = (Query<A>)Activator.CreateInstance(typeof(DbQueryA<,>).MakeGenericType(a, b), true);
            else if (dbb)
                value = (Query<A>)Activator.CreateInstance(typeof(DbQueryB<,>).MakeGenericType(a, b), true);
            else
                value = (Query<A>)Activator.CreateInstance(typeof(Query<,>).MakeGenericType(a, b), true);
            value.SetExpression(exp);
            value.SetDataSource(ds);
            ((IQuery<B>)value).SetParent(parent);
            return value;
        }
        internal Query<R> CreateQueryEx<A, B, K, R>(Query<A> pa, Query<B> pb, Expression expa, Expression expb, Expression<Func<A, B, R>> result, DataSource ds)
        {
            Query<R> value;
            Type a = TType<A>.Type;
            Type b = TType<B>.Type;
            Type k = TType<K>.Type;
            Type r = TType<R>.Type;
            bool dba = TType<DbTable>.Type.IsAssignableFrom(a);
            bool dbb = TType<DbTable>.Type.IsAssignableFrom(b);
            if (dba && dbb)
                value = (Query<R>)Activator.CreateInstance(typeof(DbQuery<,,,>).MakeGenericType(a, b, k, r), true);
            else if (dba)
                value = (Query<R>)Activator.CreateInstance(typeof(DbQueryA<,,,>).MakeGenericType(a, b, k, r), true);
            else if (dbb)
                value = (Query<R>)Activator.CreateInstance(typeof(DbQueryB<,,,>).MakeGenericType(a, b, k, r), true);
            else
                value = (Query<R>)Activator.CreateInstance(typeof(Query<,,,>).MakeGenericType(a, b, k, r), true);
            value.SetExpression(result);
            value.SetDataSource(ds);
            ((IQuery<A, B>)value).SetParents(pa, pb, expa, expb);
            return value;
        }

        public object Execute(Expression expression)
        {
            throw new NotSupportedException();
        }

        public T Execute<T>(Expression expression)
        {
            throw new NotSupportedException();
        }
    }
}
