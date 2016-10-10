using System;
using System.Collections;
using Cnaws.Web.Templates.Common;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class ArrayTag : BaseTag, IEnumerable
    {
        private Hashtable _table;

        public ArrayTag()
        {
            _table = new Hashtable();
        }

        public int Count
        {
            get { return _table.Count; }
        }

        public object this[int index]
        {
            get { return _table[index]; }
        }
        public object this[string name]
        {
            get { return _table[name]; }
        }

        public void Set(int index, Tag value, TemplateContext context)
        {
            _table[index] = value.Parse(context);
        }
        public void Set(string name, Tag value, TemplateContext context)
        {
            _table[name] = value.Parse(context);
        }

        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            return _table;
        }

        public IEnumerator GetEnumerator()
        {
            return _table.GetEnumerator();
        }
    }
}
