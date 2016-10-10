using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cnaws.Templates
{
    public static class TProperties<T>
    {
        public static readonly Dictionary<string, PropertyInfo> Properties;

        static TProperties()
        {
            if (TType<T>.IsPrimitive)
            {
                Properties = null;
            }
            else
            {
                Properties = new Dictionary<string, PropertyInfo>();
                PropertyInfo[] ps = TType<T>.Type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
                PropertyInfo p;
                for (int i = 0; i < ps.Length; ++i)
                {
                    p = ps[i];
                    Properties.Add(p.Name, p);
                }
            }
        }
    }
}
