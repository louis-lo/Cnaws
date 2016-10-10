using System;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class NumberTag : TypeTag<ValueType>
    {
        public NumberTag()
            : base()
        {
        }
        public NumberTag(ValueType value)
            : base(value)
        {
        }
    }
}
