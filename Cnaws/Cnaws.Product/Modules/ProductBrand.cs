using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;
using Cnaws.Data.Query;

namespace Cnaws.Product.Modules
{
    /// <summary>
    /// 品牌
    /// </summary>
    [Serializable]
    public class ProductBrand : IdentityModule
    {
        /// <summary>
        /// 品牌名称
        /// </summary>
        [DataColumn(16)]
        public string Name = null;
        /// <summary>
        /// 首字母
        /// </summary>
        [DataColumn(1)]
        public string FirstChar = null;
        /// <summary>
        /// 分类Id
        /// </summary>
        public int CategoryId = 0;
        /// <summary>
        /// 品牌LOGO
        /// </summary>
        [DataColumn(256)]
        public string Logo = null;
        /// <summary>
        /// 是否推荐
        /// </summary>
        public bool Recommend = false;
        /// <summary>
        /// 是否为筛选项
        /// </summary>
        public bool Screen = false;
        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum = 0;
        /// <summary>
        /// 是否审核
        /// </summary>
        public bool Approved = false;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Approved");
            DropIndex(ds, "CategoryIdApproved");
            DropIndex(ds, "CategoryIdRecommendApproved");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Approved", "Approved");
            CreateIndex(ds, "CategoryIdApproved", "CategoryId", "Approved");
            CreateIndex(ds, "CategoryIdRecommendApproved", "CategoryId", "Recommend", "Approved");
        }

