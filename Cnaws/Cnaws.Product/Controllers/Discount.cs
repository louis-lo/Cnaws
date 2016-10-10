using System;
using Cnaws.Web;
using Cnaws.Data;
using M = Cnaws.Product.Modules;
using Cnaws.Web.Templates;

namespace Cnaws.Product.Controllers
{
    public class Discount : DataController
    {
        public void Index()
        {
            List();
        }

        public void List(long index = 1)
        {
            this["RecommendList"] = M.Product.GetTopRecommendByCategory(DataSource, 5, 0);
            this["ProductList"] = M.Product.GetPageByDiscount(DataSource, index, 15, 8);
            Render("discount.html");
        }
    }
}
