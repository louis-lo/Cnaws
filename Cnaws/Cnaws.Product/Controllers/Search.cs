using System;
using Cnaws.Web;
using Cnaws.Data;
using M = Cnaws.Product.Modules;
using Cnaws.Web.Templates;
using System.Web;

namespace Cnaws.Product.Controllers
{
    public class Search : DataController
    {
        public void Index()
        {
            List();
        }

        public void List(long page = 1, Arguments args = null)
        {
            string q = Request.QueryString["q"];

            FilterParameters filter = new FilterParameters();
            filter.Load(page, args);

            this["Q"] = q;
            this["Filter"] = filter;
            this["RecommendList"] = M.Product.GetTopRecommendByCategory(DataSource, 5, 0);
            this["ProductList"] = M.Product.GetPageBySearch(DataSource, Request.QueryString, filter, page, 20, 8);
            string requestParam = string.Format("?q={0}&searchType={1}&id={2}", Request.QueryString["q"], Request.QueryString["searchType"], Request.QueryString["id"]);
            this["PageUrl"] = new FuncHandler((object[] ps) =>
            {
                return string.Concat(GetUrl("/search/list", filter.CopyByPage(Convert.ToInt64(ps[0])).ToString()), requestParam);
            });
            this["BrandUrl"] = new FuncHandler((object[] ps) =>
            {
                return string.Concat(GetUrl("/search/list", filter.CopyByBrand(Convert.ToInt32(ps[0])).ToString()), requestParam);
            });
            this["OrderUrl"] = new FuncHandler((object[] ps) =>
            {
                return string.Concat(GetUrl("/search/list", filter.CopyByOrderBy(Convert.ToInt32(ps[0])).ToString()), requestParam);
            });
            this["AttrUrl"] = new FuncHandler((object[] ps) =>
            {
                return string.Concat(GetUrl("/search/list", filter.Copy(Convert.ToInt64(ps[0]), ps[1] as string).ToString()), requestParam);
            });

            Render("search.html");
        }
    }
}
