using System;

namespace Cnaws.Web.Templates
{
    /// <summary>
    /// 分析异常类
    /// </summary>
    public class ParseException : TemplateException
    {
        /// <summary>
        /// 模板错误
        /// </summary>
        public ParseException()
            : base()
        {
        }
        /// <summary>
        /// 模板错误
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="line">行</param>
        /// <param name="column">字符</param>
        public ParseException(string message, string file, int line, int column)
            : base(message, file, line, column)
        {
        }
        /// <summary>
        /// 模板错误
        /// </summary>
        /// <param name="message">错误信息</param>
        public ParseException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// 模板错误
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="innerException">基础信息</param>
        public ParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
