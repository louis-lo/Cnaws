using System;
using Cnaws.Web;
using Cnaws.Data;
using System.Collections.Generic;
using Cnaws.Data.Query;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public class ProductAttributeMapping : NoIdentityModule
    {
        [DataColumn(true)]
        public long ProductId = 0L;
        [DataColumn(true)]
        public long AttributeId = 0L;
        public string Value = null;

        public DataStatus InsertOrUpdate(DataSource ds)
        {
            if (ExecuteCount(ds, P("ProductId", ProductId) & P("AttributeId", AttributeId)) > 0)
                return Update(ds);
            return Insert(ds);
        }
        public static long GetCountByAttributeId(DataSource ds, long attributeId)
        {
            return ExecuteCount<ProductAttributeMapping>(ds, P("AttributeId", attributeId));
        }

        public static IList<ProductAttributeMapping> GetAllByAttribute(DataSource ds, long attrId)
        {
            return DataQuery.Select<ProductAttributeMapping>(ds, "Value")
                .Where(P("AttributeId", attrId))
                .GroupBy("Value")
                .ToList<ProductAttributeMapping>();
        }

        public static IList<ProductAttributeMapping> GetListByProduct(DataSource ds, long productId)
        {
            return Db<ProductAttributeMapping>.Query(ds)
                .Select()
                .Where(W("ProductId", productId))
                .ToList<ProductAttributeMapping>();
        }

        public static Dictionary<long, string> GetAllByProduct(DataSource ds, long productId)
        {
            IList<ProductAttributeMapping> list = DataQuery.Select<ProductAttributeMapping>(ds)
                .Where(P("ProductId", productId))
                .ToList<ProductAttributeMapping>();
            Dictionary<long, string> dict = new Dictionary<long, string>(list.Count);
            foreach (ProductAttributeMapping item in list)
                dict.Add(item.AttributeId, item.Value);
            return dict;
        }

        public static IList<ProductAttributeMapping> GetAllByCategoryId(DataSource ds,int categoryId, int categorylevel)
        {
            DbWhereQueue where = W("Id", 0, DbWhereType.GreaterThan);
            if (categoryId > 0)
            {
                if (categorylevel == 3)
                    where &= W("CategoryId", categoryId);
                else if (categorylevel == 2)
                    where &= (W("CategoryId", categoryId) | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result());
                else if (categorylevel == 1)
                    where &= (W("CategoryId", categoryId) | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result() | W("CategoryId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId").InSelect<ProductCategory>(S("Id")).Where(W("ParentId", categoryId)).Result()).Result());
            }
            return Db<ProductAttributeMapping>.Query(ds).Select(S("AttributeId"), S("Value"))
                .Where(W("AttributeId").InSelect<ProductAttribute>(S("Id")).Where(where).Result()
                )
                .GroupBy(G("AttributeId"),G("Value"))
                .ToList<ProductAttributeMapping>();
        }

        
    }
}
