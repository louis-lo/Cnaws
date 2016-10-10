using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Data.Query;
using System.Collections.Generic;

namespace Cnaws.Sms.Modules
{
    public enum SmsTemplateType
    {
        Text = 0,
        Template = 1
    }

    [Serializable]
    public class SmsTemplate : NoIdentityModule
    {
        public const string Register = "register";
        public const string Password = "password";
        /// <summary>
        /// 供应商,上传好产品后发送
        /// </summary>
        public static readonly string SupplierUploadedProduct = "supplierUploadedProduct";
        /// <summary>
        /// 加盟商,账号开通后
        /// </summary>
        public static readonly string DistributorRegistered = "distributorRegistered";
        /// <summary>
        /// 客户完成下单付款
        /// </summary>
        public static readonly string MemberPaid = "memberPaid";
        /// <summary>
        /// 发货成功之后
        /// </summary>
        public static readonly string HasShipped = "hasShipped";

        [DataColumn(true, 32)]
        public string Name = null;
        [DataColumn(16)]
        public string Summary = null;
        public SmsTemplateType Type = SmsTemplateType.Text;
        [DataColumn(128)]
        public string Content = null;

        protected override void OnInstallAfter(DataSource ds)
        {
            (new SmsTemplate() { Name = Register, Summary = "用户注册" }).Insert(ds);
            (new SmsTemplate() { Name = Password, Summary = "找回密码" }).Insert(ds);
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            Name = Name.ToLower();
            return DataStatus.Success;
        }

        public static SmsTemplate GetByName(DataSource ds, string name)
        {
            return Db<SmsTemplate>.Query(ds)
                .Select()
                .Where(W("Name", name))
                .First<SmsTemplate>();
        }

        public static SplitPageData<SmsTemplate> GetPage(DataSource ds, int index, int size, int show = 8,SmsTemplateType type=SmsTemplateType.Text)
        {
            int count;
            IList<SmsTemplate> list = ExecuteReader<SmsTemplate>(ds, Os(Oa("Name")), index, size, out count,P("type", (short)type));
            return new SplitPageData<SmsTemplate>(index, size, list, count, show);
        }
    }
}
