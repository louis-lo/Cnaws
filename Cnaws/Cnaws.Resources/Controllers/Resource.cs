using System;
using System.IO;
using System.Web;
using System.Threading;
using System.Reflection;
using Cnaws.Web;
using Cnaws.Templates;

namespace Cnaws.Resources.Controllers
{
    public class Resource : ResourceController
    {
        private static readonly Version VERSION = new Version(1, 0, 1, 18);

        protected override Version Version
        {
            get { return VERSION; }
        }
        protected override string Namespace
        {
            get { return "Cnaws.Resources"; }
        }

        public void Static(string name, Arguments args)
        {
            RenderResource(name, args, true);
        }
    }
}
