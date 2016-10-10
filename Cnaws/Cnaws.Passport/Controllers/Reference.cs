using System;
using Cnaws.Web;
using Cnaws.Web.Configuration;

namespace Cnaws.Passport.Controllers
{
    public class Reference : Controller
    {
        public void Index(long id)
        {
            Utility.SetReference(this, id);
            Refresh(string.Concat(GetUrl("register"), "?target=", System.Web.HttpUtility.UrlEncode(GetUrl("ucenter"))));
        }
    }
}
