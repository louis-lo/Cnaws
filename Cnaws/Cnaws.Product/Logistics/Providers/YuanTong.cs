using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cnaws.Product.Logistics.Providers
{
    internal sealed class YuanTong : LogisticsProvider
    {
        /*<td class="time">
            2014-08-31 01:00:20
        </td>
        <td class="data">
            杭州转运中心 已收入
        </td>*/
        private static readonly Regex TimeRegex = new Regex(@"<td\s+class\s*=\s*""time""\s*>([\s\S]*?)</td>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static readonly Regex DataRegex = new Regex(@"<td\s+class\s*=\s*""data""\s*>([\s\S]*?)</td>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

        public override string Name
        {
            get { return "圆通快递"; }
        }
        public override string SearchUrl
        {
            get { return "http://trace.yto.net.cn:8022/TraceSimple.aspx"; }
        }

        public override HttpMethod HttpMethod
        {
            get { return HttpMethod.Post; }
        }
        public override string PostArguments
        {
            get { return "waybillNo={0}"; }
        }
        public override string RefererUrl
        {
            get { return "http://www.yto.net.cn/gw/index/index.html"; }
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
