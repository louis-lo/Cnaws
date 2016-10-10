using System;
using Cnaws.Web.Templates.Parser.Node;

namespace Cnaws.Web.Templates
{
    /// <summary>
    /// 编译错误
    /// </summary>
    public class CompileException : Exception
    {
    }

    public sealed class MethodNotFoundException : TemplateException
    {
        public MethodNotFoundException(string file, Tag tag)
            : base(string.Concat("Method \"", tag.ToCodeString(), "\" can not defined"), file, tag.FirstToken.BeginLine, tag.FirstToken.BeginColumn)
        {
        }
    }
    public sealed class PropertyNotFoundException : TemplateException
    {
        public PropertyNotFoundException(string file, Tag tag)
            : base(string.Concat("Property \"", tag.ToCodeString(), "\" can not defined"), file, tag.FirstToken.BeginLine, tag.FirstToken.BeginColumn)
        {
        }
    }
    public sealed class VariableNotFoundException : TemplateException
    {
        public VariableNotFoundException(string file, Tag tag)
            : base(string.Concat("Variable \"", tag.ToCodeString(), "\" can not defined"), file, tag.FirstToken.BeginLine, tag.FirstToken.BeginColumn)
        {
        }
    }
}
