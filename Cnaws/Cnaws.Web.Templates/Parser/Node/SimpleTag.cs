using System;

namespace Cnaws.Web.Templates.Parser.Node
{
    public abstract class SimpleTag : BaseTag
    {
        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="baseValue">基本值</param>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public abstract object Parse(object baseValue, TemplateContext context);
    }
}
