using System;

namespace Cnaws.Html.XPath
{
    internal abstract class XPathItem
    {
        public abstract IHtmlNode Execute(IHtmlNode node, XPathExpression[] ex);
    }
}