        private static string[] GetCacheName(int category)
        {
            return new string[] { "ProductBrand", "Module", category.ToString() };
        }
        private static string[] GetRecommendCacheName(int category)
        {
            return new string[] { "ProductBrand", "Module", category.ToString(), "1" };
        }
        private static string[] GetParentRecommendCacheName(int category)
        {
            return new string[] { "ProductBrand", "Module", category.ToString(), "2" };
        }
        private static string[] GetFirstCharCacheName(int category)
        {
            return new string[] { "ProductBrand", "Module", category.ToString(), "3" };
        }
        private static void RemoveParentRecommendCache(DataSource ds, int category)
        {
            CacheProvider.Current.Set(GetParentRecommendCacheName(category), null);
            int parent = ProductCategory.GetParentId(ds, category);
            if (parent > 0)
                RemoveParentRecommendCache(ds, category);
        }
        private static void RemoveCache(DataSource ds, int category)
        {
            CacheProvider.Current.Set(GetCacheName(category), null);
            CacheProvider.Current.Set(GetRecommendCacheName(category), null);
            RemoveParentRecommendCache(ds, category);
            CacheProvider.Current.Set(GetFirstCharCacheName(category), null);
        }
        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(FirstChar))
                return DataStatus.Failed;
            //if (ProductCategory.GetCountByParent(ds, CategoryId) > 0)
            //    return DataStatus.Exist;
            FirstChar = FirstChar.ToUpper();
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            RemoveCache(ds, CategoryId);
            return DataStatus.Success;
        }
        private void CheckCategoryId(DataSource ds)
        {
            if (CategoryId == 0)
                CategoryId = ExecuteScalar<ProductCategory, int>(ds, "CategoryId", P("Id", Id));
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            if (string.IsNullOrEmpty(FirstChar))
                return DataStatus.Failed;
            FirstChar = FirstChar.ToUpper();
            //CheckCategoryId(ds);
            //if (ProductCategory.GetCountByParent(ds, CategoryId) > 0)
            //    return DataStatus.Exist;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            RemoveCache(ds, CategoryId);
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            if (Product.GetCountByBrandId(ds, Id) > 0)
                return DataStatus.Exist;
            //CheckCategoryId(ds);
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            RemoveCache(ds, CategoryId);
            return DataStatus.Success;
        }

        public static long GetCountByCategoryId(DataSource ds, int categoryId)
        {
            return ExecuteCount<ProductBrand>(ds, P("CategoryId", categoryId));
        }
        public static ProductBrand GetById(DataSource ds, int id)
        {
            return ExecuteSingleRow<ProductBrand>(ds, P("Id", id));
        }
        public static SplitPageData<ProductBrand> GetPage(DataSource ds, bool approved, int index, int size, int show = 8)
        {
            int count;
            IList<ProductBrand> list = ExecuteReader<ProductBrand>(ds, Os(Oa("SortNum"), Oa("Id")), index, size, out count, P("Approved", approved));
            return new SplitPageData<ProductBrand>(index, size, list, count, show);
        }
        public static SplitPageData<dynamic> GetPageEx(DataSource ds, int categoryId, string sort, string order, int index, int size, int show = 8)
        {
            long count;
            IList<dynamic> list;
            if (categoryId > 0)
            {
                if ("desc".Equals(order, StringComparison.OrdinalIgnoreCase))
                {
                    if ("approved".Equals(sort, StringComparison.OrdinalIgnoreCase))
                        list = Db<ProductBrand>.Query(ds)
                            .Select(S<ProductBrand>())
                            .InnerJoin(O<ProductBrand>("Id"), O<ProductBrandMapping>("BrandId"))
                            .Where(W<ProductBrandMapping>("CategoryId", categoryId))
                            .OrderBy(D<ProductBrand>("Approved"), A<ProductBrand>("SortNum"), A<ProductBrand>("Id"))
                            .ToList(size, index, out count);
                    //list = ExecuteReader<ProductBrand>(ds, Os(Od("Approved"), Oa("SortNum"), Oa("Id")), index, size, out count, P("CategoryId", categoryId));
                    else
                        list = Db<ProductBrand>.Query(ds)
                            .Select(S<ProductBrand>())
                            .InnerJoin(O<ProductBrand>("Id"), O<ProductBrandMapping>("BrandId"))
                            .Where(W<ProductBrandMapping>("CategoryId", categoryId))
                            .OrderBy(D<ProductBrand>("SortNum"), A<ProductBrand>("Id"))
                            .ToList(size, index, out count);
                    //list = ExecuteReader<ProductBrand>(ds, Os(Od("SortNum"), Oa("Id")), index, size, out count, P("CategoryId", categoryId));
                }
                else
                {
                    if ("approved".Equals(sort, StringComparison.OrdinalIgnoreCase))
                        list = Db<ProductBrand>.Query(ds)
                            .Select(S<ProductBrand>())
                            .InnerJoin(O<ProductBrand>("Id"), O<ProductBrandMapping>("BrandId"))
                            .Where(W<ProductBrandMapping>("CategoryId", categoryId))
                            .OrderBy(A<ProductBrand>("Approved"), A<ProductBrand>("SortNum"), A<ProductBrand>("Id"))
                            .ToList(size, index, out count);
                    //list = ExecuteReader<ProductBrand>(ds, Os(Oa("Approved"), Oa("SortNum"), Oa("Id")), index, size, out count, P("CategoryId", categoryId));
                    else
                        list = Db<ProductBrand>.Query(ds)
                            .Select(S<ProductBrand>())
                            .InnerJoin(O<ProductBrand>("Id"), O<ProductBrandMapping>("BrandId"))
                            .Where(W<ProductBrandMapping>("CategoryId", categoryId))
                            .OrderBy(A<ProductBrand>("SortNum"), A<ProductBrand>("Id"))
                            .ToList(size, index, out count);
                    //list = ExecuteReader<ProductBrand>(ds, Os(Oa("SortNum"), Oa("Id")), index, size, out count, P("CategoryId", categoryId));
                }
            }
            else
            {
                if ("desc".Equals(order, StringComparison.OrdinalIgnoreCase))
                {
                    if ("approved".Equals(sort, StringComparison.OrdinalIgnoreCase))
                        list = Db<ProductBrand>.Query(ds)
                            .Select()
                            .OrderBy(D("Approved"), A("SortNum"), A("Id"))
                            .ToList(size, index, out count);
                    //list = ExecuteReader<ProductBrand>(ds, Os(Od("Approved"), Oa("SortNum"), Oa("Id")), index, size, out count);
                    else
                        list = Db<ProductBrand>.Query(ds)
                            .Select()
                            .OrderBy(D("SortNum"), A("Id"))
                            .ToList(size, index, out count);
                    //list = ExecuteReader<ProductBrand>(ds, Os(Od("SortNum"), Oa("Id")), index, size, out count);
                }
                else
                {
                    if ("approved".Equals(sort, StringComparison.OrdinalIgnoreCase))
                        list = Db<ProductBrand>.Query(ds)
                            .Select()
                            .OrderBy(A("Approved"), A("SortNum"), A("Id"))
                            .ToList(size, index, out count);
                    //list = ExecuteReader<ProductBrand>(ds, Os(Oa("Approved"), Oa("SortNum"), Oa("Id")), index, size, out count);
                    else
                        list = Db<ProductBrand>.Query(ds)
                            .Select()
                            .OrderBy(A("SortNum"), A("Id"))
                            .ToList(size, index, out count);
                    //list = ExecuteReader<ProductBrand>(ds, Os(Oa("SortNum"), Oa("Id")), index, size, out count);
                }
            }
            foreach (var item in list)
                item.Categorys = ProductBrandMapping.GetByBrandId(ds, item.Id);
            return new SplitPageData<dynamic>(index, size, list, count, show);
        }
        public static IList<ProductBrand> GetAllByCategory(DataSource ds, int categoryId)
        {
            string[] key = GetCacheName(categoryId);
            IList<ProductBrand> result = null; //CacheProvider.Current.Get<IList<ProductBrand>>(key);
            if (result == null)
            {
                //result = ExecuteReader<ProductBrand>(ds, Os(Oa("SortNum"), Oa("Id")), P("Approved", true) & P("CategoryId", categoryId));
                result = ProductBrandMapping.GetBrandListByCategoryId(ds, categoryId);
                //CacheProvider.Current.Set(key, result);
            }
            return result;

        }
        public static IList<ProductBrand> GetAllByCategoryAndScreen(DataSource ds, int categoryId)
        {
            string[] key = GetCacheName(categoryId);
            IList<ProductBrand> result = null; //CacheProvider.Current.Get<IList<ProductBrand>>(key);
            if (result == null)
            {
                //result = ExecuteReader<ProductBrand>(ds, Os(Oa("SortNum"), Oa("Id")), P("Approved", true) & P("CategoryId", categoryId));
                result = ProductBrandMapping.GetBrandListByCategoryIdAndScreen(ds, categoryId);
                //CacheProvider.Current.Set(key, result);
            }
            return result;

        }
        public static IList<ProductBrand> GetAllRecommendByCategory(DataSource ds, int categoryId)
        {
            string[] key = GetRecommendCacheName(categoryId);
            IList<ProductBrand> result = CacheProvider.Current.Get<IList<ProductBrand>>(key);
            if (result == null)
            {
                result = ExecuteReader<ProductBrand>(ds, Os(Oa("SortNum"), Oa("Id")), P("Approved", true) & P("Recommend", true) & P("CategoryId", categoryId));
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }
        private static void FillRecommend(DataSource ds, int categoryId, List<ProductBrand> list)
        {
            list.AddRange(ExecuteReader<ProductBrand>(ds, Os(Oa("SortNum"), Oa("Id")), P("Approved", true) & P("Recommend", true) & P("CategoryId", categoryId)));
            foreach (ProductCategory item in ProductCategory.GetAll(ds, categoryId))
                FillRecommend(ds, item.Id, list);
        }
        public static IList<ProductBrand> GetAllRecommendByParentCategory(DataSource ds, int categoryId)
        {
            string[] key = GetParentRecommendCacheName(categoryId);
            IList<ProductBrand> result = CacheProvider.Current.Get<IList<ProductBrand>>(key);
            if (result == null)
            {
                result = new List<ProductBrand>();
                FillRecommend(ds, categoryId, (List<ProductBrand>)result);
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }
        public static SortedDictionary<string, List<ProductBrand>> GetAllFirstCharByCategory(DataSource ds, int categoryId)
        {
            string[] key = GetFirstCharCacheName(categoryId);
            SortedDictionary<string, List<ProductBrand>> dict = CacheProvider.Current.Get<SortedDictionary<string, List<ProductBrand>>>(key);
            if (dict == null)
            {
                List<ProductBrand> temp;
                IList<ProductBrand> list = GetAllByCategory(ds, categoryId);
                dict = new SortedDictionary<string, List<ProductBrand>>(StringComparer.InvariantCulture);
                foreach (ProductBrand pb in list)
                {
                    if (dict.TryGetValue(pb.FirstChar, out temp))
                    {
                        temp.Add(pb);
                    }
                    else
                    {
                        temp = new List<ProductBrand>();
                        temp.Add(pb);
                        dict.Add(pb.FirstChar, temp);
                    }
                }
                CacheProvider.Current.Set(key, dict);
            }
            return dict;
        }
    }
}
