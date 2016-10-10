using System;

namespace Cnaws.Html
{
    internal interface IHtmlNode
    {
        IHtmlNode GetElementById(string id);
        IHtmlNode GetElementsByName(string name);
        IHtmlNode GetElementsByTagName(string tagName);
        IHtmlNode GetElementsByClassName(string className);
    }
}
