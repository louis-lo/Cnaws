using System;
using System.Collections.Generic;
using Cnaws.Web.Templates.Common;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class VariableTag : SimpleTag
    {
        private string name;
        private List<Tag> index;

        public VariableTag()
        {
            index = new List<Tag>();
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<Tag> Index
        {
            get { return index; }
            set { index = value; }
        }

        internal static bool IsNumber(object value)
        {
            if (value is int) return true;
            if (value is long) return true;
            if (value is short) return true;
            if (value is uint) return true;
            if (value is ulong) return true;
            if (value is ushort) return true;
            if (value is byte) return true;
            if (value is sbyte) return true;
            return false;
        }

        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            if (this.Index.Count > 0)
            {
                Tag tag;
                object value = context.TempData[this.Name];
                for (int i = 0; i < this.Index.Count; ++i)
                {
                    tag = this.Index[i];
                    if (tag is VariableTag)
                    {
                        object key = ((VariableTag)tag).Parse(context);
                        if (key != null)
                        {
                            if (key is string)
                                value = ReflectionHelpers.Eval(context, value, (string)key);
                            else if (IsNumber(key))
                                value = ReflectionHelpers.Eval(context, value, key.ToString());
                            else
                                throw new ParseException("variable name type error");
                        }
                        else
                        {
                            throw new ParseException("variable name is null");
                        }
                    }
                    else if (tag is StringTag)
                    {
                        value = ReflectionHelpers.Eval(context, value, ((StringTag)tag).Value);
                    }
                    else if (tag is NumberTag)
                    {
                        value = ReflectionHelpers.Eval(context, value, ((NumberTag)tag).Value.ToString());
                    }
                    else
                    {
                        throw new ParseException("variable name type error");
                    }
                    if (value is Tag)
                        value = ((Tag)value).Parse(context);
                }
                return value;
            }
            return context.TempData[this.Name];
        }

        public override object Parse(object baseValue, TemplateContext context)
        {
            base.InitRuntime(context);

            if (this.Index.Count > 0)
            {
                Tag tag;
                object value = ReflectionHelpers.Eval(context, baseValue, this.Name);
                for (int i = 0; i < this.Index.Count; ++i)
                {
                    tag = this.Index[i];
                    if (tag is VariableTag)
                    {
                        object key = ((VariableTag)tag).Parse(baseValue, context);
                        if (key != null)
                        {
                            if (key is string)
                                value = ReflectionHelpers.Eval(context, value, (string)key);
                            else if (IsNumber(key))
                                value = ReflectionHelpers.Eval(context, value, key.ToString());
                            else
                                throw new ParseException("variable name type error");
                        }
                        else
                        {
                            throw new ParseException("variable name is null");
                        }
                    }
                    else if (tag is StringTag)
                    {
                        value = ReflectionHelpers.Eval(context, value, ((StringTag)tag).Value);
                    }
                    else if (tag is NumberTag)
                    {
                        value = ReflectionHelpers.Eval(context, value, ((NumberTag)tag).Value.ToString());
                    }
                    else
                    {
                        throw new ParseException("variable name type error");
                    }
                    if (value is Tag)
                        value = ((Tag)value).Parse(context);
                }
                return value;
            }

            return ReflectionHelpers.Eval(context, baseValue, this.Name);
        }
    }
}
