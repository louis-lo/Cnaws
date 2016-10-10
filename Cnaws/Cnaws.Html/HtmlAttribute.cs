using mshtml;
using System;

namespace Cnaws.Html
{
    public sealed class HtmlAttribute
    {
        private string _name;
        private string _value;

        internal HtmlAttribute(HTMLDOMAttributeClass attribute)
        {
            
        }

        public string Name
        {
            get { return _name; }
        }
        public string Value
        {
            get { return _value; }
        }
    }
}
