using Cnaws.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cnaws.Templates
{
    public static class TType<T>
    {
        private static readonly Type _type;
        private static readonly bool _isPrimitive;

        static TType()
        {
            _type = typeof(T);
            _isPrimitive = _type.IsPrimitive || _type == typeof(string) || _type == typeof(Money);
        }

        public static Type Type
        {
            get { return _type; }
        }
        /// <summary>
        /// Boolean、Byte、SByte、Int16、UInt16、Int32、UInt32、Int64、UInt64、IntPtr、Char、Double、Single
        /// String
        /// </summary>
        public static bool IsPrimitive
        {
            get { return _isPrimitive; }
        }
    }
    public sealed class TypesInfo
    {
        private readonly bool _isItDict;
        private readonly bool _isIDict;
        private readonly bool _isItList;
        private readonly bool _isIList;
        private readonly Type[] _tDict;
        private readonly Type[] _tList;

        public TypesInfo(Type type)
        {
            FieldInfo gfield;
            Type gtype = typeof(TTypes<>).MakeGenericType(type);
            gfield = gtype.GetField("_isItDict", BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic);
            _isItDict = (bool)gfield.GetValue(null);
            gfield = gtype.GetField("_isIDict", BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic);
            _isIDict = (bool)gfield.GetValue(null);
            gfield = gtype.GetField("_isItList", BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic);
            _isItList = (bool)gfield.GetValue(null);
            gfield = gtype.GetField("_isIList", BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic);
            _isIList = (bool)gfield.GetValue(null);
            gfield = gtype.GetField("_tDict", BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic);
            _tDict = (Type[])gfield.GetValue(null);
            gfield = gtype.GetField("_tList", BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic);
            _tList = (Type[])gfield.GetValue(null);
        }

        public bool IsItDict
        {
            get { return _isItDict; }
        }
        public bool IsIDict
        {
            get { return _isIDict; }
        }
        public bool IsItList
        {
            get { return _isItList; }
        }
        public bool IsIList
        {
            get { return _isIList; }
        }
        public Type[] TDict
        {
            get { return _tDict; }
        }
        public Type[] TList
        {
            get { return _tList; }
        }
    }
    public static class TTypes<T>
    {
        private static readonly bool _isItDict;
        private static readonly bool _isIDict;
        private static readonly bool _isItList;
        private static readonly bool _isIList;
        private static readonly Type[] _tDict;
        private static readonly Type[] _tList;

        static TTypes()
        {
            Type type = TType<T>.Type;
            List<Type> types = new List<Type>();
            types.Add(type);
            types.AddRange(type.GetInterfaces());
            foreach (Type item in types)
            {
                if (item.IsITDictionary())
                {
                    _tDict = item.GetGenericArguments();
                    _isItDict = true;
                }
                if (item.IsIDictionary())
                {
                    _isIDict = true;
                }
                if (item.IsITList())
                {
                    _tList = item.GetGenericArguments();
                    _isItList = true;
                }
                if (item.IsIList())
                {
                    _isIList = true;
                }
            }
        }

        public static bool IsItDict
        {
            get { return _isItDict; }
        }
        public static bool IsIDict
        {
            get { return _isIDict; }
        }
        public static bool IsItList
        {
            get { return _isItList; }
        }
        public static bool IsIList
        {
            get { return _isIList; }
        }
        public static Type[] TDict
        {
            get { return _tDict; }
        }
        public static Type[] TList
        {
            get { return _tList; }
        }
    }
}
