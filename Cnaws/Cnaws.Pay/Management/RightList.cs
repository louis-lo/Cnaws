using System;
using Cnaws.Management;

namespace Cnaws.Sms.Management
{
    public sealed class RightList : ManagementRights
    {
        protected override void InitRights()
        {
            AddRight("支付设置", "management.pay");
        }
    }
}
