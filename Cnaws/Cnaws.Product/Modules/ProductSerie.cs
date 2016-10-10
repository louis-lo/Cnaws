using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;
using Cnaws.Data.Query;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public class ProductSerie : LongIdentityModule
    {
        /// <summary>
        /// 产品Id
        /// </summary>
        public long ProductId = 0L;
        [DataColumn(8)]
        public string Name = null;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "ProductId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "ProductId", "ProductId");
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            ds.Begin();
            return DataStatus.Success;
        }
        protected virtual DataStatus DeleteMapping(DataSource ds)
        {
            return ProductMapping.DeleteBySerie(ds, Id);
        }
        protected override DataStatus OnDeleteAfter(DataSource ds)
        {
            try
            {
                if (DeleteMapping(ds) != DataStatus.Success)
                    throw new Exception();
                ds.Commit();
            }
            catch (Exception)
            {
                ds.Rollback();
                return DataStatus.Rollback;
            }
            return DataStatus.Success;
        }
        protected override void OnDeleteFailed(DataSource ds)
        {
            ds.Rollback();
        }

        public static DataStatus Delete(DataSource ds, long id)
        {
            ds.Begin();
            try
            {
                Db<ProductMapping>.Query(ds).Delete().Where(W("SerieId", id)).Execute();
                if (Db<ProductSerie>.Query(ds).Delete().Where(W("Id", id)).Execute() != 1)
                    throw new Exception();
                ds.Commit();
                return DataStatus.Success;
            }
            catch (Exception)
            {
                ds.Rollback();
                return DataStatus.Rollback;
            }
        }

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (string.IsNullOrEmpty(Name))
                return DataStatus.ExistOther;
            return DataStatus.Success;
        }

        public static IList<ProductSerie> GetAll(DataSource ds, long productId)
        {
            return Db<ProductSerie>.Query(ds)
                    .Select()
                    .Where(W("ProductId", productId))
                    .OrderBy(A("Id"))
                    .ToList<ProductSerie>();
        }

        public static IList<dynamic> GetAllInventoryByPRoductId(DataSource ds, long productId)
        {
            return Db<ProductSerie>.Query(ds)
                    .Select(S<ProductSerie>("Value"), S_COUNT("Id"))
                    .LeftJoin(O<ProductSerie>("ProductId"), O<Product>("Id"))
                    .Where(W<ProductSerie>("ProductId", productId) & W<Product>("Inventory", 0, DbWhereType.GreaterThan))
                    .GroupBy(G<ProductSerie>("Value"))
                    .ToList();
        }

        public static ProductSerie GetByProductAndName(DataSource ds, long productId, string name)
        {
            return Db<ProductSerie>.Query(ds)
                    .Select()
                    .Where(W("ProductId", productId) & W("Name", name))
                    .First<ProductSerie>();
        }

        public IList<ProductMapping> GetMappings(DataSource ds)
        {
            return ProductMapping.GetAllBySerie(ds, Id);
        }
        /// <summary>
        /// 是否存在相同属性
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="productId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool Exists(DataSource ds, long productId,string name)
        {
            return Db<ProductSerie>.Query(ds).Select().Where(W("ProductId", productId) & W("Name", name)).Count() > 0;
        }
    }
}
