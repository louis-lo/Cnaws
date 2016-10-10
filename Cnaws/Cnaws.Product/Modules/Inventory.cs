using System;
using Cnaws.Web;
using Cnaws.Data;

namespace Cnaws.Product.Modules
{
    /// <summary>
    /// 库存
    /// </summary>
    [Serializable]
    public sealed class Inventory : ProductBase<Inventory>
    {
        /// <summary>
        /// 是否停产
        /// </summary>
        public bool IsDiscontinued = false;
    }
}
