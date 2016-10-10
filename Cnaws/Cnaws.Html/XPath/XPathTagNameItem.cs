using System;

namespace Cnaws.Html.XPath
{
    internal sealed class XPathTagNameItem : XPathStringItem
    {
        public XPathTagNameItem(string tagName)
            : base(tagName)
        {
        }

        public override IHtmlNode Execute(IHtmlNode node, XPathExpression[] ex)
        {
            return node.GetElementsByTagName(Text);
        }
    }
}
