using System;
using System.Collections.Generic;
using Cnaws.Web.Templates.Common;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class ExpressionTag : BaseTag
    {
        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            object[] value = new object[this.Children.Count];

            for (int i = 0; i < this.Children.Count; ++i)
            {
                value[i] = this.Children[i].Parse(context);
            }

            Stack<object> stack = Calculator.ProcessExpression(value);

            return Calculator.Calculate(stack);
        }
    }
}
