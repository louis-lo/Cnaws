using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.Passport.Modules;

namespace Cnaws.Passport.Management
{
    public sealed class Oauth : ManagementController
    {
        private static readonly Version VERSION = new Version(1, 0, 0, 0);

        protected override Version Version
        {
            get { return VERSION; }
        }

        protected override string Namespace
        {
            get { return "Cnaws.Passport"; }
        }

        public void Index()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (CheckPost("oauth"))
                        NotFound();
                }
            }
        }
        public void List(int page = 1)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.OAuth2.GetPage(DataSource, page, 10));
            }
        }
        public void Mod()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (IsPost)
                    {
                        M.OAuth2 attr = new M.OAuth2()
                        {
                            Id = Request["Id"],
                            Name = Request["Name"],
                            Secret = Request["Secret"],
                            Key = Request["Key"],
                            Version = "1.0.0.0",
                            Enabled = Types.GetBooleanFromString(Request["Enabled"])
                        };
                        SetResult(attr.Update(DataSource), () =>
                        {
                            WritePostLog("MOD");
                        });
                    }
                    else
                    {
                        NotFound();
                    }
                }
            }
        }
        public void All()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetJavascript("allProvider", M.OAuth2.GetAll(DataSource));
            }
        }
    }
}
