using System;

namespace Cnaws.Web.Templates.Parser.Node
{
    /// <summary>
    /// 用于执于复杂的方法或变量
    /// 类似于
    /// $User.CreateDate.ToString("yyyy-MM-dd")
    /// $Db.Query().Result.Count
    /// </summary>
    public class ReferenceTag : SimpleTag
    {
        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            if (this.Children.Count > 0)
            {
                object result = this.Children[0].Parse(context);
                for (int i = 1; i < this.Children.Count; ++i)
                {
                    result = ((SimpleTag)this.Children[i]).Parse(result, context);
                }
                return result;
            }
            return null;
        }

        public override object Parse(object baseValue, TemplateContext context)
        {
            base.InitRuntime(context);

            object result = baseValue;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                result = ((SimpleTag)this.Children[i]).Parse(result, context);
            }
            return result;
        }
    }
}
