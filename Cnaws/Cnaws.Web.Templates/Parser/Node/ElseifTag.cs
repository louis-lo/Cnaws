using System;
using System.IO;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class ElseifTag : BaseTag
    {
        private Tag test;

        public virtual Tag Test
        {
            get { return test; }
            set { test = value; }
        }

        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            if (this.Children.Count == 1)
            {
                return this.Children[0].Parse(context);
            }
            else
            {
                using (StringWriter write = new StringWriter())
                {
                    for (int i = 0; i < this.Children.Count; ++i)
                    {
                        this.Children[i].Parse(context, write);
                    }
                    return write.ToString();
                }
            }
        }

        public override void Parse(TemplateContext context, TextWriter write)
        {
            base.InitRuntime(context);

            for (int i = 0; i < this.Children.Count; ++i)
            {
                this.Children[0].Parse(context, write);
            }
        }

        public override bool ToBoolean(TemplateContext context)
        {
            return this.Test.ToBoolean(context);
        }
    }
}
