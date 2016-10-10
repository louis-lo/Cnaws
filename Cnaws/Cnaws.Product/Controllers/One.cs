using System;
using Cnaws.Web;
using Cnaws.Data;
using M = Cnaws.Product.Modules;
using System.Collections.Generic;

namespace Cnaws.Product.Controllers
{
    public class One : DataController
    {
        public virtual void Info(int productId, long productNum)
        {
            M.OneProduct product = M.OneProduct.GetById(DataSource, productId);
            if (product != null)
            {
                M.OneProductNumber number = M.OneProductNumber.GetAllById(DataSource, productNum);
                if (number != null)
                {
                    this["Product"] = product;
                    this["Number"] = number;
                    OnInfo(product, number);
                    Render("one.html");
                    return;
                }
            }
            NotFound();
        }

        protected virtual void OnInfo(M.OneProduct product, M.OneProductNumber number)
        {
        }

        public virtual void Progress(int productId, long productNum)
        {
            SetResult(true, M.OneProductOrder.GetCount(DataSource, productId, productNum));
        }

        public virtual void Orders(int productId, long productNum, long page = 1)
        {
            this["OrderList"] = M.OneProductOrder.GetPage(DataSource, productId, productNum, page, 20, 8);
            Render("one.html");
        }
        public virtual void History(int productId, long page = 1)
        {
            this["HistoryList"] = M.OneProductNumber.GetAllByProduct(DataSource, productId);
            Render("one.html");
        }
    }
}
