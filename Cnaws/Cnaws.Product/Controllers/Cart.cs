using System;
using Cnaws.Web;
using Cnaws.Data;
using M = Cnaws.Product.Modules;
using System.Collections.Generic;

namespace Cnaws.Product.Controllers
{
    public class Cart : PassportController
    {
        [Authorize(true)]
        public virtual void Index()
        {
            IList<DataJoin<M.ProductCart, M.Product>> list = M.ProductCart.GetPageByUser(DataSource, User.Identity.Id);
            Money total = 0;
            foreach (DataJoin<M.ProductCart, M.Product> item in list)
                total += item.B.GetTotalMoney(item.A.Count);
            this["CartList"] = list;
            this["TotalMoney"] = total;
            Render("cart.html");
        }

        [HttpAjax]
        [Authorize]
        public virtual void GetList()
        {
            try
            {
                SetResult(M.ProductCart.GetPageByUser(DataSource, User.Identity.Id));
            }
            catch (Exception)
            {
                SetResult(false);
            }
        }

        [HttpAjax]
        [Authorize]
        public virtual void Count()
        {
            try
            {
                SetResult(true, M.ProductCart.GetCountByUser(DataSource, User.Identity.Id));
            }
            catch (Exception)
            {
                SetResult(false);
            }
        }

        [HttpAjax]
        [HttpPost]
        [Authorize]
        public virtual void Add()
        {
            try
            {
                M.Product product = M.Product.GetSaleProduct(DataSource, long.Parse(Request.Form["Id"]));
                if (product == null)
                {
                    SetResult((int)-1023);
                }
                M.ProductCart cart = new M.ProductCart(DataSource, User.Identity.Id, product, int.Parse(Request.Form["Count"]));
                if (cart.Count <= product.Inventory)
                {
                    M.ProductCart productcart = M.ProductCart.GetProductByUser(DataSource, User.Identity.Id, long.Parse(Request.Form["Id"]));
                    if (productcart == null || productcart.Id <= 0)
                        SetResult(cart.Add(DataSource));
                    else
                    {
                        productcart.Count += int.Parse(Request.Form["Count"]);
                        SetResult(productcart.Update(DataSource));
                    }

                }
                else
                    SetResult((int)-1027);
            }
            catch (Exception ex)
            {
                SetResult(false, ex.ToString());
            }
        }

        [HttpAjax]
        [HttpPost]
        [Authorize]
        public virtual void Del()
        {
            try
            {

                string temp = Request.Form["id"];
                string[] ids = temp.Split(',');
                if (ids.Length > 0)
                {
                    DataSource.Begin();
                    try
                    {
                        for (int i = 0; i < ids.Length; ++i)
                        {
                            if ((new M.ProductCart() { Id = long.Parse(ids[i]), UserId = User.Identity.Id }).Remove(DataSource) != DataStatus.Success)
                                throw new Exception();
                        }
                        DataSource.Commit();
                        SetResult(true);
                    }
                    catch (Exception)
                    {
                        DataSource.Rollback();
                        SetResult((int)-1020);
                    }
                }
                else
                {
                    SetResult((int)-1023);
                }
            }
            catch (Exception)
            {
                SetResult(false);
            }
        }
    }
}
