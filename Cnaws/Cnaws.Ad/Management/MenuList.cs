using System;
using Cnaws.Management;

namespace Cnaws.Ad.Management
{
    public sealed class MenuList : ManagementMenus
    {
        protected override void InitMenus()
        {
            AddMenu(0, 51, "广告管理")
                .AddSubMenu("广告管理", "/ad");
        }
    }
}
