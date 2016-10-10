using System;

namespace Cnaws.Html.XPath
{
    internal sealed class XPathIdItem : XPathStringItem
    {
        public XPathIdItem(string id)
            : base(id)
        {
        }

        public override IHtmlNode Execute(IHtmlNode node, XPathExpression[] ex)
        {
            return node.GetElementById(Text);
        }
    }
}
