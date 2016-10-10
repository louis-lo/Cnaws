using System;
using System.IO;
using System.Text;

namespace Cnaws.Web.Templates.Parser.Node
{
    public abstract class BaseTag : Tag
    {
        public override string ToString()
        {
            if (this.LastToken != null && this.FirstToken != this.LastToken)
            {
                StringBuilder sb = new StringBuilder();
                Token t = this.FirstToken;
                sb.Append(t.ToString());
                while ((t = t.Next) != null && t != this.LastToken)
                {
                    sb.Append(t.ToString());
                }
                sb.Append(this.LastToken.ToString());
                return sb.ToString();
            }
            else
            {
                return this.FirstToken.ToString();
            }
        }

        public override void Parse(TemplateContext context, TextWriter write)
        {
            base.InitRuntime(context);

            write.Write(Parse(context));
        }
    }
}
