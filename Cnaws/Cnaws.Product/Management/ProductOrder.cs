using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.Product.Modules;
using System.Collections.Generic;
using Cnaws.Pay;
namespace Cnaws.Product.Management
{
    public sealed class ProductOrder : ManagementController
    {
        private static readonly Version VERSION = new Version(1, 0, 0, 0);

        protected override Version Version
        {
            get { return VERSION; }
        }

        protected override string Namespace
        {
            get { return "Cnaws.Product"; }
        }

        public void AllCategory()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    IList<M.ProductCategory> list = M.ProductCategory.GetAll(DataSource, -1);
                    if (IsPost)
                        SetResult(list);
                    else
                        SetJavascript("allCategory", list);
                }
            }
        }

        public void Index()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (CheckPost("productorder"))
                        NotFound();
                }
            }
        }

        public void List(int state,string orderid, int page = 1)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (orderid == "_")
                        orderid = "";
                    SetResult(M.ProductOrder.GetPage(DataSource, state, orderid, page, 10));
                }
            }
        }

        public void Info(string orderId)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (CheckPost("productorder", () =>
                     {
                         this["Order"] = M.ProductOrder.GetById(DataSource, orderId);
                         this["PayMent"] = Pay.Modules.PayRecord.GetById(DataSource, orderId, PaymentType.Pay);
                     }))
                    {
                        NotFound();
                    }
                }
            }
        }

        public void Notify()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (IsPost)
                    {
                        M.Supplier value = DbTable.Load<M.Supplier>(Request.Form);
                        SetResult(value.Approved(DataSource), () =>
                        {
                            WritePostLog("MOD");
                        });
                    }
                    else
                    {
                        NotFound();
                    }
                }
            }
        }
    }
}
