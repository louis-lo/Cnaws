using System;

namespace Cnaws.Html.XPath
{
    internal sealed class XPathClassNameItem : XPathStringItem
    {
        public XPathClassNameItem(string className)
            : base(className)
        {
        }

        public override IHtmlNode Execute(IHtmlNode node, XPathExpression[] ex)
        {
            return node.GetElementsByClassName(Text);
        }
    }
}
