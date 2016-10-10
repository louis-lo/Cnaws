using System;
using Cnaws.Web;
using Cnaws.Data;

namespace Cnaws.Product.Controllers
{
    public sealed class OrderCache : PassportController
    {
        [Authorize(true)]
        public void Info(string id)
        {
            
        }
    }
}
