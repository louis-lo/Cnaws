using System;
using System.Collections;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Dynamic;

namespace Cnaws.Web.Templates.Common
{
    /// <summary>
    /// 反射辅助类
    /// </summary>
    internal static class ReflectionHelpers
    {
        private static readonly char[] _expressionPartSeparator = new char[] { '.' };
        private static readonly char[] _indexExprEndChars = new char[] { ']', ')' };
        private static readonly char[] _indexExprStartChars = new char[] { '[', '(' };

        #region EVAL解析
        #region 4.0版本

        /// <summary>
        /// 获取索引值
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="expr">表达式</param>
        /// <returns></returns>
        public static object GetIndexedPropertyValue(TemplateContext context, object container, string expr)
        {
            if (container == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(expr))
            {
                return null;
            }

            int length = expr.IndexOfAny(_indexExprStartChars);
            int num = expr.IndexOfAny(_indexExprEndChars, length + 1);
            if (((length < 0) || (num < 0)) || (num == (length + 1)))
            {
                return null;
            }

            string propName = null;
            string propIndex = expr.Substring(length + 1, (num - length) - 1).Trim();
            if (length != 0)
            {
                propName = expr.Substring(0, length);
            }

            return GetIndexedProperty(context, container, propName, propIndex);
        }
        /// <summary>
        /// 获取索引值
        /// </summary>
        /// <param name="container">对象</param>
        /// <param name="propIndex">索引名称</param>
        /// <param name="isNumber">索引名称是否数字</param>
        /// <returns></returns>
        public static object GetIndexedProperty(TemplateContext context, object container, bool isNumber, object propIndex)
        {
            if (isNumber && (container as IList) != null)
                return ((IList)container)[(int)propIndex];
            PropertyInfo info = container.GetType().GetProperty("Item", BindingFlags.Public | BindingFlags.Instance, null, null, new Type[] { propIndex.GetType() }, null);
            if (info != null)
                return info.GetValue(container, new object[] { propIndex });
            if (context != null)
                throw new PropertyNotFoundException(context.CurrentPath, context.RuntimeTag);
            return null;
        }
        public static object GetIndexedProperty(TemplateContext context, object container, string propName, string propIndex)
        {
            bool flag = false;

            object value = null;

            if (propIndex.Length != 0)
            {
                if (((propIndex[0] == '"') && (propIndex[propIndex.Length - 1] == '"')) || ((propIndex[0] == '\'') && (propIndex[propIndex.Length - 1] == '\'')))
                {
                    value = propIndex.Substring(1, propIndex.Length - 2);
                }
                else if (char.IsDigit(propIndex[0]))
                {
                    int num;
                    flag = int.TryParse(propIndex, NumberStyles.Integer, CultureInfo.InvariantCulture, out num);
                    if (flag)
                    {
                        value = num;
                    }
                    else
                    {
                        value = propIndex;
                    }
                }
                else
                {
                    value = propIndex;
                }
            }
            if (value == null)
            {
                return null;
            }
            object property = null;

            if ((propName != null) && (propName.Length != 0))
            {
                property = GetPropertyValue(context, container, propName);
            }
            else
            {
                property = container;
            }
            if (property == null)
            {
                return null;
            }

            return GetIndexedProperty(context, property, flag, value);

        }

        #endregion

        public static string Eval(TemplateContext context, object container, string expression, string format)
        {
            object obj = Eval(context, container, expression);
            if ((obj == null) || (obj == DBNull.Value))
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(format))
            {
                return obj.ToString();
            }
            return string.Format(format, obj);
        }
        public static object Eval(TemplateContext context, object container, string expression)
        {
            if (expression == null)
            {
                return null;
            }
            expression = expression.Trim();
            if (expression.Length == 0)
            {
                return null;
            }
            if (container == null)
            {
                return null;
            }
            string[] expressionParts = expression.Split(_expressionPartSeparator);
            return Eval(context, container, expressionParts);
        }

