using System;
using System.Text;
using System.Reflection;
using Cnaws.Web.Templates.Common;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class FunctaionTag : SimpleTag
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private object Excute(object value, TemplateContext context, object[] args, Type[] types)
        {
            if (value != null)
            {
                FuncHandler handler = value as FuncHandler;
                if (handler != null)
                    return handler.Invoke(args);

                if (value is ClrTag)
                    return ReflectionHelpers.InvokeMethod(ReflectionHelpers.GetMethod(context, ((ClrTag)value).TargetType, this.Name, types, true), null, args);

                return ReflectionHelpers.InvokeMethod(ReflectionHelpers.GetMethod(context, value.GetType(), this.Name, types, false), value, args);
            }

            return null;
        }

        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            object[] args = new object[this.Children.Count];
            Type[] argsType = new Type[this.Children.Count];
            for (int i = 0; i < this.Children.Count; ++i)
            {
                args[i] = this.Children[i].Parse(context);
                argsType[i] = args[i] != null ? args[i].GetType() : null;
            }

            return Excute(context.TempData[this.Name], context, args, argsType);
        }

        public override object Parse(object baseValue, TemplateContext context)
        {
            base.InitRuntime(context);

            if (baseValue != null)
            {
                object[] args = new object[this.Children.Count];
                Type[] argsType = new Type[this.Children.Count];
                for (int i = 0; i < this.Children.Count; ++i)
                {
                    args[i] = this.Children[i].Parse(context);
                    if (args[i] != null)
                    {
                        argsType[i] = args[i].GetType();
                    }
                    else
                    {

                    }
                }

                try { return Excute(baseValue, context, args, argsType); }
                catch (MethodNotFoundException) { }

                object result = ReflectionHelpers.GetPropertyValue(context, baseValue, this.Name);
                if (result != null && result is FuncHandler)
                    return (result as FuncHandler).Invoke(args);
            }

            return null;
        }
    }
}
