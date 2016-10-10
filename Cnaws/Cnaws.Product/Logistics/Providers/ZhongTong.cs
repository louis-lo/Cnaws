using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cnaws.Product.Logistics.Providers
{
    internal sealed class ZhongTong : LogisticsProvider
    {
        /*<li class="pr ">
            <div data-billcode="376121801969" data-sitename="东莞石鼓" class="routeTips" style=" overflow hidden;position absolute; right 10px; width 76px; height 44px; top 8px">
                <img src="/Content/themes/ztotheme/Images/ViewSignImage.gif" />签收照片
            </div>
            <div class=""><a data-sitecode='30158' data-sitetel='0769-22887455、0769-22887457、0769-22822977' target='_blank' style='color:red' href='http://www.kuaidihelp.com/wangdian/kdyDetail?mobile=15899901058' class='routeTips'>东莞石鼓</a> 的 <a data-sitetel='15899901058' target='_blank' style='color:red' href='http://www.kuaidihelp.com/wangdian/kdyDetail?mobile=15899901058' class='routeTips'>苏成</a> 正在派件</div>
            <div class="time">2016-02-23 09:04:52</div>
        </li>*/
        private static readonly Regex TimeRegex = new Regex(@"<div\s+class\s*=\s*""time"">([\s\S]*?)</div>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static readonly Regex DataRegex = new Regex(@"<li\s+class\s*=\s*""pr\s*[^""]*"">[\s\S]*?<div\s+class\s*=\s*""[^>]*"">([\s\S]*?)</div>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

        public override string Name
        {
            get { return "中通快递"; }
        }
        public override string SearchUrl
        {
            get { return "http://www.zto.com/GuestService/BillNew?txtbill={0}"; }
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
