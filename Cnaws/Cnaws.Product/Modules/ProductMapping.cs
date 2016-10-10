using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;
using Cnaws.Data.Query;
using System.ComponentModel;
using Cnaws.Json;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public class ProductMapping : NoIdentityModule
    {
#if (DEBUG)
        [Description("产品Id")]
#endif
        [DataColumn(true)]
        public long ProductId = 0L;
#if (DEBUG)
        [Description("产品属性Id")]
#endif
        [DataColumn(true)]
        public long SerieId = 0L;
#if (DEBUG)
        [Description("产品属性名称")]
#endif
        [DataColumn(16)]
        public string Name = null;
#if (DEBUG)
        [Description("产品属性值")]
#endif
        [DataColumn(32)]
        public string Value = null;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Value");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Value", "Value");
        }

        public static DataStatus DeleteBySerie(DataSource ds, long serieId)
        {
            return (new ProductMapping() { SerieId = serieId }).Delete(ds, "SerieId");
        }

        public static IList<ProductMapping> GetAllByProduct(DataSource ds, long productId)
        {
            return ExecuteReader<ProductMapping>(ds, P("ProductId", productId));
            //return Db<ProductMapping>.Query(ds)
            //    .Select(S<ProductMapping>())
            //    .LeftJoin(O<ProductMapping>("ProductId"), O<Product>("Id"))
            //    .Where(W<ProductMapping>("ProductId", productId) & (W<Product>("State", ProductState.Sale) | W<Product>("State", ProductState.BeforeSaved)))
            //    .ToList<ProductMapping>();
        }

        public static IList<ProductMapping> GetAllBySerie(DataSource ds, long serieId)
        {
            return Db<ProductMapping>.Query(ds)
                .Select("Value")
                .Where(W("SerieId", serieId))
                .GroupBy("Value")
                .ToList<ProductMapping>();
        }

        public static ProductMapping GetBySerie(DataSource ds, long productId, long serieId)
        {
            return Db<ProductMapping>.Query(ds)
                .Select()
                .Where(W("ProductId", productId) & W("SerieId", serieId))
                .OrderBy(A("SerieId"))
                .First<ProductMapping>();
        }
        public static string GetAttributes(DataSource ds, long Id)
        {
            return JsonValue.Serialize(GetAllByProduct(ds, Id));
        }
        public static IList<ProductMapping> GetAllByAllProduct(DataSource ds, long productId)
        {
            return Db<ProductMapping>.Query(ds)
                .Select()
                .Where(W("ProductId").InSelect<Product>("Id").Where((W("Id", productId) | W("ParentId", productId)) & (W("State", ProductState.Sale) | W("State", ProductState.BeforeSaved))).Result())
                .ToList<ProductMapping>();
        }
        public static IList<ProductMapping> GetAllByAllProductEx(DataSource ds, long productId)
        {
            return Db<ProductMapping>.Query(ds)
                .Select()
                .Where(W("ProductId").InSelect<Product>("Id").Where(W("Id", productId) | W("ParentId", productId)).Result())
                .ToList<ProductMapping>();
        }
        public static IList<ProductMapping> GetAllByAllProductAndNotSerie(DataSource ds, long productId, long serieId)
        {
            return Db<ProductMapping>.Query(ds)
                .Select("Value")
                .Where(W("ProductId").InSelect<Product>("Id").Where(W("Id", productId) | W("ParentId", productId) & (W("State", ProductState.Sale) | W("State", ProductState.BeforeSaved))).Result() & W("SerieId", serieId, DbWhereType.NotEqual))
                .GroupBy("Value")
                .ToList<ProductMapping>();
        }

        public override string ToString()
        {
            return string.Concat(Name, ':', Value);
        }

        public static bool Exists(DataSource ds, long productId, long serieId, string value)
        {

            DbWhereQueue where = new DbWhereQueue();
            where = (W("SerieId", serieId) & W("Value", value));
            IList<ProductMapping> Mappings = GetAllByProduct(ds, productId);
            long serieCount = Db<ProductSerie>.Query(ds).Select().Where(W("ProductId").InSelect<ProductSerie>("ProductId").Where(W("Id", serieId)).Result()).Count();
            if (Mappings.Count == serieCount||(Mappings.Count==serieCount-1&&(GetBySerieIdAndProductId(ds,serieId,productId)<=0)))
            {
                foreach (ProductMapping mapping in Mappings)
                {
                    if (mapping.SerieId != serieId)
                        where &= (W("ProductId").InSelect<ProductMapping>("ProductId").Where(W("SerieId", mapping.SerieId) & W("Value", mapping.Value)).GroupBy("ProductId").Result());
                }
                where = (where) & W("ProductId", productId, DbWhereType.NotEqual);
            }
            else
            {
                return false;
            }
            return Db<ProductMapping>.Query(ds).Select().Where(W("ProductId").InSelect<ProductMapping>("ProductId").Where(where).GroupBy("ProductId").Result()).Count() > 0;
        }

        public static long GetBySerieIdAndProductId(DataSource ds, long serieId,long productId)
        {
            return Db<ProductMapping>.Query(ds).Select().Where(W("SerieId", serieId) & W("ProductId", productId)).Count();
        }
    }
}