        #region
        public static object Eval(TemplateContext context, object container, string[] expressionParts)
        {
            return Eval(context, container, expressionParts, 0, expressionParts.Length);
        }
        private static object Eval(TemplateContext context, object container, string[] expressionParts, int start, int end)
        {
            string propName;
            object property = container;
            for (int i = start; (i < end) && (property != null); ++i)
            {
                propName = expressionParts[i];
                if (propName.IndexOfAny(_indexExprStartChars) < 0)
                    property = GetPropertyValue(context, property, propName);
                else
                    property = GetIndexedPropertyValue(context, property, propName);
            }
            return property;
        }
        #endregion

        private static class PropertyOrField<T>
        {
            public static readonly Dictionary<string, MemberInfo> All;

            static PropertyOrField()
            {
                All = new Dictionary<string, MemberInfo>();

                Type t = typeof(T);
                FieldInfo[] fs = t.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
                foreach (FieldInfo f in fs)
                {
                    try
                    {
                        All.Add(f.Name, f);
                    }
                    catch (ArgumentException)
                    {
                        MemberInfo tmp;
                        if (All.TryGetValue(f.Name, out tmp))
                        {
                            if (tmp.DeclaringType.IsAssignableFrom(f.DeclaringType))
                                All[f.Name] = f;
                        }
                    }
                }

                PropertyInfo[] ps = t.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
                foreach (PropertyInfo p in ps)
                {
                    try
                    {
                        All.Add(p.Name, p);
                    }
                    catch (ArgumentException)
                    {
                        MemberInfo tmp;
                        if (All.TryGetValue(p.Name, out tmp))
                        {
                            if (tmp.DeclaringType.IsAssignableFrom(p.DeclaringType))
                                All[p.Name] = p;
                        }
                    }
                }
            }
        }
        private static class StaticPropertyOrField<T>
        {
            public static readonly Dictionary<string, MemberInfo> All;

            static StaticPropertyOrField()
            {
                All = new Dictionary<string, MemberInfo>();

                Type t = typeof(T);
                FieldInfo[] fs = t.GetFields(BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
                foreach (FieldInfo f in fs)
                {
                    try
                    {
                        All.Add(f.Name, f);
                    }
                    catch (ArgumentException)
                    {
                        MemberInfo tmp;
                        if (All.TryGetValue(f.Name, out tmp))
                        {
                            if (tmp.DeclaringType.IsAssignableFrom(f.DeclaringType))
                                All[f.Name] = f;
                        }
                    }
                }

                PropertyInfo[] ps = t.GetProperties(BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.Public);
                foreach (PropertyInfo p in ps)
                {
                    try
                    {
                        All.Add(p.Name, p);
                    }
                    catch (ArgumentException)
                    {
                        MemberInfo tmp;
                        if (All.TryGetValue(p.Name, out tmp))
                        {
                            if (tmp.DeclaringType.IsAssignableFrom(p.DeclaringType))
                                All[p.Name] = p;
                        }
                    }
                }
            }
        }
        private static Dictionary<string, MemberInfo> GetPropertyOrField(Type type, bool shared)
        {
            Type gtype = (shared ? typeof(StaticPropertyOrField<>) : typeof(PropertyOrField<>)).MakeGenericType(type);
            FieldInfo gfield = gtype.GetField("All", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
            return (Dictionary<string, MemberInfo>)gfield.GetValue(null);
        }

        public static object GetPropertyValue(TemplateContext context, Type type, string propName)
        {
            return GetPropertyValueImpl(context, type, null, propName);
        }
        public static object GetPropertyValue(TemplateContext context, object container, string propName)
        {
            if (container == null)
                return null;
            return GetPropertyValueImpl(context, container.GetType(), container, propName);
        }
        private sealed class MemberBinder : GetMemberBinder
        {
            public MemberBinder(string name, bool ignoreCase = false) 
                : base(name, ignoreCase)
            {
            }

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                return target;
            }
        }
        private static object GetPropertyValueImpl(TemplateContext context, Type type, object container, string propName)
        {
            if (string.IsNullOrEmpty(propName))
                return null;

