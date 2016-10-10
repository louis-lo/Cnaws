using System;

namespace Cnaws.Html.XPath
{
    internal abstract class XPathExpression
    {
        private string _name;
        private string _value;

        protected XPathExpression(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
        }
        public string Value
        {
            get { return _value; }
        }

        public abstract bool Evaluation(IHtmlNode node);
    }
}
