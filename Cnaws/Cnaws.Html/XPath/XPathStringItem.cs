using System;

namespace Cnaws.Html.XPath
{
    internal abstract class XPathStringItem : XPathItem
    {
        private string _text;

        protected XPathStringItem(string text)
        {
            _text = text;
        }

        public string Text
        {
            get { return _text; }
        }
    }
}
