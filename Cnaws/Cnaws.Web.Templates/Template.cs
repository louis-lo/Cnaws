using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Cnaws.Web.Templates.Parser.Node;

namespace Cnaws.Web.Templates
{
    /// <summary>
    /// 模板实例类
    /// </summary>
    public class Template : BlockTag, ITemplate
    {
        private TemplateContext _context;

        /// <summary>
        /// TemplateContext
        /// </summary>
        public TemplateContext Context
        {
            get
            {
                return _context;
            }
            set { _context = value; }
        }

        ///// <summary>
        ///// Template
        ///// </summary>
        //public Template()
        //    : this(null)
        //{
        //}
        ///// <summary>
        ///// Template
        ///// </summary>
        ///// <param name="text">模板内容</param>
        //public Template(string text)
        //    : this(new TemplateContext(), text)
        //{
        //}
        /// <summary>
        /// Template
        /// </summary>
        /// <param name="context">TemplateContext 对象</param>
        /// <param name="text">模板内容</param>
        public Template(TemplateContext context, string text)
        {
            this._context = context;
            this.TemplateContent = text;
        }

        /// <summary>
        /// 模板解析结果呈现
        /// </summary>
        /// <param name="writer"></param>
        public virtual void Render(TextWriter writer)
        {
            try
            {
                base.Render(this.Context, writer);
            }
            catch (Exception e)
            {
                if (this.Context.ThrowExceptions)
                    throw e;
                else
                    this.Context.AddError(e);
            }
        }

        public virtual void Eval()
        {
            try
            {
                base.Eval(this.Context);
            }
            catch (Exception e)
            {
                if (this.Context.ThrowExceptions)
                    throw e;
                else
                    this.Context.AddError(e);
            }
        }

        /// <summary>
        /// 模板解析结果呈现
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            string document;

            using (StringWriter writer = new StringWriter())
            {
                Render(writer);
                document = writer.ToString();
            }

            return document;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Set(string key, object value)
        {
            Context.TempData[key] = value;
        }

        /// <summary>
        /// 批量设置数据
        /// </summary>
        /// <param name="dic">字典</param>
        public void Set(Dictionary<string, object> dic)
        {
            foreach (KeyValuePair<string, object> value in dic)
            {
                Set(value.Key, value.Value);
            }
        }

        ///// <summary>
        ///// 从指定的文件加载 Template
        ///// </summary>
        ///// <param name="filename">完整的本地文件路径</param>
        ///// <param name="encoding">编码</param>
        ///// <returns></returns>
        //public static Template FromFile(string filename, Encoding encoding)
        //{
        //    TemplateContext ctx = new TemplateContext();
        //    ctx.Charset = encoding;
        //    ctx.CurrentPath = Path.GetDirectoryName(filename);

        //    Template template = new Template(ctx, Resources.Load(filename, encoding));

        //    return template;
        //}
    }
}
