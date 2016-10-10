using System;
using Cnaws.Management;

namespace Cnaws.Banner.Management
{
    public sealed class RightList : ManagementRights
    {
        protected override void InitRights()
        {
            AddRight("BANNER设置", "management.banner");
        }
    }
}
