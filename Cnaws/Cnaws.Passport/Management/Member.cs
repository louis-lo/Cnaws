using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.Passport.Modules;

namespace Cnaws.Passport.Management
{
    public sealed class Member : ManagementController
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

        public void InfoEx(long id)
        {
            if (CheckRight())
            {
                SetJavascript("Member", M.MemberInfo.GetById(DataSource, id));
            }
        }
    }
}
