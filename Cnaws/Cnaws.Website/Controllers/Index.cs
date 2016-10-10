using System;
using System.Web;
using Cnaws.Web;
using Cnaws.Data;

namespace Cnaws.Website.Controllers
{
    public sealed class Index : Controller
    {
        public void index()
        {
            Render("home.html");
        }
    }
}
