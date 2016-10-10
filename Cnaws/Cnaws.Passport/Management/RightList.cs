using System;
using Cnaws.Management;

namespace Cnaws.Passport.Management
{
    public sealed class RightList : ManagementRights
    {
        protected override void InitRights()
        {
            AddRight("第三方登录", "management.oauth");
        }
    }
}
