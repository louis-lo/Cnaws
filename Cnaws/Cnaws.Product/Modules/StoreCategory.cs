using Cnaws.Data;
using Cnaws.Data.Query;
using Cnaws.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public class StoreCategory : ProductCategory
    {
        public long UserId = 0;
        private static string[] GetCacheName(int id, long xdgid)
        {
            return new string[] { "StoreCategory", "Module", id.ToString(), xdgid.ToString() };
        }
        private static void RemoveCache(int id, int parentId, long xdgid)
        {
            CacheProvider.Current.Set(GetCacheName(-1, xdgid), null);
            if (parentId > -1)
                CacheProvider.Current.Set(GetCacheName(parentId, xdgid), null);
            CacheProvider.Current.Set(GetCacheName(id, xdgid), null);
        }
        protected void RemoveCacheImpl(int id, int parentId, long xdgid)
        {
            RemoveCache(id, parentId, xdgid);
        }
        private void RemoveCache()
        {
            RemoveCacheImpl(Id, ParentId, UserId);
        }
        protected override void CheckParentId(DataSource ds)
        {
            if (ParentId == 0)
                ParentId = ExecuteScalar<StoreCategory, int>(ds, "ParentId", P("Id", Id));
        }
        protected override bool HasReferences(DataSource ds)
        {
            if (ExecuteCount<StoreCategory>(ds, P("ParentId", Id)) > 0)
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
        protected override DataStatus OnInsertAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }
        public new static StoreCategory GetById(DataSource ds, int id)
        {
            return ExecuteSingleRow<StoreCategory>(ds, P("Id", id));
        }
        public new static IList<StoreCategory> GetAllParentsById(DataSource ds, int id)
        {
            StoreCategory pc;
            List<StoreCategory> list = new List<StoreCategory>();
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
        public new static int GetParentId(DataSource ds, int id)
        {
            return ExecuteScalar<StoreCategory, int>(ds, "ParentId", P("Id", id));
        }
        public new static SplitPageData<StoreCategory> GetPage(DataSource ds, int parentId, int index, int size, int show = 8)
        {
            int count;
            IList<StoreCategory> list = ExecuteReader<StoreCategory>(ds, Os(Oa("SortNum"), Oa("Id")), index, size, out count, P("ParentId", parentId));
            return new SplitPageData<StoreCategory>(index, size, list, count, show);
        }
        public new static IList<StoreCategory> GetAll(DataSource ds, long xdgid, int parentId)
        {
            string[] key = GetCacheName(parentId, xdgid);
            IList<StoreCategory> result = CacheProvider.Current.Get<IList<StoreCategory>>(key);
            if (result == null)
            {
                if (parentId == -1)
                    result = ExecuteReader<StoreCategory>(ds, Os(Oa("SortNum"), Oa("Id")), P("UserId", xdgid));
                else
                    result = ExecuteReader<StoreCategory>(ds, Os(Oa("SortNum"), Oa("Id")), P("ParentId", parentId) & P("UserId", xdgid));
                CacheProvider.Current.Set(key, result);
            }
            return result;
        }
        public new static IList<StoreCategory> GetTop(DataSource ds, int parentId, int count)
        {
            return Db<StoreCategory>.Query(ds)
                .Select()
                .Where(W("ParentId", parentId))
                .OrderBy(A("SortNum"), A("Id"))
                .ToList<StoreCategory>(count);
        }
        public new static long GetCountByParent(DataSource ds, int parentId)
        {
            return ExecuteCount<StoreCategory>(ds, P("ParentId", parentId));
        }
        public new static int GetDefaultChild(DataSource ds, int id)
        {

            return DataQuery
                .Select<StoreCategory>(ds, "Id")
                .Where(P("ParentId", id))
                .OrderBy(Oa("SortNum"), Oa("Id"))
                .Single<int>();
        }

        public static DataStatus Update(DataSource ds, StoreCategory model)
        {
            int result = Db<StoreCategory>.Query(ds).Update()
                .Set("Name", model.Name)
                .Set("Image", model.Image)
                .Set("SortNum", model.SortNum)
                .Where(W("UserId", model.UserId) & W("Id", model.Id)).Execute();
            model.OnUpdateAfter(ds);
            return result > 0 ? DataStatus.Success : DataStatus.Failed;
        }

        protected override DataStatus OnUpdateAfter(DataSource ds)
        {
            RemoveCache();
            return DataStatus.Success;
        }

        public static DataStatus Delete(DataSource ds, long userId, int id)
        {
            int result = Db<StoreCategory>.Query(ds).Delete()
                .Where(W("UserId", userId) & W("Id", id)).Execute();
            new StoreCategory() {Id=id,UserId=userId }.OnDeleteAfter(ds);
            return result > 0 ? DataStatus.Success : DataStatus.Failed;
        }

        public static IList<StoreCategory> GetStoreCategoryListByUserId(DataSource ds, long userId)
        {
            return Db<StoreCategory>.Query(ds).Select().Where(W("UserId", userId)).OrderBy(A("SortNum")).ToList<StoreCategory>();
        }

        public IList<Product> GetProductListByStoreCategoryId(DataSource ds)
        {
            return Product.GetProductListByStoreCategoryId(ds, Id, 4);
        }

        #region 乡道馆分类
        #region 通过分类Id查询乡道馆分类
        /// <summary>
        /// 通过分类Id查询乡道馆分类
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static StoreCategory GetXDGCategoryByCategoryId(DataSource ds, int categoryId)
        {
            return Db<StoreCategory>.Query(ds).Select().
                        Where(W("Id", categoryId)).
                        First<StoreCategory>();
        }
        #endregion

        #region 通过用户Id查询乡道馆一级分类集合
        /// <summary>
        /// 通过用户Id查询乡道馆一级分类集合
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static IList<StoreCategory> GetXDGCategoryOne(DataSource ds, long userId)
        {
            return Db<StoreCategory>.Query(ds).Select().
                        Where(W("UserId", userId) & W("ParentId", 0)).
                        OrderBy(A("SortNum")).
                        ToList<StoreCategory>();
        }
        /// <summary>
        /// 通过用户Id查询乡道馆一级分类集合
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="userId"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public static IList<StoreCategory> GetXDGCategory(DataSource ds, long userId, long parentid)
        {
            return Db<StoreCategory>.Query(ds).Select().
                        Where(W("UserId", userId) & W("ParentId", parentid)).
                        OrderBy(A("SortNum")).
                        ToList<StoreCategory>();
        }

        #endregion

        #region 通过分类Id查询乡道馆二级分类集合
        /// <summary>
        /// 通过分类Id查询乡道馆二级分类集合
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public IList<StoreCategory> GetXDGCategoryTwo(DataSource ds)
        {
            return Db<StoreCategory>.Query(ds).Select().
                        Where(W("ParentId", this.Id)).
                        OrderBy(A("SortNum")).
                        ToList<StoreCategory>();
        }
        #endregion

        #region 通过父分类Id查询一级分类
        /// <summary>
        /// 通过父分类Id查询一级分类
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public StoreCategory GetParentCategory(DataSource ds)
        {
            return Db<StoreCategory>.Query(ds).Select().
                    Where(W("Id", this.ParentId)).
                    First<StoreCategory>();
        }
        #endregion

        #region 通过分类Id查询分类下面的商品总数
        /// <summary>
        /// 通过分类Id查询分类下面的商品总数
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public long GetProductCount(DataSource ds)
        {
            return Product.GetProductCount(ds, this.Id);
        }
        #endregion

        #region 通过分类Id查询分类下面的商品集合
        /// <summary>
        /// 通过分类Id查询分类下面的商品集合
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public SplitPageData<Product> GetProduct(DataSource ds, int pageIndex, int pageSize)
        {
            return Product.GetProductListByXDGCategoryId(ds, this.Id, GetAllParentsById(ds, this.Id).Count, pageIndex, pageSize);
        }
        #endregion

        #region 删除分类
        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public DataStatus Delete(DataSource ds)
        {
            return Db<StoreCategory>.Query(ds).Delete().
                        Where(W("UserId", this.UserId) & W("Id", this.Id)).
                        Execute() > 0 ? DataStatus.Success : DataStatus.Failed;
        }
        #endregion

        #region 修改分类
        /// <summary>
        /// 修改分类
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public DataStatus Update(DataSource ds)
        {
            return Db<StoreCategory>.Query(ds).Update().
                        Set("Name", this.Name).
                        Set("Image", this.Image).
                        Set("SortNum", this.SortNum).
                        Where(W("UserId", this.UserId) & W("Id", this.Id)).
                        Execute() > 0 ? DataStatus.Success : DataStatus.Failed;
        }
        #endregion 
        #endregion

    }
}
