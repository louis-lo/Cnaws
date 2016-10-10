using System;
using Cnaws.Web;
using Cnaws.Data;

namespace Cnaws.Web.Controllers
{
    public class Index : DataController
    {
        public void index()
        {
            Render("index.html");
        }
    }
}
