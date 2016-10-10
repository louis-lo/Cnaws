using System;
using System.IO;

namespace Cnaws.Web.Templates
{
    /// <summary>
    /// Template 基类
    /// </summary>
    public interface ITemplate
    {
        /// <summary>
        /// TemplateContext
        /// </summary>
        TemplateContext Context { get; set; }
        /// <summary>
        /// 模板内容
        /// </summary>
        string TemplateContent { get; set; }
        /// <summary>
        /// 结果呈现
        /// </summary>
        /// <param name="writer"></param>
        void Render(TextWriter writer);
    }
}
