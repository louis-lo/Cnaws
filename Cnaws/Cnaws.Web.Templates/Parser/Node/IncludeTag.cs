using System;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class IncludeTag : BaseTag
    {
        private Tag path;

        public Tag Path
        {
            get { return path; }
            set { path = value; }
        }

        private string LoadResource(object path, TemplateContext context)
        {
            if (path != null)
            {
                string file = path.ToString();
                if (string.IsNullOrEmpty(context.CurrentPath))
                {
                    return Resources.LoadResource(file, context.Charset);
                }
                else
                {
                    return Resources.LoadResource(new string[] { System.IO.Path.GetDirectoryName(context.CurrentPath) }, file, context.Charset);
                }
            }
            return null;
        }

        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            object path = this.Path.Parse(context);
            return LoadResource(path.ToString(), context);
        }
    }
}
