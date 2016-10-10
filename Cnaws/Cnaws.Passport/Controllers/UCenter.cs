using System;
using Cnaws.Web;
using Cnaws.Data;
using M = Cnaws.Passport.Modules;

namespace Cnaws.Passport.Controllers
{
    public class UCenter : PassportController
    {
        [Authorize(true)]
        public void Index()
        {
            this["Member"] = M.MemberInfo.GetById(DataSource, User.Identity.Id);
            Render("ucenter.html");
        }
    }
}
