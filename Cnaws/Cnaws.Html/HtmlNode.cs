using System;
using System.Text;

namespace Cnaws.Html
{
    public abstract class HtmlNode
    {
        private HtmlNodeType _type;
        private HtmlNode _parent;
        private HtmlNode _previous;
        private HtmlNode _next;

        protected HtmlNode(HtmlNodeType type)
        {
            _type = type;
            _parent = null;
            _previous = null;
            _next = null;
        }

        public HtmlNodeType Type
        {
            get { return _type; }
        }
        public virtual bool IsClosed
        {
            get { return true; }
        }
        public virtual bool AutoClosed
        {
            get { return false; }
        }
        public virtual string Name
        {
            get { return null; }
        }
        public HtmlNode ParentNode
        {
            get { return _parent; }
            internal set { _parent = value; }
        }
        public HtmlNodeList ChildNodes
        {
            get { return new HtmlNodeList(this); }
        }
        public HtmlNode PreviousSibling
        {
            get { return _previous; }
            internal set { _previous = value; }
        }
        public HtmlNode NextSibling
        {
            get { return _next; }
            internal set { _next = value; }
        }
        public HtmlAttributeCollection Attributes
        {
            get { return new HtmlAttributeCollection(this); }
        }
        public virtual HtmlAttribute FirstAttribute
        {
            get { return null; }
        }
        public virtual HtmlNode FirstChild
        {
            get { return null; }
        }
        public HtmlNode LastChild
        {
            get
            {
                HtmlNode last = null;
                HtmlNode first = FirstChild;
                while (first != null)
                {
                    last = first;
                    first = first.NextSibling;
                }
                return last;
            }
        }
        public virtual string InnerText
        {
            get { return null; }
        }
        public virtual string OuterHtml
        {
            get { return null; }
        }
        public virtual string InnerHtml
        {
            get { return null; }
        }

        internal virtual void AddAttribute(HtmlAttribute attr)
        {
        }
        internal virtual void AddChild(HtmlNode child)
        {
        }

        public override string ToString()
        {
            return OuterHtml;
        }
    }

    internal class HtmlTextNode : HtmlNode
    {
        private StringBuilder _text;

        public HtmlTextNode()
            : base(HtmlNodeType.Text)
        {
            _text = new StringBuilder();
        }
        protected HtmlTextNode(HtmlNodeType type)
            : base(type)
        {
            _text = new StringBuilder();
        }

        public override string InnerText
        {
            get { return _text.ToString(); }
        }
        public override string InnerHtml
        {
            get { return InnerText; }
        }
        public override string OuterHtml
        {
            get { return InnerHtml; }
        }

        public void Append(char c)
        {
            _text.Append(c);
        }
        public void Append(string s)
        {
            _text.Append(s);
        }
    }

    internal sealed class HtmlCommentNode : HtmlTextNode
    {
        public HtmlCommentNode()
            : base(HtmlNodeType.Comment)
        {
        }

        public override string OuterHtml
        {
            get { return string.Concat("<!--", base.OuterHtml, "-->"); }
        }
    }

    internal class HtmlSingleElement : HtmlNode
    {
        private string _name;
        private HtmlAttribute _attribute;

        public HtmlSingleElement(string name)
            : base(HtmlNodeType.Element)
        {
            _name = name;
            _attribute = null;
        }
        
        public override string Name
        {
            get { return _name; }
        }
        public override HtmlAttribute FirstAttribute
        {
            get { return _attribute; }
        }
        public override string OuterHtml
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('<').Append(Name).Append(GetAttributeString()).Append(" />");
                return sb.ToString();
            }
        }

        protected string GetAttributeString()
        {
            StringBuilder sb = new StringBuilder();
            HtmlAttribute attr = _attribute;
            while (attr != null)
            {
                sb.Append(' ').Append(attr.ToString());
                attr = attr.Next;
            }
            return sb.ToString();
        }

        internal override void AddAttribute(HtmlAttribute attr)
        {
            if (_attribute == null)
            {
                _attribute = attr;
            }
            else
            {
                HtmlAttribute temp = _attribute;
                while (temp.Next != null)
                    temp = temp.Next;
                temp.Next = attr;
            }
        }
    }
    internal class HtmlElement : HtmlSingleElement
    {
        private HtmlNode _child;

        public HtmlElement(string name)
            : base(name)
        {
            _child = null;
        }

        public override bool IsClosed
        {
            get { return false; }
        }
        public override bool AutoClosed
        {
            get
            {
                HtmlAutoCloseTag tag;
                return Enum.TryParse(Name, true, out tag);
            }
        }
        public override HtmlNode FirstChild
        {
            get { return _child; }
        }
        public override string InnerText
        {
            get
            {
                HtmlNode node = _child;
                StringBuilder sb = new StringBuilder();
                while (node != null)
                {
                    sb.Append(node.InnerText);
                    node = node.NextSibling;
                }
                return sb.ToString();
            }
        }
        public override string InnerHtml
        {
            get
            {
                HtmlNode node = _child;
                StringBuilder sb = new StringBuilder();
                while (node != null)
                {
                    sb.Append(node.InnerHtml);
                    node = node.NextSibling;
                }
                return sb.ToString();
            }
        }
        public override string OuterHtml
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('<').Append(Name).Append(GetAttributeString()).Append('>');
                sb.Append(InnerHtml);
                sb.Append("</").Append(Name).Append('>');
                return sb.ToString();
            }
        }

        internal override void AddChild(HtmlNode child)
        {
            if (_child == null)
            {
                _child = child;
            }
            else
            {
                HtmlNode temp = _child;
                while (temp.NextSibling != null)
                    temp = temp.NextSibling;
                temp.NextSibling = child;
            }
        }

        private enum HtmlSingleTag
        {
            Br,
            Hr,
            Img,
            Col,
            Link,
            Meta,
            Base,
            Input,
            Embed,
            Param
        }
        internal static HtmlNode Create(string name)
        {
            HtmlSingleTag tag;
            if (Enum.TryParse(name, true, out tag))
                return new HtmlSingleElement(name);
            return new HtmlElement(name);
        }
        private enum HtmlAutoCloseTag
        {
            P,
            Li,
            Dd,
            Dt,
            Td,
            Tr,
            Ul,
            Html,
            Body,
            TBody,
            THead,
            TFoot,
            Option,
            ColGroup
        }
    }
}
