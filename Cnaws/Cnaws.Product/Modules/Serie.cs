using System;
using Cnaws.Web;
using Cnaws.Data;

namespace Cnaws.Product.Modules
{
    /// <summary>
    /// 系列
    /// </summary>
    [Serializable]
    public sealed class Serie : IdentityModule
    {
        [DataColumn(16)]
        public string Name = null;
    }
}
