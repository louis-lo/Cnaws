using System;

namespace Cnaws.Web.Templates
{
    /// <summary>
    /// 引擎基类
    /// </summary>
    public interface IEngine
    {
        //void Render(TemplateContext context, TextWriter writer);

        /// <summary>
        /// 创建Template实现
        /// </summary>
        /// <returns></returns>
        ITemplate CreateTemplate(string path);
    }
}
