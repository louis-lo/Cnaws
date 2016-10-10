using mshtml;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cnaws.Html
{
    public abstract class HtmlElementCollection : IHtmlNode, ICollection, IEnumerable
    {
        public abstract int Count { get; }
        public abstract HtmlElement this[string id] { get; }
        public abstract HtmlElement this[int index] { get; }
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }
        object ICollection.SyncRoot
        {
            get { return this; }
        }
        public abstract void CopyTo(Array dest, int index);
        public abstract IEnumerator GetEnumerator();

        internal abstract void Add(HtmlElement value);
        internal abstract void Add(HtmlElementCollection value);

        public abstract HtmlElement GetElementById(string id);
        public abstract HtmlElementCollection GetElementsByName(string name);
        public abstract HtmlElementCollection GetElementsByTagName(string tagName);
        public abstract HtmlElementCollection GetElementsByClassName(string className);

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
    }
    internal sealed class MsHtmlElementCollection : HtmlElementCollection
    {
        private IHTMLElementCollection _collection;

        public MsHtmlElementCollection(IHTMLElementCollection collection)
        {
            _collection = collection;
        }

        public override int Count
        {
            get
            {
                if (_collection != null)
                    return _collection.length;
                return 0;
            }
        }
        public override HtmlElement this[string id]
        {
            get
            {
                if (_collection != null)
                {
                    IHTMLElement value = _collection.item(id, 0) as IHTMLElement;
                    if (value != null)
                        return new HtmlElement(value);
                }
                return null;
            }
        }
        public override HtmlElement this[int index]
        {
            get
            {
                if (_collection != null)
                {
                    IHTMLElement value = _collection.item(index, 0) as IHTMLElement;
                    if (value != null)
                        return new HtmlElement(value);
                }
                return null;
            }
        }
        public override void CopyTo(Array dest, int index)
        {
            int count = Count;
            for (int i = 0; i < count; ++i)
                dest.SetValue(this[i], index++);
        }
        public override IEnumerator GetEnumerator()
        {
            HtmlElement[] array = new HtmlElement[Count];
            ((ICollection)this).CopyTo(array, 0);
            return array.GetEnumerator();
        }

        internal override void Add(HtmlElement value)
        {
            throw new NotImplementedException();
        }
        internal override void Add(HtmlElementCollection value)
        {
            throw new NotImplementedException();
        }

        public override HtmlElement GetElementById(string id)
        {
            HtmlElement el;
            for (int i = 0; i < Count; ++i)
            {
                el = this[i];
                if (string.Equals(id, el.Id, StringComparison.OrdinalIgnoreCase))
                    return el;
                el = el.GetElementById(id);
                if (el != null)
                    return el;
            }
            return null;
        }
        public override HtmlElementCollection GetElementsByName(string name)
        {
            HtmlElement el;
            HtmlElementCollection els;
            HtmlElementCollection value = new NetHtmlElementCollection();
            for (int i = 0; i < Count; ++i)
            {
                el = this[i];
                if (string.Equals(el.Name, name, StringComparison.OrdinalIgnoreCase))
                    value.Add(el);
                els = el.GetElementsByName(name);
                if (els.Count > 0)
                    value.Add(els);
            }
            return value;
        }
        public override HtmlElementCollection GetElementsByClassName(string className)
        {
            HtmlElement el;
            HtmlElementCollection els;
            NetHtmlElementCollection value = new NetHtmlElementCollection();
            for (int i = 0; i < Count; ++i)
            {
                el = this[i];
                if (el.HasClassName(className))
                    value.Add(el);
                els = el.GetElementsByClassName(className);
                if (els.Count > 0)
                    value.Add(els);
            }
            return value;
        }
        public override HtmlElementCollection GetElementsByTagName(string tagName)
        {
            HtmlElement el;
            HtmlElementCollection els;
            NetHtmlElementCollection value = new NetHtmlElementCollection();
            for (int i = 0; i < Count; ++i)
            {
                el = this[i];
                if (string.Equals(el.TagName, tagName, StringComparison.OrdinalIgnoreCase))
                    value.Add(el);
                els = el.GetElementsByTagName(tagName);
                if (els.Count > 0)
                    value.Add(els);
            }
            return value;
        }
    }
    internal sealed class NetHtmlElementCollection : HtmlElementCollection
    {
        private List<HtmlElement> _list;

        public NetHtmlElementCollection()
        {
            _list = new List<HtmlElement>();
        }

        public override int Count
        {
            get { return _list.Count; }
        }
        public override HtmlElement this[int index]
        {
            get
            {
                try
                {
                    return _list[index];
                }
                catch (ArgumentOutOfRangeException)
                {
                    return null;
                }
            }
        }
        public override HtmlElement this[string id]
        {
            get { return GetElementById(id); }
        }
        public override void CopyTo(Array dest, int index)
        {
            _list.CopyTo((HtmlElement[])dest, index);
        }
        public override IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        internal override void Add(HtmlElement value)
        {
            _list.Add(value);
        }
        internal override void Add(HtmlElementCollection value)
        {
            if (value is NetHtmlElementCollection)
            {
                _list.AddRange(((NetHtmlElementCollection)value)._list);
            }
            else
            {
                for (int i = 0; i < value.Count; ++i)
                    _list.Add(value[i]);
            }
        }

        public override HtmlElement GetElementById(string id)
        {
            HtmlElement value;
            foreach (HtmlElement el in _list)
            {
                if (string.Equals(id, el.Id, StringComparison.OrdinalIgnoreCase))
                    return el;
                value = el.GetElementById(id);
                if (value != null)
                    return value;
            }
            return null;
        }
        public override HtmlElementCollection GetElementsByName(string name)
        {
            HtmlElementCollection els;
            HtmlElementCollection value = new NetHtmlElementCollection();
            foreach (HtmlElement el in _list)
            {
                if (string.Equals(el.Name, name, StringComparison.OrdinalIgnoreCase))
                    value.Add(el);
                els = el.GetElementsByName(name);
                if (els.Count > 0)
                    value.Add(els);
            }
            return value;
        }
        public override HtmlElementCollection GetElementsByClassName(string className)
        {
            HtmlElementCollection els;
            NetHtmlElementCollection value = new NetHtmlElementCollection();
            foreach (HtmlElement el in _list)
            {
                if (el.HasClassName(className))
                    value.Add(el);
                els = el.GetElementsByClassName(className);
                if (els.Count > 0)
                    value.Add(els);
            }
            return value;
        }
        public override HtmlElementCollection GetElementsByTagName(string tagName)
        {
            HtmlElementCollection els;
            NetHtmlElementCollection value = new NetHtmlElementCollection();
            foreach (HtmlElement el in _list)
            {
                if (string.Equals(el.TagName, tagName, StringComparison.OrdinalIgnoreCase))
                    value.Add(el);
                els = el.GetElementsByTagName(tagName);
                if (els.Count > 0)
                    value.Add(els);
            }
            return value;
        }
    }
}
