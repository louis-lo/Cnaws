using System;
using Cnaws.Management;

namespace Cnaws.FriendLink.Management
{
    public sealed class RightList : ManagementRights
    {
        protected override void InitRights()
        {
            AddRight("友情链接", "management.friendlink");
        }
    }
}
