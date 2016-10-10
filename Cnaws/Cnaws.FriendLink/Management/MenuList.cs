using System;
using Cnaws.Management;

namespace Cnaws.FriendLink.Management
{
    public sealed class MenuList : ManagementMenus
    {
        protected override void InitMenus()
        {
            AddMenu(0, 50, "友情链接")
                .AddSubMenu("友链管理", "/friendlink");
        }
    }
}
