using System;
using System.IO;
using System.Collections;
using Cnaws.Web.Templates.Common;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class ForeachTag : BaseTag
    {
        private string name;
        private Tag source;
        
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Tag Source
        {
            get { return source; }
            set { source = value; }
        }

        private void Excute(object value, TemplateContext context, TextWriter writer)
        {
            IEnumerable enumerable = ReflectionHelpers.ToIEnumerable(value);
            TemplateContext ctx;
            if (enumerable != null)
            {
                IEnumerator ienum = enumerable.GetEnumerator();
                ctx = TemplateContext.CreateContext(context);
                int i = 0;
                while (ienum.MoveNext())
                {
                    ++i;
                    ctx.TempData[this.Name] = ienum.Current;
                    //为了兼容以前的用户 foreachIndex 保留
                    ctx.TempData["foreachIndex"] = i;
                    for (int n = 0; n < this.Children.Count; ++n)
                    {
                        this.Children[n].Parse(ctx, writer);
                    }
                }
            }
        }

        public override void Parse(TemplateContext context, TextWriter writer)
        {
            base.InitRuntime(context);

            if (this.Source != null)
            {
                Excute(this.Source.Parse(context), context, writer);
            }
        }

        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            using (StringWriter write = new StringWriter())
            {
                Excute(this.Source.Parse(context), context, write);
                return write.ToString();
            }
        }
    }
}
