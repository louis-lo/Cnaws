using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.Sms.Modules;

namespace Cnaws.Sms.Management
{
    /// <summary>
    /// 短信模板管理
    /// </summary>
    public sealed class SmsTmpl : ManagementController
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
            if (CheckAjax() && CheckRight() && CheckPost("smstmpl"))
                NotFound();
        }
        public void List(int page = 1)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.SmsTemplate.GetPage(DataSource, page, 10));
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
                        M.SmsTemplate tmpl = new M.SmsTemplate()
                        {
                            Name = Request["Name"],
                            Content = Request["Content"],
                        };
                        SetResult(tmpl.Update(DataSource, ColumnMode.Include, "Content"), () =>
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
    }
}
