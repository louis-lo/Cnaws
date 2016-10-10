using System;
using Cnaws.Web;
using Cnaws.Data;
using M = Cnaws.Product.Modules;
using System.Collections.Generic;
using Cnaws.Web.Templates;
using S = Cnaws.Statistic.Modules;

namespace Cnaws.Product.Controllers
{
    public class Category : DataController
    {
        public virtual void List(int id, long page = 1, Arguments args = null)
        {
            //int temp;
            //do
            //{
            //    temp = M.ProductCategory.GetDefaultChild(DataSource, id);
            //    if (temp > 0)
            //        id = temp;
            //}
            //while (temp > 0);

            IList<M.ProductCategory> cates = M.ProductCategory.GetAllParentsById(DataSource, id);

            IList<M.ProductAttribute> attrs = M.ProductAttribute.GetAllByCategory(DataSource, id);
            FilterParameters filter = new FilterParameters(attrs);
            filter.Load(page, args);

            this["Filter"] = filter;
            this["Category"] = M.ProductCategory.GetById(DataSource, id);
            this["CategoryList"] = cates;
            this["BigCategoryList"] = M.ProductCategory.GetAll(DataSource, cates[0].Id);
            if (cates.Count > 1)
                this["SmallCategoryList"] = M.ProductCategory.GetAll(DataSource, cates[1].Id);
            if (cates.Count > 2)
                this["ThreeCategoryList"] = M.ProductCategory.GetAll(DataSource, cates[2].Id);
            this["BrandList"] = M.ProductBrand.GetAllByCategory(DataSource, id);
            this["AttributeList"] = attrs;
            this["RecommendList"] = M.Product.GetTopRecommendByCategory(DataSource, 5, id);
            this["ProductList"] = M.Product.GetBrandPageByArguments(DataSource, id,false, filter, cates.Count,5);

            this["PageUrl"] = new FuncHandler((object[] ps) =>
            {
                return GetUrl("/category/list/", id.ToString(), filter.CopyByPage(Convert.ToInt64(ps[0])).ToString());
            });
            this["BrandUrl"] = new FuncHandler((object[] ps) =>
            {
                return GetUrl("/category/list/", id.ToString(), filter.CopyByBrand(Convert.ToInt32(ps[0])).ToString());
            });
            this["OrderUrl"] = new FuncHandler((object[] ps) =>
            {
                return GetUrl("/category/list/", id.ToString(), filter.CopyByOrderBy(Convert.ToInt32(ps[0])).ToString());
            });
            this["AttrUrl"] = new FuncHandler((object[] ps) =>
            {
                return GetUrl("/category/list/", id.ToString(), filter.Copy(Convert.ToInt64(ps[0]), ps[1] as string).ToString());
            });

            long index = filter.Page;
            filter.Page = 1;
            SplitPageData<DataJoin<M.Product, S.StatisticData>> BrandList = M.Product.GetBrandPageByArguments(DataSource, id, true, filter, cates.Count, 4, 8);
            if (BrandList.PagesCount >= index)
                filter.Page = index;
            else
                filter.Page = BrandList.PagesCount;
            this["BrandProductList"] = M.Product.GetBrandPageByArguments(DataSource, id, true, filter, cates.Count, 4, 8);
            Render("category.html");
        }

        public virtual void Child(int id)
        {
            if (IsAjax)
            {
                SetResult(M.ProductCategory.GetAll(DataSource, id));
            }
            else
            {
                NotFound();
            }
        }
    }
}
