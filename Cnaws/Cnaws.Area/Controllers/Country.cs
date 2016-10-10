using System;
using Cnaws.Web;
using M = Cnaws.Area;
using System.Web;
using Cnaws.Json;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Cnaws.Area.Controllers
{
    public sealed class Country : ResourceController
    {
        private static readonly Version VERSION = new Version(1, 0, 0, 2);

        protected override Version Version
        {
            get { return VERSION; }
        }
        protected override string Namespace
        {
            get { return "Cnaws.Area"; }
        }

        private bool RenderCache(string contentType)
        {
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetMaxAge(DateTime.MaxValue - DateTime.Now);
            string ticks = VERSION.ToString(4);
            Response.Cache.SetETag(ticks);
            bool hascache = false;
            string etag = Request.Headers["If-None-Match"];
            if (!string.IsNullOrEmpty(etag))
                hascache = string.Equals(ticks, etag);
            if (hascache)
            {
                Response.StatusCode = 304;
                return true;
            }
            else
            {
                Response.StatusCode = 200;
                Response.ContentType = contentType;
                Response.Cache.SetLastModified(DateTime.Now);
                return false;
            }
        }

        public void All(string name = null)
        {
            if (!RenderCache("application/x-javascript"))
            {
                using (M.Country country = M.Country.GetCountry(name))
                    Response.Write(StaticCallResult.GetJson<City[]>(-200, country.GetAll()));
            }
        }
        public void AllJson(string name = null)
        {
            if (!RenderCache("application/x-javascript"))
            {
                using (M.Country country = M.Country.GetCountry(name))
                    Response.Write(string.Concat("var AreaAll = ", JsonValue.Serialize(country.GetAllJson()), ";"));
            }
        }
        public void Provinces(string name = null)
        {
            if (!RenderCache("application/x-javascript"))
            {
                using (M.Country country = M.Country.GetCountry(name))
                    Response.Write(StaticCallResult.GetJson<City[]>(-200, country.GetProvinces()));
            }
        }
        public void Cities(int parent, string name = null)
        {
            if (!RenderCache("application/x-javascript"))
            {
                using (M.Country country = M.Country.GetCountry(name))
                    Response.Write(StaticCallResult.GetJson<City[]>(-200, country.GetCities(parent)));
            }
        }
        public void Counties(int parent, string name = null)
        {
            if (!RenderCache("application/x-javascript"))
            {
                using (M.Country country = M.Country.GetCountry(name))
                    Response.Write(StaticCallResult.GetJson<City[]>(-200, country.GetCounties(parent)));
            }
        }
        public void Parent(int value, string name = null)
        {
            if (!RenderCache("application/x-javascript"))
            {
                using (M.Country country = M.Country.GetCountry(name))
                    Response.Write(StaticCallResult.GetJson<City[]>(-200, country.GetParent(value)));
            }
        }
        public void Current(string name = null)
        {
            try
            {
                IPLocation local;
                using (IPArea area = new IPArea())
                    local = area.Search(ClientIp);
                using (M.Country country = M.Country.GetCountry(name))
                    SetResult(local.GetCity(country));
            }
            catch (Exception)
            {
                SetResult(false);
            }
        }

#if(DEBUG)
        public void PingYin(string name = null)
        {
            using (M.Country country = M.Country.GetCountry(name))
            {
                List<KeyValuePair<string, string>> list;
                Dictionary<string, List<KeyValuePair<string, string>>> dict = country.GetAllCityPingYin();
                foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
                {
                    if (dict.TryGetValue(c.ToString(), out list))
                    {
                        if (list != null && list.Count > 0)
                        {
                            Response.Write(string.Format("<dl>\r\n<dt>{0}</dt>\r\n<dd>\r\n", c));
                            foreach (KeyValuePair<string, string> p in list)
                                Response.Write(string.Format("<a href=\"http://{0}.cfezb.com\">{1}</a>\r\n", p.Key, p.Value));
                            Response.Write("</dd>\r\n</dl>\r\n");
                        }
                        /*
                    <dl>
                                    <dt>A</dt>
                                    <dd><a href="#">北京</a><a href="#">上海</a><a href="#">广州</a><a href="#">重庆</a><a href="#">深圳</a><a href="#">北京</a><a href="#">上海</a><a href="#">广州</a><a href="#">重庆</a><a href="#">深圳</a><a href="#">北京</a><a href="#">上海</a><a href="#">广州</a><a href="#">重庆</a><a href="#">深圳</a></dd>
                                </dl>    
                    */
                    }
                }
            }
        }
#endif

        public void Static(string name, Arguments args)
        {
            RenderResource(name, args, false);
        }

        public void GetPickerData()
        {
            if (!RenderCache("application/x-javascript"))
            {
                string json;

                using (M.Country country = M.Country.GetCountry())
                {
                    List<PickerData> pickerDataList = new List<PickerData>();
                    foreach (City item in country.GetProvinces())
                    {
                        PickerData provinces = new PickerData();
                        provinces.id = item.Id;
                        provinces.name = item.Name;
                        foreach (City item2 in country.GetCities(item.Id))
                        {
                            PickerData cities = new PickerData();
                            cities.id = item2.Id;
                            cities.name = item2.Name;
                            foreach (City item3 in country.GetCounties(item2.Id))
                            {
                                PickerData counties = new PickerData();
                                counties.id = item3.Id;
                                counties.name = item3.Name;
                                cities.sub.Add(counties);
                            }
                            provinces.sub.Add(cities);
                        }
                        pickerDataList.Add(provinces);
                    }

                    json = new JavaScriptSerializer().Serialize(pickerDataList);
                    Response.Write(json);
                }
            }
        }

        //   public static int[] GetArea(

    }

    public class PickerData
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<PickerData> sub { get; set; }
        public PickerData()
        {
            sub = new List<PickerData>();
        }
    }
}
