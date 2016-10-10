using System;
using Cnaws.Management;

namespace Cnaws.Sms.Management
{
    public sealed class RightList : ManagementRights
    {
        protected override void InitRights()
        {
            AddRight("短信设置", "management.sms");
            AddRight("模板管理", "management.smstmpl");
            AddRight("发送短信", "management.sendsms");
        }
    }
}
