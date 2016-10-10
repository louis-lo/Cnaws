using System;
using System.IO;

namespace Cnaws.Web.Templates.Parser.Node
{
    public class BlockTag : BaseTag
    {
        private string text;

        public string TemplateContent
        {
            get { return text; }
            set { text = value; }
        }

        public override object Parse(TemplateContext context)
        {
            base.InitRuntime(context);

            using (StringWriter writer = new StringWriter())
            {
                Render(context, writer);

                return writer.ToString();
            }
        }

        public override void Parse(TemplateContext context, TextWriter write)
        {
            base.InitRuntime(context);

            Render(context, write);
        }

        protected void Render(TemplateContext context, TextWriter writer)
        {
            if (context == null)
            {
                writer.Write(this.TemplateContent);
                return;
            }

            if (!string.IsNullOrEmpty(this.TemplateContent))
            {
                TemplateLexer lexer = new TemplateLexer(this.TemplateContent);
                TemplateParser parser = new TemplateParser(context, lexer.Parse());

                while (parser.MoveNext())
                {
                    try
                    {
                        parser.Current.Parse(context, writer);
                    }
                    catch (TemplateException e)
                    {
                        if (context.ThrowExceptions)
                        {
                            throw e;
                        }
                        else
                        {
                            context.AddError(e);
                            writer.Write(parser.Current.ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        Exception baseException = e.GetBaseException();

                        ParseException ex = new ParseException(baseException.Message, baseException);
                        if (context.ThrowExceptions)
                        {
                            throw ex;
                        }
                        else
                        {
                            context.AddError(ex);
                            writer.Write(parser.Current.ToString());
                        }
                    }
                }

            }
        }

        protected void Eval(TemplateContext context)
        {
            if (context != null)
            {
                if (!string.IsNullOrEmpty(this.TemplateContent))
                {
                    TemplateLexer lexer = new TemplateLexer(this.TemplateContent);
                    TemplateParser parser = new TemplateParser(context, lexer.Parse());
                    while (parser.MoveNext())
                    {
                        try
                        {
                            parser.Current.Parse(context);
                        }
                        catch (TemplateException e)
                        {
                            if (context.ThrowExceptions)
                                throw e;
                            else
                                context.AddError(e);
                        }
                        catch (Exception e)
                        {
                            Exception baseException = e.GetBaseException();
                            ParseException ex = new ParseException(baseException.Message, baseException);
                            if (context.ThrowExceptions)
                                throw ex;
                            else
                                context.AddError(ex);
                        }
                    }
                }
            }
        }
    }
}
