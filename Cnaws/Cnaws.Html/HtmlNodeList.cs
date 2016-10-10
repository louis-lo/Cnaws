using System;
using System.Collections;

namespace Cnaws.Html
{
    public sealed class HtmlNodeList : IEnumerable
    {
        private readonly HtmlNode _parent;

        internal HtmlNodeList(HtmlNode parent)
        {
            _parent = parent;
        }

        public int Count
        {
            get
            {
                int i = 0;
                HtmlNode current = _parent.FirstChild;
                while (current != null)
                {
                    current = current.NextSibling;
                    ++i;
                }
                return i;
            }
        }

        public HtmlNode this[int index]
        {
            get
            {
                int i = 0;
                HtmlNode current = root.FirstChild;
                while (current != null)
                {
                    if (i == index)
                        return current;
                    current = current.NextSibling;
                    ++i;
                }
                return null;
            }
        }

        private HtmlNode GetNextNode(HtmlNode node)
        {
            if (node == null)
                return root.FirstChild;
            return node.NextSibling;
        }

        public IEnumerator GetEnumerator()
        {
            if (root.FirstChild == null)
                return new HtmlEmptyNodeListEnumerator(this);
            return new HtmlNodeListEnumerator(this);
        }

        private sealed class HtmlNodeListEnumerator : IEnumerator
        {
            private readonly HtmlNodeList list;
            private HtmlNode current;

            internal HtmlNodeListEnumerator(HtmlNodeList list)
            {
                this.list = list;
                current = null;
            }

            public object Current
            {
                get { return current; }
            }

            public bool MoveNext()
            {
                current = list.GetNextNode(current);
                return current != null;
            }

            public void Reset()
            {
                current = null;
            }
        }
        private sealed class HtmlEmptyNodeListEnumerator : IEnumerator
        {
            internal HtmlEmptyNodeListEnumerator(HtmlNodeList list)
            {
            }

            public object Current
            {
                get { return null; }
            }

            public bool MoveNext()
            {
                return false;
            }

            public void Reset()
            {
            }
        }
    }
}
