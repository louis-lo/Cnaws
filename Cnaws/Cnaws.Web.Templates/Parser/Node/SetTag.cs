using System;
using System.IO;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class SetTag : BaseTag
    {
        private string _name;
        private Tag _value;
        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Tag Value
        {
            get { return _value; }
            set { _value = value; }
        }


        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            object value = this.Value.Parse(context);
            if (!context.TempData.SetValue(this.Name, value))
            {
                context.TempData.Push(this.Name, value);
            }
            return null;
        }

        public override void Parse(TemplateContext context, TextWriter write)
        {
            Parse(context);
        }
    }
}
