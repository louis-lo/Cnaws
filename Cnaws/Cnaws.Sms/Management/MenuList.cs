using System;
using Cnaws.Management;

namespace Cnaws.Sms.Management
{
    public sealed class MenuList : ManagementMenus
    {
        protected override void InitMenus()
        {
            AddMenu(0, 96, "短信管理")
                .AddSubMenu("短信设置", "/sms")
                // .AddSubMenu("发送短信", "/smssend")
                .AddSubMenu("模板管理", "/smstmpl")
                .AddSubMenu("短信日志", "/sms/log");
        }
    }
}
