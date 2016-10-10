using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.Pay.Modules;

namespace Cnaws.Pay.Management
{
    public sealed class Pay : ManagementController
    {
        private static readonly Version VERSION = new Version(1, 0, 0, 0);

        protected override Version Version
        {
            get { return VERSION; }
        }

        protected override string Namespace
        {
            get { return "Cnaws.Pay"; }
        }

        public void Index()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (CheckPost("pay"))
                        NotFound();
                }
            }
        }
        public void List(int page = 1)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.Payment.GetPage(DataSource, page, 10));
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
                        M.Payment attr = new M.Payment()
                        {
                            Id = Request["Id"],
                            Partner = Request["Partner"],
                            PartnerId = Request["PartnerId"],
                            PartnerKey = Request["PartnerKey"],
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
                    SetJavascript("allProvider", M.Payment.GetAll(DataSource));
            }
        }
        public void Log(Arguments args)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (args == null || args.Count == 0)
                    {
                        if (CheckPost("log"))
                            NotFound();
                    }
                    else
                    {
                        if (string.Equals(args[0], "list", StringComparison.OrdinalIgnoreCase))
                        {
                            int page = 1;
                            if (args.Count > 1)
                                page = int.Parse(args[1]);
                            if (page < 1)
                                page = 1;
                            SetResult(M.PayLog.GetPage(DataSource, page, 10));
                        }
                        else
                        {
                            NotFound();
                        }
                    }
                }
            }
        }
    }
}
