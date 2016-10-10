using System;
using System.Web;

namespace Cnaws.Web
{
    public sealed class HttpAsyncModule : HttpModule
    {
        protected override void OnInit(HttpApplication context)
        {
            context.AddOnBeginRequestAsync(OnBeginBeginRequest, OnEndBeginRequest);
        }

        public IAsyncResult OnBeginBeginRequest(object sender, EventArgs e, AsyncCallback cb, object data)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            throw new NotImplementedException();

        }
        public void OnEndBeginRequest(IAsyncResult ar)
        {

        }
    }
}
