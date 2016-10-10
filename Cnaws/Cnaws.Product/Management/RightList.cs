using System;
using Cnaws.Management;

namespace Cnaws.Product.Management
{
    public sealed class RightList : ManagementRights
    {
        protected override void InitRights()
        {
            AddRight("城品惠-商品分类管理", "management.productcategory");
            AddRight("城品惠-商品品牌管理", "management.productbrand");
            AddRight("城品惠-商品规格管理", "management.productattribute");
            //AddRight("产品管理", "management.product");
            AddRight("城品惠-订单管理", "management.productorder");
            //AddRight("售后管理", "management.aftersalse");
            //AddRight("1元抢管理", "management.oneproduct");
            //AddRight("试用管理", "management.trialproduct");
            //AddRight("供应商管理", "management.supplier");
            //AddRight("加盟商管理", "management.distributor");
        }
    }
}
