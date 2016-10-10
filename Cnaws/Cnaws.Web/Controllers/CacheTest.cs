using System;
using Cnaws.Web;
using Cnaws.Data;
using M = Cnaws.Web.Modules;
using Cnaws.Web.Caching;
using Cnaws.Json;

namespace Cnaws.Web.Controllers
{
#if(DEBUG)
    public sealed class CacheTest : DataController
    {
        public void Index()
        {
            DateTime begin;
            DateTime end;

            M.DataTestA a = new M.DataTestA()
            {
                Boolean = true,
                DateTime = DateTime.Now,
                Double = 0.9,
                Guid = Guid.NewGuid(),
                Id = 9,
                Int32 = 99,
                Int64 = 999L,
                String = "te\r\nst",
                Money = 9999
            };
            AppCache.Instance.Set("TEST", a);
            FileCache.Instance.Set("TEST", a);
            MMFileCache.Instance.Set("MMTEST", a);
            SqlCache.Instance.Set("TEST", a);
            //RedisCache.Instance.Set("TEST", a);

            Response.Write("Instance:<br/>");
            Response.Write(JsonValue.Serialize(a));
            Response.Write("<br/>");
            Response.Write("AppCache:<br/>");
            Response.Write(JsonValue.Serialize(AppCache.Instance.Get<M.DataTestA>("TEST")));
            Response.Write("<br/>");
            Response.Write("FileCache:<br/>");
            Response.Write(JsonValue.Serialize(FileCache.Instance.Get<M.DataTestA>("TEST")));
            Response.Write("<br/>");
            Response.Write("MMFileCache:<br/>");
            Response.Write(JsonValue.Serialize(MMFileCache.Instance.Get<M.DataTestA>("MMTEST")));
            Response.Write("<br/>");
            Response.Write("SqlCache:<br/>");
            Response.Write(JsonValue.Serialize(SqlCache.Instance.Get<M.DataTestA>("TEST")));
            Response.Write("<br/>");
            //Response.Write("RedisCache:<br/>");
            //Response.Write(JsonValue.Serialize(RedisCache.Instance.Get<M.DataTestA>("TEST")));
            //Response.Write("<br/>");

            Response.Write("----------------------------------------------<br/>Set 1000<br/>");

            begin = DateTime.Now;
            for (int i = 0; i < 1000; ++i)
                AppCache.Instance.Set("TEST", a);
            end = DateTime.Now;
            Response.Write("AppCache:");
            Response.Write((end - begin).TotalMilliseconds);
            Response.Write("<br/>");

            begin = DateTime.Now;
            for (int i = 0; i < 1000; ++i)
                FileCache.Instance.Set("TEST", a);
            end = DateTime.Now;
            Response.Write("FileCache:");
            Response.Write((end - begin).TotalMilliseconds);
            Response.Write("<br/>");

            begin = DateTime.Now;
            for (int i = 0; i < 1000; ++i)
                MMFileCache.Instance.Set("MMTEST", a);
            end = DateTime.Now;
            Response.Write("MMFileCache:");
            Response.Write((end - begin).TotalMilliseconds);
            Response.Write("<br/>");

            begin = DateTime.Now;
            for (int i = 0; i < 1000; ++i)
                SqlCache.Instance.Set("TEST", a);
            end = DateTime.Now;
            Response.Write("SqlCache:");
            Response.Write((end - begin).TotalMilliseconds);
            Response.Write("<br/>");

            Response.Write("----------------------------------------------<br/>Get 1000<br/>");

            begin = DateTime.Now;
            for (int i = 0; i < 1000; ++i)
                AppCache.Instance.Get<M.DataTestA>("TEST");
            end = DateTime.Now;
            Response.Write("AppCache:");
            Response.Write((end - begin).TotalMilliseconds);
            Response.Write("<br/>");

            begin = DateTime.Now;
            for (int i = 0; i < 1000; ++i)
                FileCache.Instance.Get<M.DataTestA>("TEST");
            end = DateTime.Now;
            Response.Write("FileCache:");
            Response.Write((end - begin).TotalMilliseconds);
            Response.Write("<br/>");

            begin = DateTime.Now;
            for (int i = 0; i < 1000; ++i)
                MMFileCache.Instance.Get<M.DataTestA>("MMTEST");
            end = DateTime.Now;
            Response.Write("MMFileCache:");
            Response.Write((end - begin).TotalMilliseconds);
            Response.Write("<br/>");

            begin = DateTime.Now;
            for (int i = 0; i < 1000; ++i)
                SqlCache.Instance.Get<M.DataTestA>("TEST");
            end = DateTime.Now;
            Response.Write("SqlCache:");
            Response.Write((end - begin).TotalMilliseconds);
            Response.Write("<br/>");
        }
    }
#endif
}
