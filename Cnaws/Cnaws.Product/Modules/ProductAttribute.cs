using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;
using Cnaws.Data.Query;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public class ProductAttribute : LongIdentityModule
    {
        /// <summary>
        /// 名称
        /// </summary>
        [DataColumn(16)]
        public string Name = null;
        /// <summary>
        /// 分类Id
        /// </summary>
        public int CategoryId = 0;
        /// <summary>
        /// 是否为筛选项
        /// </summary>
        public bool Screen = false;
        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum = 0;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "CategoryId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "CategoryId", "CategoryId");
        }

        private static string[] GetCacheName(int category)
        {
            return new string[] { "ProductAttribute", "Module", category.ToString() };
        }
        private static void RemoveCache(int category)
        {
            CacheProvider.Current.Set(GetCacheName(category), null);
        }
        protected virtual void RemoveCacheImpl(int category)
        {
            RemoveCache(category);
        }
        private void RemoveCache()
        {
            RemoveCacheImpl(CategoryId);
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            //if (ProductCategory.GetCountByParent(ds, CategoryId) > 0)
            //    return DataStatus.Exist;
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }

        protected virtual void CheckCategoryId(DataSource ds)
        {
            if (CategoryId == 0)
                CategoryId = ExecuteScalar<ProductCategory, int>(ds, "Id", P("Id", Id));
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "CategoryId");
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            CheckCategoryId(ds);
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }

        protected virtual bool HasReferences(DataSource ds)
        {
            if (ProductAttributeMapping.GetCountByAttributeId(ds, Id) > 0)
                return true;
            return false;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            if (HasReferences(ds))
                return DataStatus.Exist;
            CheckCategoryId(ds);
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }

        public static long GetCountByCategoryId(DataSource ds, int categoryId)
        {
            return ExecuteCount<ProductAttribute>(ds, P("CategoryId", categoryId));
        }
        public static ProductAttribute GetById(DataSource ds, int id)
        {
            return ExecuteSingleRow<ProductAttribute>(ds, P("Id", id));
        }
        public static SplitPageData<ProductAttribute> GetPage(DataSource ds, int categoryId, int index, int size, int show = 8)
        {
            int count;
            IList<ProductAttribute> list;
            if (categoryId > 0)
                list = ExecuteReader<ProductAttribute>(ds, Os(Oa("SortNum"), Oa("Id")), index, size, out count, P("CategoryId", categoryId));
            else
                list = ExecuteReader<ProductAttribute>(ds, Os(Oa("SortNum"), Oa("Id")), index, size, out count);
            return new SplitPageData<ProductAttribute>(index, size, list, count, show);
        }
        public static IList<ProductAttribute> GetAllByCategory(DataSource ds, int categoryId)
        {
            string[] key;
            IList<ProductAttribute> value;
            List<ProductAttribute> result = new List<ProductAttribute>();
            foreach (ProductCategory cate in ProductCategory.GetAllParentsById(ds, categoryId))
            {
                key = GetCacheName(cate.Id);
                value = CacheProvider.Current.Get<IList<ProductAttribute>>(key);
                if (value == null)
                {
                    value = ExecuteReader<ProductAttribute>(ds, Os(Oa("SortNum"), Oa("Id")), P("CategoryId", cate.Id));
                    CacheProvider.Current.Set(key, value);
                }
                result.AddRange(value);
            }
            return result;
        }
        public static IList<ProductAttribute> GetAllByCategoryAndScreen(DataSource ds, int categoryId)
        {
            string[] key;
            IList<ProductAttribute> value;
            List<ProductAttribute> result = new List<ProductAttribute>();
            foreach (ProductCategory cate in ProductCategory.GetAllParentsById(ds, categoryId))
            {
                key = GetCacheName(cate.Id);
                value = CacheProvider.Current.Get<IList<ProductAttribute>>(key);
                if (value == null)
                {
                    value = ExecuteReader<ProductAttribute>(ds, Os(Oa("SortNum"), Oa("Id")), P("CategoryId", cate.Id) & P("Screen", true));
                    CacheProvider.Current.Set(key, value);
                }
                result.AddRange(value);
            }
            return result;
        }


        public IList<ProductAttributeMapping> GetAllValuesByCategory(DataSource ds)
        {
            return ProductAttributeMapping.GetAllByAttribute(ds, Id);
        }
        public static IList<DataJoin<ProductAttribute, ProductAttributeMapping>> GetAllValuesByProduct(DataSource ds, long productId)
        {
            return DataQuery
                .Select<ProductAttribute>(ds, C<ProductAttribute>("Name"), C<ProductAttributeMapping>("Value"))
                .Join<ProductAttribute>("Id", DataJoinType.Right).On<ProductAttributeMapping>("AttributeId")
                .Where(P<ProductAttributeMapping>("ProductId", productId))
                .ToList<DataJoin<ProductAttribute, ProductAttributeMapping>>();
        }


    }
}
