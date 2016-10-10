using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Data.Query;
using Cnaws.Statistic.Modules;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public class ProductCategory : IdentityModule
    {
        [DataColumn(16)]
        public string Name = null;
        [DataColumn(256)]
        public string Image = null;
        public int ParentId = 0;
        /// <summary>
        /// 显示品牌Logo还是文章
        /// </summary>
        public bool ShowLogo = false;
        public int SortNum = 0;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "ParentId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "ParentId", "ParentId");
        }

        private static string[] GetCacheName(int id)
        {
            return new string[] { "ProductCategory", "Module", id.ToString() };
        }
        private static void RemoveCache(int id, int parentId)
        {
            CacheProvider.Current.Set(GetCacheName(-1), null);
            if (parentId > -1)
                CacheProvider.Current.Set(GetCacheName(parentId), null);
            CacheProvider.Current.Set(GetCacheName(id), null);
        }
        protected virtual void RemoveCacheImpl(int id, int parentId)
        {
            RemoveCache(id, parentId);
        }
        private void RemoveCache()
        {
            RemoveCacheImpl(Id, ParentId);
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }
        protected virtual void CheckParentId(DataSource ds)
        {
            if (ParentId == 0)
                ParentId = ExecuteScalar<ProductCategory, int>(ds, "ParentId", P("Id", Id));
        }


        public static IList<dynamic> GetCategoryByApiProductList(DataSource ds, int categoryId, int categorylevel, FilterParameters2 parameters, int productType = 1)
        {
            DbWhereQueue where = null;
            ///关键词
            if (!string.IsNullOrEmpty(parameters.KeyWord))
            {
                foreach (string s in parameters.KeyWord.Split(' '))
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        if (where == null)
                            where = W("Title", s, DbWhereType.Like);
                        else
                            where |= W("Title", s, DbWhereType.Like);
                    }
                }
            }
            else
            {
                where = W<Product>("Title", parameters.KeyWord, DbWhereType.Like);
            }
            where &= (W("State", ProductState.Sale) | W("State", ProductState.BeforeSaved)) & W("ParentId", 0);
            //分类
            if (categoryId > 0)
            {
                if (categorylevel == 3)
                    where &= W("CategoryId", categoryId);
                else if (categorylevel == 2)
                    where &= (W("CategoryId", categoryId) | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result());
                else if (categorylevel == 1)
                    where &= (W("CategoryId", categoryId) | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result() | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result()).Result());
            }

            ///产品属性
            if (!string.IsNullOrEmpty(parameters.Attribute))
            {
                if (parameters.Attribute.IndexOf('@') != -1)
                {
                    string[] Attributes = parameters.Attribute.Split('@');
                    foreach (string Attr_Item in Attributes)
                    {
                        if (!string.IsNullOrEmpty(Attr_Item))
                        {
                            if (Attr_Item.IndexOf('_') != -1)
                            {
                                string[] Attr_Value = Attr_Item.Split('_');
                                if (!string.IsNullOrEmpty(Attr_Value[0]) && !string.IsNullOrEmpty(Attr_Value[1]))
                                {
                                    where &= (W("Id").InSelect<ProductAttributeMapping>(S("ProductId")).Where(W("AttributeId", long.Parse(Attr_Value[0].ToString())) & W("Value", Attr_Value[1].ToString())).Result());
                                }
                            }
                        }
                    }
                }
            }
            //供应类型
            if (parameters.SupplierType != -1)
            {
                where &= W("SupplierType", parameters.SupplierType);
            }
            if (parameters.Brand > 0)
                where &= W("BrandId", parameters.Brand);
            //}
            if (parameters.StoreId > 0)
            {
                where &= W("SupplierId", parameters.StoreId);
                if (parameters.StoreId > 0)
                {
                    where &= W("SupplierId", parameters.StoreId);
                    if (parameters.StoreCategoryId > 0)
                    {
                        if (Db<StoreCategory>.Query(ds).Select(S("ParentId")).Where(W("Id", parameters.StoreCategoryId)).First<StoreCategory>().ParentId > 0)
                        {
                            where &= W("StoreCategoryId", parameters.StoreCategoryId);
                        }
                        else
                        {
                            where &= (W("StoreCategoryId", parameters.StoreCategoryId) | W("StoreCategoryId").InSelect<StoreCategory>(S("Id")).Where(W("ParentId", parameters.StoreCategoryId)).Result());
                        }

                    }
                }
            }
            where &= W("ProductType", productType);

            IList<dynamic> list=Db<ProductCategory>.Query(ds).Select(S<ProductCategory>("*"))
                .Where(W("Id").InSelect<Product>(S("CategoryId")).Where(where).Result())
                .ToList();
            return list;
        }

        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            columns = Exclude(columns, mode, "ParentId");
            if (string.IsNullOrEmpty(Name))
                return DataStatus.Failed;
            CheckParentId(ds);
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }
        protected virtual bool HasReferences(DataSource ds)
        {
            if (ExecuteCount<ProductCategory>(ds, P("ParentId", Id)) > 0)
                return true;
            if (ProductBrand.GetCountByCategoryId(ds, Id) > 0)
                return true;
            if (Product.GetCountByCategoryId(ds, Id) > 0)
                return true;
            if (ProductAttribute.GetCountByCategoryId(ds, Id) > 0)
                return true;
            if (OneProduct.GetCountByCategoryId(ds, Id) > 0)
                return true;
            return false;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            if (HasReferences(ds))
                return DataStatus.Exist;
            CheckParentId(ds);
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }

        public static ProductCategory GetById(DataSource ds, int id)
        {
            return ExecuteSingleRow<ProductCategory>(ds, P("Id", id));
        }
        public static IList<ProductCategory> GetAllParentsById(DataSource ds, int id)
        {
            ProductCategory pc;
            List<ProductCategory> list = new List<ProductCategory>();
            while (id > 0)
            {
                pc = GetById(ds, id);
                if (pc != null)
                {
                    list.Insert(0, pc);
                    id = pc.ParentId;
                }
                else
                {
                    break;
                }
            }
            return list;
        }
        public static int GetParentId(DataSource ds, int id)
        {
            return ExecuteScalar<ProductCategory, int>(ds, "ParentId", P("Id", id));
        }
        public static SplitPageData<ProductCategory> GetPage(DataSource ds, int parentId, int index, int size, int show = 8)
        {
            int count;
            IList<ProductCategory> list = ExecuteReader<ProductCategory>(ds, Os(Oa("SortNum"), Oa("Id")), index, size, out count, P("ParentId", parentId));
            return new SplitPageData<ProductCategory>(index, size, list, count, show);
        }
        public static IList<ProductCategory> GetAll(DataSource ds, int parentId)
        {
            string[] key = GetCacheName(parentId);
            IList<ProductCategory> result = CacheProvider.Current.Get<IList<ProductCategory>>(key);
            if (result == null)
            {
                if (parentId == -1)
                    result = ExecuteReader<ProductCategory>(ds);
                else
                    result = ExecuteReader<ProductCategory>(ds, Os(Oa("SortNum"), Oa("Id")), P("ParentId", parentId));
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }
        public static IList<ProductCategory> GetTop(DataSource ds, int parentId, int count)
        {
            return Db<ProductCategory>.Query(ds)
                .Select()
                .Where(W("ParentId", parentId))
                .OrderBy(A("SortNum"), A("Id"))
                .ToList<ProductCategory>(count);
        }
        public static long GetCountByParent(DataSource ds, int parentId)
        {
            return ExecuteCount<ProductCategory>(ds, P("ParentId", parentId));
        }
        public static int GetDefaultChild(DataSource ds, int id)
        {
            return DataQuery
                .Select<ProductCategory>(ds, "Id")
                .Where(P("ParentId", id))
                .OrderBy(Oa("SortNum"), Oa("Id"))
                .Single<int>();
        }

        public IList<ProductCategory> GetChildren(DataSource ds)
        {
            return GetAll(ds, Id);
        }
        public IList<ProductCategory> GetBrother(DataSource ds)
        {
            return GetAll(ds, ParentId);
        }
    }
}
