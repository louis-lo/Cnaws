using System;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class BooleanTag : TypeTag<bool>
    {
        public BooleanTag()
            : base()
        {
        }
        public BooleanTag(bool value)
            : base(value)
        {
        }

        public override bool ToBoolean(TemplateContext context)
        {
            return this.Value;
        }
    }
}
