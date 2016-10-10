using System;
using System.Web;

namespace Cnaws.Web.Controllers
{
    public sealed class Errors : Controller
    {
        public void Index()
        {
            Code();
        }

        public void Code(int code = 500)
        {
            this["Code"] = code;
            if (Context.Error != null)
            {
                HttpException e;
                Exception ex = Context.Error.GetBaseException();
                if (ex is HttpException)
                    e = (HttpException)ex;
                else
                    e = new HttpException(500, ex.Message, ex);
                this["Error"] = e;
            }
            Render(string.Concat("errors/code/", code, ".html"));
        }
    }
}
