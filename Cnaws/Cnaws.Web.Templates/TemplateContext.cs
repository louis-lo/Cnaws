using System;
using System.Text;
using Cnaws.Web.Templates.Parser;
using Cnaws.Web.Templates.Parser.Node;

namespace Cnaws.Web.Templates
{
    /// <summary>
    /// 方法标签委托
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate object FuncHandler(params object[] args);

    /// <summary>
    /// Context
    /// </summary>
    public class TemplateContext : ICloneable
    {
        private string currentPath;
        private Encoding charset;
        private bool throwErrors;
        //private string url;
        //private string res;
        private VariableScope variableScope;
        private Tag runtimeTag;
        
        /// <summary>
        /// Context
        /// </summary>
        public TemplateContext(string path, Encoding charset, bool throwErrors/*, string url, string res*/, VariableScope data)
        {
            this.currentPath = path;
            this.charset = charset;
            this.throwErrors = throwErrors;
            //this.url = url;
            //this.res = res;
            data.SetContext(this);
            this.variableScope = data;
            this.runtimeTag = null;
        }

        /// <summary>
        /// 当前资源路径
        /// </summary>
        public string CurrentPath
        {
            get { return currentPath; }
        }

        /// <summary>
        /// 当前资源编码
        /// </summary>
        public Encoding Charset
        {
            get { return charset; }
        }

        /// <summary>
        /// 是否抛出异常(默认为true)
        /// </summary>
        public bool ThrowExceptions
        {
            get { return throwErrors; }
        }

        //public string Url
        //{
        //    get { return url; }
        //}

        //public string Res
        //{
        //    get { return res; }
        //}

        /// <summary>
        /// 模板数据
        /// </summary>
        public VariableScope TempData
        {
            get { return variableScope; }
        }

        public Tag RuntimeTag
        {
            get { return runtimeTag; }
        }

        internal void SetRuntimeTag(Tag tag)
        {
            this.runtimeTag = tag;
        }

        /// <summary>
        /// 将异常添加到当前 异常集合中。
        /// </summary>
        /// <param name="e">异常</param>
        public void AddError(Exception e)
        {
            //功能暂未实现
        }

        /// <summary>
        /// 清除所有异常
        /// </summary>
        public void ClearError()
        {
            //功能暂未实现
        }

        /// <summary>
        /// 从指定TemplateContext创建一个类似的实例
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static TemplateContext CreateContext(TemplateContext context)
        {
            return new TemplateContext(context.currentPath, context.charset, context.throwErrors/*, context.url, context.res*/, new VariableScope(context.TempData));
        }

        #region ICloneable 成员

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
