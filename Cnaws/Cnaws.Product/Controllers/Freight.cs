using System;
using Cnaws.Web;
using Cnaws.Data;
using M = Cnaws.Product.Modules;

namespace Cnaws.Product.Controllers
{
    public class Freight : DataController
    {
        public virtual void Info(long id)
        {
            this["Freight"] = M.FreightTemplate.GetById(DataSource, id);
            Render("freight.html");
        }
    }
}