            if (container != null && typeof(DynamicObject).IsAssignableFrom(type))
            {
                object v;
                DynamicObject d = (DynamicObject)container;
                if (d.TryGetMember(new MemberBinder(propName), out v))
                    return v;
                return null;
            }

            MemberInfo m;
            Dictionary<string, MemberInfo> all = GetPropertyOrField(type, container == null);
            if (all.TryGetValue(propName, out m))
            {
                if (m.MemberType == MemberTypes.Field)
                    return ((FieldInfo)m).GetValue(container);

                if (m.MemberType == MemberTypes.Property)
                    return ((PropertyInfo)m).GetValue(container);
            }
            else
            {
                if (container != null)
                {
                    int num;
                    object value;
                    bool flag = int.TryParse(propName, NumberStyles.Integer, CultureInfo.InvariantCulture, out num);
                    if (flag)
                        value = num;
                    else
                        value = propName;
                    return GetIndexedProperty(context, container, flag, value);
                }
            }
            return null;
        }

        #endregion

        #region

        public static MethodInfo GetMethod(TemplateContext context, Type type, string methodName, Type[] args, bool shared)
        {
            if (type == null)
                throw new MethodNotFoundException(context.CurrentPath, context.RuntimeTag);

            BindingFlags flags = BindingFlags.Public | BindingFlags.IgnoreCase;
            if (shared)
                flags |= BindingFlags.Static;
            else
                flags |= BindingFlags.Instance;

            MethodInfo method = null;
            if (args != null && Array.IndexOf(args, null) == -1)
                method = type.GetMethod(methodName, flags, null, args, null);

            if (method == null)
                method = type.GetMethod(methodName, flags);

            if (method == null && context != null)
                throw new MethodNotFoundException(context.CurrentPath, context.RuntimeTag);

            return method;
        }
        public static object InvokeMethod(MethodInfo method, object obj, object[] args)
        {
            ParameterInfo[] ps = method.GetParameters();
            if ((args == null && ps.Length > 0) || (args != null && args.Length < ps.Length))
            {
                object[] temp = new object[ps.Length];
                for (int i = 0; i < ps.Length; ++i)
                {
                    if (args != null && i < args.Length)
                        temp[i] = args[i];
                    else
                        temp[i] = ps[i].DefaultValue;
                }
                args = temp;
            }
            return method.Invoke(obj, args);
        }
        //public static object GetMethod(TemplateContext context, object container, string methodName, object[] args)
        //{
        //    Type[] types = new Type[args.Length];
        //    for (int i = 0; i < args.Length; ++i)
        //        types[i] = args[i].GetType();
        //    Type t = container.GetType();
        //    MethodInfo method = GetMethod(context, t, methodName, types);
        //    return method.Invoke(container, args);
        //}

        public static IEnumerable ToIEnumerable(object dataSource)
        {
            IListSource source;
            IEnumerable result;

            if (dataSource == null)
                return null;
            source = dataSource as IListSource;
            if ((source = dataSource as IListSource) != null)
            {
                IList list = source.GetList();
                if (!source.ContainsListCollection)
                {
                    return list;
                }
                if ((list != null) && (list is ITypedList))
                {
                    PropertyDescriptorCollection itemProperties = ((ITypedList)list).GetItemProperties(new PropertyDescriptor[0]);
                    if ((itemProperties == null) || (itemProperties.Count == 0))
                    {
                        return null;
                    }
                    PropertyDescriptor descriptor = itemProperties[0];
                    if (descriptor != null)
                    {
                        object component = list[0];
                        object value = descriptor.GetValue(component);
                        if ((value != null) && ((result = value as IEnumerable) != null))
                        {
                            return result;
                        }
                    }
                    return null;
                }
            }
            if ((result = dataSource as IEnumerable) != null)
            {
                return result;
            }
            return null;

        }

        #endregion
    }
}
