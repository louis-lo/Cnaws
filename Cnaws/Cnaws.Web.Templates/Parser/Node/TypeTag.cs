using System;
using System.IO;

namespace Cnaws.Web.Templates.Parser.Node
{
    /// <summary>
    /// 基本类型标签
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public abstract class TypeTag<T> : Tag
    {
        private T baseValue;

        public TypeTag()
        {
            baseValue = default(T);
        }
        public TypeTag(T value)
        {
            baseValue = value;
        }

        /// <summary>
        /// 值
        /// </summary>
        public T Value
        {
            get { return this.baseValue; }
            set { this.baseValue = value; }
        }

        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            return this.Value;
        }

        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <param name="write">TextWriter</param>
        public override void Parse(TemplateContext context, TextWriter write)
        {
            base.InitRuntime(context);

            write.Write(this.Value.ToString());
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
