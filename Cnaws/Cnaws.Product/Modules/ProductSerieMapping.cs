using System;
using Cnaws.Web;
using Cnaws.Data;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public sealed class ProductSerieMapping : NoIdentityModule
    {
        [DataColumn(true)]
        public long SerieId = 0L;
        [DataColumn(16)]
        public string Name = null;
        [DataColumn(32)]
        public string Value = null;
    }
}
