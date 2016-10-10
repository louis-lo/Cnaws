using Cnaws.Web.Templates.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cnaws.Web.Templates.Parser.Node
{
    public sealed class ClrTag : BaseTag
    {
        private static readonly Dictionary<string, ClrTag> _static;

        private string _name;
        private List<string> _list;
        private Type _type;

        static ClrTag()
        {
            _static = new Dictionary<string, ClrTag>();
        }
        public ClrTag()
        {
            _name = null;
            _list = new List<string>();
            _type = null;
        }
        private ClrTag(ClrTag parent, string name)
        {
            _name = GetKey(parent, name);
            _list = new List<string>(parent._list.Count + 1);
            _list.AddRange(parent._list);
            _list.Add(name);
            _type = null;
        }

        private static string GetKey(ClrTag tag, string name)
        {
            if (tag._name != null)
                return string.Concat(tag._name, '.', name);
            return name;
        }

        public Type TargetType
        {
            get
            {
                if (_type == null)
                {
                    if (_list.Count > 1)
                    {
                        _type = Type.GetType(_name, false, true);
                        if (_type == null)
                        {
                            for (int i = 1; i < _list.Count; ++i)
                            {
                                string[] dll = new string[_list.Count - i];
                                _list.CopyTo(0, dll, 0, dll.Length);
                                _type = Type.GetType(string.Concat(_name, ",", string.Join(".", dll)), false, true);
                                if (_type != null)
                                    break;
                            }
                        }
                    }
                }
                return _type;
            }
        }

        public object this[string name]
        {
            get
            {
                ClrTag tag;
                string key = GetKey(this, name);
                if (_static.TryGetValue(key, out tag))
                    return tag;
                tag = new ClrTag(this, name);
                _static[key] = tag;
                return tag;
            }
        }

        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            return this;
        }

        public override void Parse(TemplateContext context, TextWriter write)
        {
            base.InitRuntime(context);
        }
    }
}
