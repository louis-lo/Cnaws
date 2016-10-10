using mshtml;
using System;
using System.Collections;

namespace Cnaws.Html
{
    public sealed class HtmlAttributeCollection : ICollection, IEnumerable
    {
        private IHTMLAttributeCollection _collection;

        internal HtmlAttributeCollection(IHTMLAttributeCollection collection)
        {
            _collection = collection;
        }

        public int Count
        {
            get
            {
                if (_collection != null)
                    return _collection.length;
                return 0;
            }
        }
        public string this[string name]
        {
            get
            {
                if (_collection != null)
                {
                    object value = _collection.item(name);
                    if (value != null)
                        return value.ToString();
                }
                return null;
            }
        }
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }
        object ICollection.SyncRoot
        {
            get { return this; }
        }
        void ICollection.CopyTo(Array dest, int index)
        {
            foreach(object item in _collection)
            {
                dest.SetValue(item, index++);
            }
        }
        public IEnumerator GetEnumerator()
        {
            HtmlAttribute[] array = new HtmlAttribute[Count];
            ((ICollection)this).CopyTo(array, 0);
            return array.GetEnumerator();
        }
    }
}
