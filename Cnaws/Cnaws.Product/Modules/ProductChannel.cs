using System;
using Cnaws.Web;
using Cnaws.Data;

namespace Cnaws.Product.Modules
{
    [Serializable]
    public sealed class ProductChannel : IdentityModule
    {
        [DataColumn(32)]
        public string Name = null;
    }
}
