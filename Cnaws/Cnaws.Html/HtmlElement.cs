using mshtml;
using System;

namespace Cnaws.Html
{
    public sealed class HtmlElement : IHtmlNode
    {
        private IHTMLElement _element;

        internal HtmlElement(IHTMLElement element)
        {
            _element = element;
        }

        public HtmlElementCollection All
        {
            get
            {
                if (_element != null)
                {
                    IHTMLElementCollection value = _element.all as IHTMLElementCollection;
                    if (value != null)
                        return new MsHtmlElementCollection(value);
                }
                return new NetHtmlElementCollection();
            }
        }
        //public HtmlAttributeCollection Attributes
        //{
        //    get
        //    {
        //        if (_element != null)
        //        {
        //            IHTMLDOMNode node = _element as IHTMLDOMNode;
        //            if (node != null)
        //            {
        //                IHTMLAttributeCollection value = node.attributes as IHTMLAttributeCollection;
        //                if (value != null)
        //                    return new HtmlAttributeCollection(value);
        //            }
        //        }
        //        return null;
        //    }
        //}
        public HtmlElementCollection Children
        {
            get
            {
                if (_element != null)
                {
                    IHTMLElementCollection value = _element.children as IHTMLElementCollection;
                    if (value != null)
                        return new MsHtmlElementCollection(value);
                }
                return new NetHtmlElementCollection();
            }
        }
        public HtmlElement FirstChild
        {
            get
            {
                if (_element != null)
                {
                    IHTMLDOMNode node = _element as IHTMLDOMNode;
                    if (node != null)
                    {
                        IHTMLElement value = node.firstChild as IHTMLElement;
                        if (value != null)
                            return new HtmlElement(value);
                    }
                }
                return null;
            }
        }
        public HtmlElement LastChild
        {
            get
            {
                if (_element != null)
                {
                    IHTMLDOMNode node = _element as IHTMLDOMNode;
                    if (node != null)
                    {
                        IHTMLElement value = node.lastChild as IHTMLElement;
                        if (value != null)
                            return new HtmlElement(value);
                    }
                }
                return null;
            }
        }
        public string Id
        {
            get
            {
                if (_element != null)
                    return _element.id;
                return null;
            }
        }
        public string InnerHtml
        {
            get
            {
                if (_element != null)
                    return _element.innerHTML;
                return null;
            }
        }
        public string InnerText
        {
            get
            {
                if (_element != null)
                    return _element.innerText;
                return null;
            }
        }
        public string Name
        {
            get { return GetAttribute("name"); }
        }
        public HtmlElement PreviousSibling
        {
            get
            {
                if (_element != null)
                {
                    IHTMLDOMNode node = _element as IHTMLDOMNode;
                    if (node != null)
                    {
                        IHTMLElement value = node.previousSibling as IHTMLElement;
                        if (value != null)
                            return new HtmlElement(value);
                    }
                }
                return null;
            }
        }
        public HtmlElement NextSibling
        {
            get
            {
                if (_element != null)
                {
                    IHTMLDOMNode node = _element as IHTMLDOMNode;
                    if (node != null)
                    {
                        IHTMLElement value = node.nextSibling as IHTMLElement;
                        if (value != null)
                            return new HtmlElement(value);
                    }
                }
                return null;
            }
        }
        public string OuterHtml
        {
            get
            {
                if (_element != null)
                    return _element.outerHTML;
                return null;
            }
        }
        public string OuterText
        {
            get
            {
                if (_element != null)
                    return _element.outerText;
                return null;
            }
        }
        public string Style
        {
            get
            {
                if (_element != null)
                {
                    IHTMLStyle value = _element.style;
                    if (value != null)
                        return value.cssText;
                }
                return null;
            }
        }
        public string TagName
        {
            get
            {
                if (_element != null)
                    return _element.tagName;
                return null;
            }
        }

        public string GetAttribute(string name)
        {
            if (_element != null)
            {
                object value = _element.getAttribute(name, 0);
                if (value != null)
                    return value.ToString();
            }
            return null;
        }

        public HtmlElement GetElementById(string id)
        {
            HtmlElement el;
            foreach (HtmlElement child in Children)
            {
                if (string.Equals(child.Id, id, StringComparison.OrdinalIgnoreCase))
                    return child;
                el = child.GetElementById(id);
                if (el != null)
                    return el;
            }
            return null;
        }
        public HtmlElementCollection GetElementsByName(string name)
        {
            HtmlElementCollection els;
            HtmlElementCollection value = new NetHtmlElementCollection();
            foreach (HtmlElement child in Children)
            {
                if (string.Equals(child.Name, name, StringComparison.OrdinalIgnoreCase))
                    value.Add(child);
                els = child.GetElementsByName(name);
                if (els.Count > 0)
                    value.Add(els);
            }
            return value;
        }
        public HtmlElementCollection GetElementsByTagName(string tagName)
        {
            if (_element != null)
            {
                IHTMLElement2 el2 = _element as IHTMLElement2;
                if (el2 != null)
                {
                    IHTMLElementCollection value = el2.getElementsByTagName(tagName);
                    if (value != null)
                        return new MsHtmlElementCollection(value);
                }
            }
            return new NetHtmlElementCollection();
        }
        public HtmlElementCollection GetElementsByClassName(string className)
        {
            HtmlElementCollection els;
            HtmlElementCollection value = new NetHtmlElementCollection();
            foreach (HtmlElement child in Children)
            {
                if (child.HasClassName(className))
                    value.Add(child);
                els = child.GetElementsByClassName(className);
                if (els.Count > 0)
                    value.Add(els);
            }
            return value;
        }

        internal unsafe bool HasClassName(string className)
        {
            if (!string.IsNullOrEmpty(className))
            {
                string temp = GetAttribute("class");
                if (!string.IsNullOrEmpty(temp))
                {
                    string[] array = temp.Split(' ');
                    foreach (string s in array)
                    {
                        if (className.Equals(s, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
            }
            return false;
        }

        IHtmlNode IHtmlNode.GetElementById(string id)
        {
            return GetElementsByTagName(id);
        }
        IHtmlNode IHtmlNode.GetElementsByName(string name)
        {
            return GetElementsByName(name);
        }
        IHtmlNode IHtmlNode.GetElementsByTagName(string tagName)
        {
            return GetElementsByTagName(tagName);
        }
        IHtmlNode IHtmlNode.GetElementsByClassName(string className)
        {
            return GetElementsByTagName(className);
        }
    }
}
