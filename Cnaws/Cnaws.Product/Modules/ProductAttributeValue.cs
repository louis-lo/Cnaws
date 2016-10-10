using System;
using Cnaws.Web;
using Cnaws.Data;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public sealed class ProductAttributeValue : LongIdentityModule
    {
        public long AttributeId = 0L;
        [DataColumn(16)]
        public string Name = null;
        public int SortNum = 0;
    }
}
