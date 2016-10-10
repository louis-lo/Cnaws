using Cnaws.Data;
using System;
using System.Collections.Generic;

namespace Cnaws.Product
{
    [Serializable]
    public sealed class ProductCacheInfo
    {
        public string Title = null;
        public string Image = null;
        public string Content = null;
        public string BarCode = null;
        public string Unit = null;
        public Dictionary<string, string> Series = new Dictionary<string, string>();
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();

        public ProductCacheInfo()
        {
        }
        public ProductCacheInfo(DataSource ds, Modules.Product p)
        {
            Title = p.Title;
            Image = p.Image;
            Content = p.Content;
            BarCode = p.BarCode;
            Unit = p.Unit;

            IList<Modules.ProductMapping> series = Modules.ProductMapping.GetAllByProduct(ds, p.Id);
            foreach (Modules.ProductMapping item in series)
                Series.Add(item.Name, item.Value);

            IList<DataJoin<Modules.ProductAttribute, Modules.ProductAttributeMapping>> attrs = Modules.ProductAttribute.GetAllValuesByProduct(ds, p.Id);
            foreach (DataJoin<Modules.ProductAttribute, Modules.ProductAttributeMapping> item in attrs)
                Attributes.Add(item.A.Name, item.B.Value);
        }
    }
}
