using System;
using Cnaws.Web;
using Cnaws.Data;

namespace Cnaws.Passport.Controllers
{
    public class Logout : DataController
    {
        public void Index()
        {
            string target = Request.QueryString["target"];
            if (string.IsNullOrEmpty(target))
            {
                if (Request.UrlReferrer != null)
                    target = Request.UrlReferrer.ToString();
            }
            if (string.IsNullOrEmpty(target))
                target = GetUrl("ucenter");
            OnLogouted(User.Identity.Id);
            PassportAuthentication.SignOut();
            Refresh(target);
        }

        protected virtual void OnLogouted(long userId)
        {

        }
    }
}
