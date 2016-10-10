using Cnaws.Web.Templates.Common;
using System;
using System.Collections;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class JsonTag : SimpleTag
    {
        private Tag _json;

        public JsonTag()
        {
            _json = null;
        }

        public Tag Json
        {
            get { return _json; }
            set { _json = value; }
        }

        private object ParseImpl(TemplateContext context)
        {
            base.InitRuntime(context);

            string json = null;
            if (_json is VariableTag)
                json = ((VariableTag)_json).Parse(context) as string;
            else if (_json is ReferenceTag)
                json = ((ReferenceTag)_json).Parse(context) as string;
            else if (_json is StringTag)
                json = ((StringTag)_json).ToString();
            if (json == null)
                return null;
            return (new Json.JsonReader(json)).ReadValue(context);
        }

        public override object Parse(TemplateContext context)
        {
            return ParseImpl(context);
        }

        public override object Parse(object baseValue, TemplateContext context)
        {
            return ParseImpl(context);
        }
    }
}
