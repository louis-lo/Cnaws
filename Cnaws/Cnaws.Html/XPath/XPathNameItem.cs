using System;

namespace Cnaws.Html.XPath
{
    internal sealed class XPathNameItem : XPathStringItem
    {
        public XPathNameItem(string name)
            : base(name)
        {
        }

        public override IHtmlNode Execute(IHtmlNode node, XPathExpression[] ex)
        {
            return node.GetElementsByName(Text);
        }
    }
}
