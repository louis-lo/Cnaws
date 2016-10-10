using Cnaws.Data;
using Cnaws.Data.Query;
using Cnaws.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnaws.Product.Modules
{
    public class StoreInfo: IdentityModule
    {
        public long UserId = 0;
        /// <summary>
        /// 店铺、乡道馆名称
        /// </summary>
        [DataColumn(64)]
        public string StoreName = null;
        /// <summary>
        /// 店铺、乡道馆Logo
        /// </summary>
        [DataColumn(200)]
        public string StoreLogo = null;
        /// <summary>
        /// 店铺、乡道馆口号
        /// </summary>
        [DataColumn(250)]
        public string StoreSlogan = null;
        /// <summary>
        /// 店铺、乡道馆公告内容
        /// </summary>
        [DataColumn(250)]
        public string StoreNotice = null;
        /// <summary>
        /// 店铺、乡道馆简介
        /// </summary>
        [DataColumn(500)]
        public string StoreExplain = null;
        /// <summary>
        /// 店铺、乡道馆营业执照
        /// </summary>
        [DataColumn(300)]
        public string StoreBusinessLicense = null;

        public static StoreInfo GetStoreInfoByUserId(DataSource ds, long userId)
        {
            return Db<StoreInfo>.Query(ds).Select().Where(W("UserId", userId)).First<StoreInfo>();
        }

        public static DataStatus Update(DataSource ds, StoreInfo model)
        {
            if (GetStoreInfoByUserId(ds, model.UserId) != null)
            {
                int result = Db<StoreInfo>.Query(ds).Update()
                    .Set("StoreName", model.StoreName)
                    .Set("StoreLogo", model.StoreLogo)
                    .Set("StoreSlogan", model.StoreSlogan)
                    .Set("StoreNotice", model.StoreNotice)
                    .Set("StoreExplain", model.StoreExplain)
                    .Set("StoreBusinessLicense", model.StoreBusinessLicense)
                    .Where(W("UserId", model.UserId)).Execute();
                if (result > 0)
                {
                    return DataStatus.Success;
                }
                else
                {
                    return DataStatus.Failed;
                }
            }
            else
            {
                return model.Insert(ds);
            }
        }
    }
}
