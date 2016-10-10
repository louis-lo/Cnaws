using System;

namespace Cnaws.Web.Templates
{
    /// <summary>
    /// 常规性错误
    /// </summary>
    public class TemplateException : Exception
    {
        private string errorFile;
        private int errorLine;
        private int errorColumn;
        //private string errorCode;

        public string File
        {
            get { return errorFile; }
        }
        /// <summary>
        /// 所在行
        /// </summary>
        public int Line
        {
            get { return errorLine; }
        }
        /// <summary>
        /// 所在字符
        /// </summary>
        public int Column
        {
            get { return errorColumn; }
        }
        ///// <summary>
        ///// 错误代码
        ///// </summary>
        //public string Code
        //{
        //    get { return errorCode; }
        //}

        /// <summary>
        /// 模板错误
        /// </summary>
        public TemplateException()
            : base()
        {
        }
        /// <summary>
        /// 模板错误
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="line">行</param>
        /// <param name="column">字符</param>
        public TemplateException(string message, string file, int line, int column)
            : base(string.Concat(message, "\r\nFile:", file, " Line:", line.ToString(), " Column:", column.ToString()))
        {
            this.errorFile = file;
            this.errorLine = line;
            this.errorColumn = column;
        }
        /// <summary>
        /// 模板错误
        /// </summary>
        /// <param name="message">错误信息</param>
        public TemplateException(String message)
            : base(message)
        {
        }
        /// <summary>
        /// 模板错误
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="innerException">基础信息</param>
        public TemplateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
