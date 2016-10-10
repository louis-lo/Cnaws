using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.Sms.Modules;

namespace Cnaws.Sms.Management
{
    public sealed class SmsSend : ManagementController
    {
        private static readonly Version VERSION = new Version(1, 0, 0, 0);

        protected override Version Version
        {
            get { return VERSION; }
        }

        protected override string Namespace
        {
            get { return "Cnaws.Sms"; }
        }

        public void Index()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (CheckPost("smssend"))
                        NotFound();
                }
            }
        }
        public void Send()
        {

        }
    }
}
