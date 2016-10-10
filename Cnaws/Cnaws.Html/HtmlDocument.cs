using System;
using mshtml;

namespace Cnaws.Html
{
    public sealed class HtmlDocument : IHtmlNode, IDisposable
    {
        private HTMLDocumentClass _doc;
        private bool _disposed;

        public HtmlDocument()
        {
            _doc = null;
            _disposed = false;
        }

        public void LoadHtml(string html)
        {
            Close();
            _doc = new HTMLDocumentClass();
            _doc.IHTMLDocument2_write(html);
        }

        public HtmlElementCollection All
        {
            get
            {
                if (_doc != null)
                {
                    IHTMLElementCollection value = _doc.IHTMLDocument2_all;
                    if (value != null)
                        return new MsHtmlElementCollection(value);
                }
                return new NetHtmlElementCollection();
            }
        }
        public HtmlElement Body
        {
            get
            {
                if (_doc != null)
                {
                    IHTMLElement value = _doc.IHTMLDocument2_body;
                    if (value != null)
                        return new HtmlElement(value);
                }
                return null;
            }
        }
        public string DefaultEncoding
        {
            get
            {
                if (_doc != null)
                    return _doc.IHTMLDocument2_defaultCharset;
                return null;
            }
        }
        public string Encoding
        {
            get
            {
                if (_doc != null)
                    return _doc.IHTMLDocument2_charset;
                return null;
            }
        }
        public HtmlElementCollection Images
        {
            get
            {
                if (_doc != null)
                {
                    IHTMLElementCollection value = _doc.IHTMLDocument2_images;
                    if (value != null)
                        return new MsHtmlElementCollection(value);
                }
                return new NetHtmlElementCollection();
            }
        }
        public HtmlElementCollection Links
        {
            get
            {
                if (_doc != null)
                {
                    IHTMLElementCollection value = _doc.IHTMLDocument2_links;
                    if (value != null)
                        return new MsHtmlElementCollection(value);
                }
                return new NetHtmlElementCollection();
            }
        }
        public string Title
        {
            get
            {
                if (_doc != null)
                    return _doc.IHTMLDocument2_title;
                return null;
            }
        }

        public HtmlElement DocumentElement
        {
            get
            {
                if (_doc != null)
                {
                    IHTMLElement value = _doc.IHTMLDocument3_documentElement;
                    if (value != null)
                        return new HtmlElement(value);
                }
                return null;
            }
        }

        public HtmlElement GetElementById(string id)
        {
            if (_doc != null)
            {
                IHTMLElement value = _doc.IHTMLDocument3_getElementById(id);
                if (value != null)
                    return new HtmlElement(value);
            }
            return null;
        }
        public HtmlElementCollection GetElementsByName(string name)
        {
            if (_doc != null)
            {
                IHTMLElementCollection value = _doc.IHTMLDocument3_getElementsByName(name);
                if (value != null)
                    return new MsHtmlElementCollection(value);
            }
            return new NetHtmlElementCollection();
        }
        public HtmlElementCollection GetElementsByTagName(string tagName)
        {
            if (_doc != null)
            {
                IHTMLElementCollection value = _doc.IHTMLDocument3_getElementsByTagName(tagName);
                if (value != null)
                    return new MsHtmlElementCollection(value);
            }
            return new NetHtmlElementCollection();
        }
        public HtmlElementCollection GetElementsByClassName(string className)
        {
            HtmlElementCollection all = All;
            NetHtmlElementCollection value = new NetHtmlElementCollection();
            foreach (HtmlElement el in all)
            {
                if (el.HasClassName(className))
                    value.Add(el);
            }
            return value;
        }

        public void Close()
        {
            if (_doc != null)
            {
                _doc.IHTMLDocument2_close();
                _doc = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Close();
                }
                _disposed = true;
            }
        }

        IHtmlNode IHtmlNode.GetElementById(string id)
        {
            return GetElementById(id);
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
            return GetElementsByClassName(className);
        }

        ~HtmlDocument()
        {
            Dispose(false);
        }
    }
}
