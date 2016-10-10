using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cnaws.Product.Logistics.Providers
{
    internal sealed class ShenTong : LogisticsProvider
    {
        /*<div class="fl-left">2016-01-03 09:47:37 </div><div class="fl-right invert-mid">
            <span>
                快件已到达 <a href="javascript:void(0);" name="express_name_display" sitemobile="0769-22995234" express_no="220567600741" shop_id="71416">
                    广东东莞南城公司
                    (0769-22995234)<div style="display:none;z-index:994" id="994" class="cp-layer">
                    </div>
                </a>
            </span>
        </div>*/
        private static readonly Regex TimeRegex = new Regex(@"<div\s+class\s*=\s*""fl-left""\s*>([\s\S]*?)</div>\s*<div\s+class\s*=\s*""fl-right", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static readonly Regex DataRegex = new Regex(@"<div\s+class\s*=\s*""fl-right\s+[^""]+"">([\s\S]*?)</div>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

        public override string Name
        {
            get { return "申通快递"; }
        }
        public override string SearchUrl
        {
            get { return "http://q1.sto.cn/chaxun/result?express_no={0}"; }
        }

        public override ILogisticsInfo[] ParseResult(string s)
        {
            MatchCollection times = TimeRegex.Matches(s);
            MatchCollection datas = DataRegex.Matches(s);
            int count = Math.Min(times.Count, datas.Count);
            List<route> list = new List<route>(count);
            for (int i = 0; i < count; ++i)
                list.Add(new route(times[i].Groups[1].Value.Trim(), datas[i].Groups[1].Value.Trim()));
            return list.ToArray();
        }
    }
}
