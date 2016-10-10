using System;
using System.IO;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class TextTag : Tag
    {
        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            return this.ToString();
        }

        public override void Parse(TemplateContext context, TextWriter write)
        {
            base.InitRuntime(context);

            write.Write(this.ToString());
        }

        public override string ToString()
        {
            return this.FirstToken.ToString();
        }
    }
}
