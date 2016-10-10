using System;
using System.IO;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class ForTag : BaseTag
    {
        private Tag initial;
        private Tag test;
        private Tag dothing;

        /// <summary>
        /// 初始标签 
        /// </summary>
        public Tag Initial
        {
            get { return initial; }
            set { initial = value; }
        }

        /// <summary>
        /// 逻辑标签
        /// </summary>
        public Tag Test
        {
            get { return test; }
            set { test = value; }
        }

        /// <summary>
        /// Do
        /// </summary>
        public Tag Do
        {
            get { return dothing; }
            set { dothing = value; }
        }

        private void Excute(TemplateContext context, TextWriter writer)
        {
            this.Initial.Parse(context);
            //如果标签为空，则直接为false,避免死循环以内存溢出
            bool run;

            if (this.Test == null)
            {
                run = false;
            }
            else
            {
                run = this.Test.ToBoolean(context);
            }

            while (run)
            {
                for (int i = 0; i < this.Children.Count; ++i)
                {
                    this.Children[i].Parse(context, writer);
                }
                if (this.Do != null)
                {
                    this.Do.Parse(context);
                }
                run = this.Test == null ? true : this.Test.ToBoolean(context);
            }
        }

        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            using (StringWriter write = new StringWriter())
            {
                Excute(context, write);
                return write.ToString();
            }
        }

        public override void Parse(TemplateContext context, TextWriter write)
        {
            base.InitRuntime(context);

            Excute(context, write);
        }
    }
}
