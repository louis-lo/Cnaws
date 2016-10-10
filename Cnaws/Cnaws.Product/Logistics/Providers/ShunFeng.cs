using Cnaws.Json;
using System;
using System.Collections.Generic;

namespace Cnaws.Product.Logistics.Providers
{
    public sealed class route : ILogisticsInfo
    {
        public string scanDateTime = null;
        public string remark = null;

        public route()
        {
        }
        public route(string time, string status)
        {
            scanDateTime = time;
            remark = status;
        }

        DateTime ILogisticsInfo.Time
        {
            get { return DateTime.Parse(scanDateTime); }
        }
        string ILogisticsInfo.Status
        {
            get { return remark; }
        }
    }
    public sealed class express
    {
        public List<route> routes = new List<route>();
    }

    internal sealed class ShunFeng : LogisticsProvider
    {
        /*[
        {
            "id":"471728472811",
            "origin":"深圳市",
            "originOld":"深圳市",
            "originCode":"755AC",
            "destination":"东莞市",
            "destinationOld":"东莞市",
            "receiveBillFlg":null,
            "delivered":true,
            "expectedDeliveryTime":null,
            "refundable":false,
            "limitTypeCode":"T4",
            "limitTypeName":"标准快递",
            "mainlandToMainland":true,
            "routes":[
                {
                    "scanDateTime":"2015-12-25 23:09:05",
                    "remark":"顺丰速运 已收取快件",
                    "stayWhyCode":""
                },
                {
                    "scanDateTime":"2015-12-25 23:26:25",
                    "remark":"快件离开<a style='color:red;text-decoration:none;' href='javascript:void(0)' link-type='store-point' store-type='1' virtual-address-code='755AC' terminal-code='755AC' terminal-type='1'><font color='red'>【深圳福田上步大厦营业部】</font></a>,正发往 【深圳彩田集散中心】",
                    "stayWhyCode":""
                },
                {
                    "scanDateTime":"2015-12-26 00:00:39",
                    "remark":"快件到达 【深圳彩田集散中心】",
                    "stayWhyCode":""
                },
                {"scanDateTime":"2015-12-26 00:37:14","remark":"快件离开【深圳彩田集散中心】,正发往 【东莞虎门集散中心】","stayWhyCode":""},
                {"scanDateTime":"2015-12-26 02:20:13","remark":"快件到达 【东莞虎门集散中心】","stayWhyCode":""},
                {"scanDateTime":"2015-12-26 03:51:31","remark":"快件离开【东莞虎门集散中心】,正发往 <a style='color:red;text-decoration:none;' href='javascript:void(0)' link-type='store-point' store-type='1' virtual-address-code='769AO' terminal-code='769AO' terminal-type='1'><font color='red'>【东莞南城艺展中心营业点】</font></a>","stayWhyCode":""},
                {"scanDateTime":"2015-12-26 05:21:40","remark":"快件到达 <a style='color:red;text-decoration:none;' href='javascript:void(0)' link-type='store-point' store-type='1' virtual-address-code='769AO' terminal-code='769AO' terminal-type='1'><font color='red'>【东莞南城艺展中心营业点】</font></a>","stayWhyCode":""},
                {"scanDateTime":"2015-12-26 05:47:11","remark":"快件离开<a style='color:red;text-decoration:none;' href='javascript:void(0)' link-type='store-point' store-type='1' virtual-address-code='769AO' terminal-code='769AO' terminal-type='1'><font color='red'>【东莞南城艺展中心营业点】</font></a>,正发往 <a style='color:red;text-decoration:none;' href='javascript:void(0)' link-type='store-point' store-type='1' virtual-address-code='769AJ' terminal-code='769AJ' terminal-type='1'><font color='red'>【东莞南城新基长生水营业点】</font></a>","stayWhyCode":""},
                {"scanDateTime":"2015-12-26 06:13:56","remark":"快件到达 <a style='color:red;text-decoration:none;' href='javascript:void(0)' link-type='store-point' store-type='1' virtual-address-code='769AJ' terminal-code='769AJ' terminal-type='1'><font color='red'>【东莞南城新基长生水营业点】</font></a>","stayWhyCode":""},
                {"scanDateTime":"2015-12-26 08:26:10","remark":"正在派送途中,请您准备签收<br/>(派件人:张志锋,<span id=\"opr_phone\">电话:</span>18820757557)","stayWhyCode":""},
                {"scanDateTime":"2015-12-26 10:07:14","remark":"已签收,感谢使用顺丰,期待再次为您服务","stayWhyCode":""},
                {"scanDateTime":"2015-12-26 10:07:14","remark":"在官网\"运单资料&签收图\",可查看签收人信息","stayWhyCode":"1"}
            ],
            "prioritized":false,
            "warehouse":true,
            "signed":true,
            "expressState":"1",
            "lstElementHtml":null,
            "childSet":[],
            "showThermometer":false,
            "productCode":"SE0001",
            "productName":"顺丰标快",
            "billFlag":"1"
        }
        ]*/

        public override string Name
        {
            get { return "顺丰快递"; }
        }
        public override string SearchUrl
        {
            get { return "http://www.sf-express.com/sf-service-web/service/bills/{0}/routes?app=bill&lang=sc&region=cn&translate="; }
        }

        public override ILogisticsInfo[] ParseResult(string s)
        {
            return JsonValue.Deserialize<List<express>>(s)[0].routes.ToArray();
        }
    }
}
