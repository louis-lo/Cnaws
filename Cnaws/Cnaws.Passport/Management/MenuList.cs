using System;
using Cnaws.Management;

namespace Cnaws.Passport.Management
{
    public sealed class MenuList : ManagementMenus
    {
        protected override void InitMenus()
        {
            AddMenu(0, 95, "第三方登录")
                .AddSubMenu("登录设置", "/oauth");
        }
    }
}
