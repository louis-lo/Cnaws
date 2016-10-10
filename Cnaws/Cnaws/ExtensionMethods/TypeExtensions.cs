using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using Cnaws.Templates;
using System.Runtime.CompilerServices;

namespace Cnaws.ExtensionMethods
{
    public static class TypeExtensions
    {
        public static string ToString(this Type type, bool isFull = false)
        {
            StringBuilder r = new StringBuilder(string.Concat(type.FullName, ","), type.FullName.Length + type.Assembly.FullName.Length + 1);
            if (isFull) 
                r.Append(type.Assembly.FullName);
            else 
                r.Append(type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(',')));
            return r.ToString();
        }
        public static Type ToType(this string s, bool throwOnError = true)
        {
            return Type.GetType(s, throwOnError, true);
        }

        public static bool Equals(this Type type, TypeCode code)
        {
            return Types.Equals(type, code);
        }
        public static TypeCode GetTypeCode(this Type type)
        {
            return Type.GetTypeCode(type);
        }
        public static object GetDefaultValue(this Type type)
        {
            return Types.GetDefaultValue(type);
        }
        public static object GetObjectFromString(this Type type, string s)
        {
            return Types.GetObjectFromString(type, s);
        }
        public static bool IsIList(this Type type)
        {
            return Types.IsIList(type);
        }
        public static bool IsITList(this Type type)
        {
            return Types.IsITList(type);
        }
        public static bool IsIDictionary(this Type type)
        {
            return Types.IsIDictionary(type);
        }
        public static bool IsITDictionary(this Type type)
        {
            return Types.IsITDictionary(type);
        }
        public static bool IsAnonymousType(this Type type)
        {
            return Attribute.GetCustomAttribute(type, TType<CompilerGeneratedAttribute>.Type) != null;
        }

        public static Dictionary<string, FieldInfo> GetStaticFields(this Type type)
        {
            Type gtype = typeof(TFields<>).MakeGenericType(type);
            FieldInfo gfield = gtype.GetField("Fields", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
            return (Dictionary<string, FieldInfo>)gfield.GetValue(null);
        }
        public static Dictionary<string, KeyValuePair<FieldInfo, A>> GetStaticAllNameSetAttFields<A>(this Type type) where A : Attribute, ICustomName
        {
            Type gtype = typeof(TAllNameSetAttFields<,>).MakeGenericType(type, typeof(A));
            FieldInfo gfield = gtype.GetField("Fields", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
            return (Dictionary<string, KeyValuePair<FieldInfo, A>>)gfield.GetValue(null);
        }
        public static Dictionary<string, KeyValuePair<FieldInfo, A>> GetStaticAllSetAttFields<A>(this Type type) where A : Attribute
        {
            Type gtype = typeof(TAllSetAttFields<,>).MakeGenericType(type, typeof(A));
            FieldInfo gfield = gtype.GetField("Fields", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
            return (Dictionary<string, KeyValuePair<FieldInfo, A>>)gfield.GetValue(null);
        }
        public static Dictionary<string, FieldInfo> GetStaticAllNameSetFields<A>(this Type type) where A : Attribute, ICustomName
        {
            Type gtype = typeof(TAllNameSetFields<,>).MakeGenericType(type, typeof(A));
            FieldInfo gfield = gtype.GetField("Fields", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
            return (Dictionary<string, FieldInfo>)gfield.GetValue(null);
        }
        public static List<KeyValuePair<string, FieldInfo>> GetStaticAllNameGetFields<A>(this Type type) where A : Attribute, ICustomName
        {
            Type gtype = typeof(TAllNameGetFields<,>).MakeGenericType(type, typeof(A));
            FieldInfo gfield = gtype.GetField("Fields", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
            return (List<KeyValuePair<string, FieldInfo>>)gfield.GetValue(null);
        }
        public static Dictionary<string, KeyValuePair<FieldInfo, A>> GetStaticAllNameGetAttFields<A>(this Type type) where A : Attribute
        {
            Type gtype = typeof(TAllNameGetAttFields<,>).MakeGenericType(type, typeof(A));
            FieldInfo gfield = gtype.GetField("Fields", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
            return (Dictionary<string, KeyValuePair<FieldInfo, A>>)gfield.GetValue(null);
        }
        public static Dictionary<string, PropertyInfo> GetStaticProperties(this Type type)
        {
            Type gtype = typeof(TProperties<>).MakeGenericType(type);
            FieldInfo gfield = gtype.GetField("Properties", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
            return (Dictionary<string, PropertyInfo>)gfield.GetValue(null);
        }
        public static Dictionary<string, List<MethodInfo>> GetStaticMethods(this Type type)
        {
            Type gtype = typeof(TMethods<>).MakeGenericType(type);
            FieldInfo gfield = gtype.GetField("Methods", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
            return (Dictionary<string, List<MethodInfo>>)gfield.GetValue(null);
        }
    }
}
