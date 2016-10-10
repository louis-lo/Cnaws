using System;
using System.IO;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class NullTag : BaseTag
    {
        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            return null;
        }

        public override void Parse(TemplateContext context, TextWriter write)
        {
            base.InitRuntime(context);
        }

        public override bool ToBoolean(TemplateContext context)
        {
            return false;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
