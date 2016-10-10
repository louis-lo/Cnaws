using System;
using Cnaws.Management;

namespace Cnaws.Pay.Management
{
    public sealed class MenuList : ManagementMenus
    {
        protected override void InitMenus()
        {
            AddMenu(0, 97, "支付管理")
                .AddSubMenu("支付设置", "/pay")
                .AddSubMenu("支付日志", "/pay/log");
        }
    }
}
