using System;
using System.IO;
using System.Text;
using System.Collections.ObjectModel;

namespace Cnaws.Web.Templates.Parser.Node
{
    /// <summary>
    /// 标签基类
    /// </summary>
    public abstract class Tag
    {
        private Token first, last;
        private Tag parent;
        private Collection<Tag> children = new Collection<Tag>();

        /// <summary>
        /// 子标签
        /// </summary>
        public Collection<Tag> Children
        {
            get { return this.children; }
        }

        protected void InitRuntime(TemplateContext context)
        {
            context.SetRuntimeTag(this);
        }

        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public abstract object Parse(TemplateContext context);

        /// <summary>
        /// 解析结果
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <param name="write">TextWriter</param>
        public abstract void Parse(TemplateContext context, TextWriter write);

        /// <summary>
        /// 转换为 Boolean 
        /// </summary>
        /// <param name="context">TemplateContext</param>
        /// <returns></returns>
        public virtual bool ToBoolean(TemplateContext context)
        {
            object value = Parse(context);
            if (value == null)
                return false;
            switch (Convert.GetTypeCode(value))
            {
                case TypeCode.Boolean:
                    return (bool)value;
                case TypeCode.String:
                    return !string.IsNullOrEmpty((string)value);
                case TypeCode.UInt16:
                    return ((ushort)value) != 0;
                case TypeCode.UInt32:
                    return ((uint)value) != 0U;
                case TypeCode.UInt64:
                    return ((ulong)value) != 0UL;
                case TypeCode.Int16:
                    return ((short)value) != 0;
                case TypeCode.Int32:
                    return ((int)value) != 0;
                case TypeCode.Int64:
                    return ((long)value) != 0L;
                case TypeCode.Decimal:
                    return (decimal)value != 0;
                case TypeCode.Double:
                    return (double)value != 0.0;
                case TypeCode.Single:
                    return (float)value != 0.0F;
                default:
                    return value != null;
            }
        }

        /// <summary>
        /// 开始Token
        /// </summary>
        public Token FirstToken
        {
            get { return first; }
            set { first = value; }
        }
        /// <summary>
        /// 结束Token
        /// </summary>
        public Token LastToken
        {
            set { last = value; }
            get { return last; }
        }

        public Tag Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// 添加一个子标签
        /// </summary>
        /// <param name="node"></param>
        public void AddChild(Tag node)
        {
            node.parent = this;
            Children.Add(node);
        }

        public string ToCodeString()
        {
            Tag parent = this;
            while (parent.Parent != null)
                parent = parent.Parent;
            StringBuilder sb = new StringBuilder();
            Token token = parent.FirstToken;
            while (token != null && token != this.LastToken.Next)
            {
                sb.Append(token.Text);
                token = token.Next;
            }
            return sb.ToString();
        }
    }
}
