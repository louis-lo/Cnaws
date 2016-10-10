using System;
using Cnaws.Management;

namespace Cnaws.Ad.Management
{
    public sealed class RightList : ManagementRights
    {
        protected override void InitRights()
        {
            AddRight("广告管理", "management.ad");
        }
    }
}
