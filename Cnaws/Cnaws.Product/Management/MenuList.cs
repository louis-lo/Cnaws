using System;
using Cnaws.Management;

namespace Cnaws.Product.Management
{
    public sealed class MenuList : ManagementMenus
    {
        protected override void InitMenus()
        {
            AddMenu(1, 1, "产品管理")
                .AddSubMenu("产品分类", "/productcategory")
                .AddSubMenu("产品品牌", "/productbrand")
                .AddSubMenu("产品规格", "/productattribute")
                .AddSubMenu("产品管理", "/s_product")
                 .AddSubMenu("乡道馆产品管理", "/x_product")
                .AddSubMenu("提醒发货", "/reminderdelivery")
                .AddSubMenu("订单管理", "/productorder")
                .AddSubMenu("售后管理", "/aftersales")
                .AddSubMenu("产品统计", "/productstatistics");


            AddMenu(1, 2, "1元抢管理")
                .AddSubMenu("1元抢商品", "/oneproduct")
                .AddSubMenu("1元抢期数", "/oneproduct/number");

            AddMenu(1, 3, "试用管理")
               .AddSubMenu("试用管理", "/trialproduct");

            AddMenu(1, 4, "供应商管理")
                .AddSubMenu("供应商管理", "/supplier");

            AddMenu(1, 5, "加盟商管理")
                .AddSubMenu("加盟商管理", "/distributor");
        }
    }
}
