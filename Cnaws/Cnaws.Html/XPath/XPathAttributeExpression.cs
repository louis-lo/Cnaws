using System;

namespace Cnaws.Html.XPath
{
    internal sealed class XPathAttributeExpression : XPathExpression
    {
        public XPathAttributeExpression(string name, string value)
            : base(name, value)
        {
        }

        public override bool Evaluation(IHtmlNode node)
        {
            if (node is HtmlElement)
                return string.Equals(((HtmlElement)node).GetAttribute(Name), Value);
            return false;
        }
    }
}
