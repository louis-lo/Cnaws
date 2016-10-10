using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cnaws.Templates
{
    public static class TMethods<T>
    {
        public static readonly Dictionary<string, List<MethodInfo>> Methods;

        static TMethods()
        {
            if (TType<T>.IsPrimitive)
            {
                Methods = null;
            }
            else
            {
                Methods = new Dictionary<string, List<MethodInfo>>();
                MethodInfo[] ps = TType<T>.Type.GetMethods(BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public);
                MethodInfo p;
                for (int i = 0; i < ps.Length; ++i)
                {
                    p = ps[i];
                    if (Methods.ContainsKey(p.Name))
                    {
                        Methods[p.Name].Add(p);
                    }
                    else
                    {
                        List<MethodInfo> list = new List<MethodInfo>();
                        list.Add(p);
                        Methods.Add(p.Name, list);
                    }
                }
            }
        }
    }
}
