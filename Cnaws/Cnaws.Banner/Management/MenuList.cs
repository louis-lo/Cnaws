using System;
using Cnaws.Management;

namespace Cnaws.Banner.Management
{
    public sealed class MenuList : ManagementMenus
    {
        protected override void InitMenus()
        {
            AddMenu(0, 30, "BANNER管理")
                .AddSubMenu("BANNER设置", "/banner");
        }
    }
}
