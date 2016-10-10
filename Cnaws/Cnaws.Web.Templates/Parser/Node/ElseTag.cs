using System;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class ElseTag : ElseifTag
    {
        public override bool ToBoolean(TemplateContext context)
        {
            return true;
        }
    }
}
