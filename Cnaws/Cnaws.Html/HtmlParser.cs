using System;
using System.Text;

namespace Cnaws.Html
{
    internal abstract class HtmlParser
    {
        public abstract HtmlNode Parse(HtmlReader reader);
    }

    internal class HtmlElementParser : HtmlParser
    {
        public override HtmlNode Parse(HtmlReader reader)
        {
            if (reader.Current != '<')
                return null;

            if (!HtmlUtil.IsChar(reader.Peek()))
                return null;

            reader.Read();//skip <

            StringBuilder name = new StringBuilder();
            while (HtmlUtil.IsCharOrNumber(reader.Current))
                name.Append(reader.Read());

            HtmlNode node = HtmlElement.Create(name.ToString());

            if (reader.Current == ' ')
            {
                HtmlAttribute attr;
                while (ParseAttribute(reader, out attr))
                    node.AddAttribute(attr);
                reader.SkipWhiteSpace();
            }

            bool closed = false;
            if (reader.Current == '/')
                closed = true;

            if (reader.Current == '>')
            {
                if (closed || node.IsClosed)
                    return node;

                HtmlNode child;
                while (ParseChild(reader, out child))
                    node.AddChild(child);
                reader.SkipWhiteSpace();

                
            }

            throw new Exception();
        }
        private bool ParseAttribute(HtmlReader reader, out HtmlAttribute attr)
        {
            reader.SkipWhiteSpace();
            if (HtmlUtil.IsChar(reader.Current))
            {
                StringBuilder sb = new StringBuilder();

            }
            attr = null;
            return false;
        }
        private bool ParseChild(HtmlReader reader, out HtmlNode node)
        {
            reader.SkipWhiteSpace();
            if (HtmlUtil.IsChar(reader.Current))
            {
                StringBuilder sb = new StringBuilder();

            }
            node = null;
            return false;
        }
    }
}
