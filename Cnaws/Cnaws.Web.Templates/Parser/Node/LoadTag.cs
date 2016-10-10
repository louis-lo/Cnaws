using System;
using System.IO;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class LoadTag : BlockTag
    {
        private Tag path;
        public Tag Path
        {
            get { return path; }
            set { path = value; }
        }

        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            object path = this.Path.Parse(context);
            LoadResource(path, context);
            return base.Parse(context);
        }

        public override void Parse(TemplateContext context, TextWriter write)
        {
            base.InitRuntime(context);

            object path = this.Path.Parse(context);
            LoadResource(path, context);
            base.Parse(context, write);
        }

        private void LoadResource(object path, TemplateContext context)
        {
            if (path != null)
            {
                if (string.IsNullOrEmpty(context.CurrentPath))
                {
                    this.TemplateContent = Resources.LoadResource(path.ToString(), context.Charset);
                }
                else
                {
                    this.TemplateContent = Resources.LoadResource(new string[] { System.IO.Path.GetDirectoryName(context.CurrentPath) }, path.ToString(), context.Charset);
                }
            }
        }
    }
}
