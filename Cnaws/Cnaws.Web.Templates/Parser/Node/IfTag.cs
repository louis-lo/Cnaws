using System;

namespace Cnaws.Web.Templates.Parser.Node
{
    /// <summary>
    /// IF标题
    /// </summary>
    public class IfTag : BaseTag
    {
        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            for (int i = 0; i < this.Children.Count - 1; ++i) //最后面一个子对象为EndTag
            {
                if (this.Children[i].ToBoolean(context))
                {
                    return this.Children[i].Parse(context);
                }
            }
            return null;
        }
    }
}
