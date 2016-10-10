using System;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class StringTag : TypeTag<string>
    {
        public StringTag()
            : base()
        {
        }
        public StringTag(string value)
            : base(value)
        {
        }

        public override bool ToBoolean(TemplateContext context)
        {
            return !string.IsNullOrEmpty(this.Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
