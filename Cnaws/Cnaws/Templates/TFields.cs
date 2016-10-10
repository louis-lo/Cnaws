using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cnaws.Templates
{
    public static class TFields<T>
    {
        public static readonly Dictionary<string, FieldInfo> Fields;

        static TFields()
        {
            if (TType<T>.IsPrimitive)
            {
                Fields = null;
            }
            else
            {
                Fields = new Dictionary<string, FieldInfo>();
                FieldInfo[] fs = TType<T>.Type.GetFields(BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                FieldInfo f;
                for (int i = 0; i < fs.Length; ++i)
                {
                    f = fs[i];
                    if (!f.IsNotSerialized)
                        Fields.Add(f.Name, f);
                }
            }
        }
    }

    public static class TAllNameSetAttFields<T, A> where A : Attribute, ICustomName
    {
        public static readonly Dictionary<string, KeyValuePair<FieldInfo, A>> Fields;

        static TAllNameSetAttFields()
        {
            if (TType<T>.IsPrimitive)
            {
                Fields = null;
            }
            else
            {
                Fields = new Dictionary<string, KeyValuePair<FieldInfo, A>>();
                FieldInfo[] fs = TType<T>.Type.GetFields(BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                FieldInfo f;
                for (int i = 0; i < fs.Length; ++i)
                {
                    f = fs[i];
                    if (!f.IsNotSerialized)
                    {
                        string name = f.Name;
                        A att = Attribute.GetCustomAttribute(f, TType<A>.Type) as A;
                        if (att != null)
                        {
                            if (!string.IsNullOrEmpty(att.Name))
                                name = att.Name;
                        }
                        Fields.Add(name, new KeyValuePair<FieldInfo, A>(f, att));
                    }
                }
            }
        }
    }

    public static class TAllSetAttFields<T, A> where A : Attribute
    {
        public static readonly Dictionary<string, KeyValuePair<FieldInfo, A>> Fields;

        static TAllSetAttFields()
        {
            if (TType<T>.IsPrimitive)
            {
                Fields = null;
            }
            else
            {
                Fields = new Dictionary<string, KeyValuePair<FieldInfo, A>>();
                FieldInfo[] fs = TType<T>.Type.GetFields(BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                FieldInfo f;
                for (int i = 0; i < fs.Length; ++i)
                {
                    f = fs[i];
                    if (!f.IsNotSerialized)
                    {
                        A att = Attribute.GetCustomAttribute(f, TType<A>.Type) as A;
                        Fields.Add(f.Name, new KeyValuePair<FieldInfo, A>(f, att));
                    }
                }
            }
        }
    }

    public static class TAllNameSetFields<T, A> where A : Attribute, ICustomName
    {
        public static readonly Dictionary<string, FieldInfo> Fields;

        static TAllNameSetFields()
        {
            if (TType<T>.IsPrimitive)
            {
                Fields = null;
            }
            else
            {
                Fields = new Dictionary<string, FieldInfo>();
                FieldInfo[] fs = TType<T>.Type.GetFields(BindingFlags.SetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                FieldInfo f;
                for (int i = 0; i < fs.Length; ++i)
                {
                    f = fs[i];
                    if (!f.IsNotSerialized)
                    {
                        string name = f.Name;
                        A att = Attribute.GetCustomAttribute(f, TType<A>.Type) as A;
                        if (att != null)
                        {
                            if (!string.IsNullOrEmpty(att.Name))
                                name = att.Name;
                        }
                        Fields.Add(name, f);
                    }
                }
            }
        }
    }

    public static class TAllNameGetFields<T, A> where A : Attribute, ICustomName
    {
        public static readonly List<KeyValuePair<string, FieldInfo>> Fields;

        static TAllNameGetFields()
        {
            if (TType<T>.IsPrimitive)
            {
                Fields = null;
            }
            else
            {
                Fields = new List<KeyValuePair<string, FieldInfo>>();
                FieldInfo[] fs = TType<T>.Type.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                FieldInfo f;
                for (int i = 0; i < fs.Length; ++i)
                {
                    f = fs[i];
                    if (!f.IsNotSerialized)
                    {
                        string name = f.Name;
                        A att = Attribute.GetCustomAttribute(f, TType<A>.Type) as A;
                        if (att != null)
                        {
                            if (!string.IsNullOrEmpty(att.Name))
                                name = att.Name;
                        }
                        Fields.Add(new KeyValuePair<string, FieldInfo>(name, f));
                    }
                }
            }
        }
    }
    public static class TAllNameGetAttFields<T, A> where A : Attribute, ICustomName
    {
        public static readonly Dictionary<string, KeyValuePair<FieldInfo, A>> Fields;

        static TAllNameGetAttFields()
        {
            if (TType<T>.IsPrimitive)
            {
                Fields = null;
            }
            else
            {
                Fields = new Dictionary<string, KeyValuePair<FieldInfo, A>>();
                FieldInfo[] fs = TType<T>.Type.GetFields(BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                FieldInfo f;
                for (int i = 0; i < fs.Length; ++i)
                {
                    f = fs[i];
                    if (!f.IsNotSerialized)
                    {
                        string name = f.Name;
                        A att = Attribute.GetCustomAttribute(f, TType<A>.Type) as A;
                        if (att != null)
                        {
                            if (!string.IsNullOrEmpty(att.Name))
                                name = att.Name;
                        }
                        Fields.Add(name, new KeyValuePair<FieldInfo, A>(f, att));
                    }
                }
            }
        }
    }
}
