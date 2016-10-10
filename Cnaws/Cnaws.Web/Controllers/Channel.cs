using System;
using Cnaws.Web;
using Cnaws.Data;

namespace Cnaws.Web.Controllers
{
    public sealed class Channel : DataController
    {
        public void Default(string action, Arguments args)
        {
            this["Arguments"] = args;
            Render(string.Concat("channels/", action, ".html"));
        }
    }
}
