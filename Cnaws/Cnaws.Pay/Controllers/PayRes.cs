using System;
using Cnaws.Web;

namespace Cnaws.Pay.Controllers
{
    public class PayRes : ResourceController
    {
        private static readonly Version VERSION = new Version(1, 0, 0, 0);

        protected override Version Version
        {
            get { return VERSION; }
        }
        protected override string Namespace
        {
            get { return "Cnaws.Pay"; }
        }

        public void Static(string name, Arguments args)
        {
            RenderResource(name, args, false);
        }
    }
}
