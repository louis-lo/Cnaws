using Cnaws.Json;
using Cnaws.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Xml;

namespace Cnaws.Pay.Providers
{
    internal sealed class Wxpay : PayProvider
    {
        internal enum ResultType
        {
            NotMatch,
            Failed,
            Success
        }
        internal sealed class WxPayException : Exception
        {
            public WxPayException(string msg)
                : base(msg)
            {
            }
        }
        internal sealed class WxPayConfig
        {
            private Wxpay _pay;

            public WxPayConfig(Wxpay pay)
            {
                _pay = pay;
            }

            /// <summary>
            /// 绑定支付的APPID
            /// </summary>
            public string APPID
            {
                get { return _pay.Partner; }
            }
            /// <summary>
            /// 商户号
            /// </summary>
            public string MCHID
            {
                get { return _pay.PartnerId; }
            }
            /// <summary>
            /// 商户支付密钥，参考开户邮件设置
            /// </summary>
            public string KEY
            {
                get { return _pay.PartnerKey; }
            }
            /// <summary>
            /// 公众帐号secert
            /// </summary>
            public string APPSECRET
            {
                get { return _pay.PartnerSecret; }
            }

            /// <summary>
            /// 证书（仅退款、撤销订单时需要）
            /// </summary>
            public byte[] SSLCERT_DATA
            {
                get { return Properties.Resources.apiclient_cert; }
            }
            /// <summary>
            /// 证书密码（仅退款、撤销订单时需要）
            /// </summary>
            public string SSLCERT_PASSWORD
            {
                get { return "1233410002"; }
            }

            /// <summary>
            /// 支付结果通知回调url，用于商户接收支付结果
            /// </summary>
            public string NOTIFY_URL
            {
                get { return _pay.AsyncCallbackUrl; }
            }

            /// <summary>
            /// 商户系统后台机器IP
            /// </summary>
            public string IP
            {
                get { return HttpContext.Current.Request.ServerVariables["Local_Addr"]; }
            }

            ///// <summary>
            ///// 默认IP和端口号分别为0.0.0.0和0，此时不开启代理（如有需要才设置）
            ///// </summary>
            //public string PROXY_URL
            //{
            //    get { return "http://10.152.18.220:8080"; }
            //}

            /// <summary>
            /// 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
            /// </summary>
            public int REPORT_LEVENL
            {
                get { return 0; }
            }

            /// <summary>
            /// 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
            /// </summary>
            public int LOG_LEVENL
            {
                get { return 0; }
            }
        }
        internal sealed class WxPayLog
        {
            private Wxpay _pay;

            public WxPayLog(Wxpay pay)
            {
                _pay = pay;
            }

            private WxPayConfig WxPayConfig
            {
                get { return _pay.Config; }
            }

            ///// <summary>
            ///// 向日志文件写入调试信息
            ///// </summary>
            ///// <param name="className">类名</param>
            ///// <param name="content">写入内容</param>
            //public void Debug(string className, string content)
            //{
            //    if (WxPayConfig.LOG_LEVENL >= 3)
            //        WriteLog("DEBUG", className, content);
            //}

            ///// <summary>
            ///// 向日志文件写入运行时信息
            ///// </summary>
            ///// <param name="className">类名</param>
            ///// <param name="content">写入内容</param>
            //public void Info(string className, string content)
            //{
            //    if (WxPayConfig.LOG_LEVENL >= 2)
            //        WriteLog("INFO", className, content);
            //}

            ///// <summary>
            ///// 向日志文件写入出错信息
            ///// </summary>
            ///// <param name="className">类名</param>
            ///// <param name="content">写入内容</param>
            //public void Error(string className, string content)
            //{
            //    if (WxPayConfig.LOG_LEVENL >= 1)
            //        WriteLog("ERROR", className, content);
            //}

            public void Log(string prefix, string content)
            {
                _pay.WriteLog(_pay.TradeNo, string.Concat(prefix, ": ", content));
            }

            ///// <summary>
            ///// 实际的写日志操作
            ///// </summary>
            ///// <param name="type">日志记录类型</param>
            ///// <param name="className">类名</param>
            ///// <param name="content">写入内容</param>
            //private void WriteLog(string type, string className, string content)
            //{
            //    _pay.WriteLog(_pay.TradeNo, string.Concat(type, ' ', className, ": ", content));
            //}
        }
        internal sealed class WxPayData
        {
            private Wxpay _pay;
            private SortedDictionary<string, string> m_values;

            public WxPayData(Wxpay pay)
            {
                _pay = pay;
                m_values = new SortedDictionary<string, string>();
            }
            public WxPayData(Wxpay pay, SortedDictionary<string, string> data)
            {
                _pay = pay;
                m_values = data;
            }

            private WxPayConfig WxPayConfig
            {
                get { return _pay.Config; }
            }
            private WxPayLog Log
            {
                get { return _pay.LogEx; }
            }

            public string this[string key]
            {
                get { return GetValue(key); }
                set { SetValue(key, value); }
            }

            /// <summary>
            /// 设置某个字段的值
            /// </summary>
            /// <param name="key">字段名</param>
            /// <param name="value">字段值</param>
            public void SetValue(string key, string value)
            {
                m_values[key] = value;
            }

            /// <summary>
            /// 根据字段名获取某个字段的值
            /// </summary>
            /// <param name="key">字段名</param>
            /// <returns>key对应的字段值</returns>
            public string GetValue(string key)
            {
                string o;
                if (m_values.TryGetValue(key, out o))
                    return o;
                return null;
            }

            /// <summary>
            /// 判断某个字段是否已设置
            /// </summary>
            /// <param name="key">字段名</param>
            /// <returns>若字段key已被设置，则返回true，否则返回false</returns>
            public bool IsSet(string key)
            {
                string o = null;
                if (m_values.TryGetValue(key, out o))
                    return (!string.IsNullOrEmpty(o));
                return false;
            }
            public bool TryGetValue(string key, out string value)
            {
                return m_values.TryGetValue(key, out value);
            }

            /// <summary>
            /// 将Dictionary转成xml
            /// </summary>
            /// <exception cref="WxPayException"></exception>
            /// <returns>经转换得到的xml串</returns>
            public string ToXml()
            {
                //数据为空时不能转化为xml格式
                if (0 == m_values.Count)
                {
                    //Log.Error(GetType().ToString(), "WxPayData数据为空!");
                    throw new WxPayException("WxPayData数据为空!");
                }

                StringBuilder xml = new StringBuilder("<xml>");
                foreach (KeyValuePair<string, string> pair in m_values)
                {
                    //字段值不能为null，会影响后续流程
                    if (string.IsNullOrEmpty(pair.Value))
                    {
                        //Log.Error(GetType().ToString(), "WxPayData内部含有值为null的字段!");
                        throw new WxPayException("WxPayData内部含有值为null的字段!");
                    }

                    if ("total_fee".Equals(pair.Key) || "execute_time_".Equals(pair.Key))
                        xml.Append('<').Append(pair.Key).Append('>').Append(pair.Value).Append("</").Append(pair.Key).Append('>');
                    else
                        xml.Append('<').Append(pair.Key).Append("><![CDATA[").Append(pair.Value).Append("]]></").Append(pair.Key).Append('>');
                }
                xml.Append("</xml>");
                return xml.ToString();
            }

            /// <summary>
            /// 将xml转为WxPayData对象并返回对象内部的数据
            /// </summary>
            /// <param name="xml">待转换的xml串</param>
            /// <exception cref="WxPayException"></exception>
            /// <returns>经转换得到的Dictionary</returns>
            public static WxPayData FromXml(Wxpay pay, string xml)
            {
                if (string.IsNullOrEmpty(xml))
                {
                    //pay.LogEx.Error(TType<WxPayData>.Type.ToString(), "将空的xml串转换为WxPayData不合法!");
                    throw new WxPayException("将空的xml串转换为WxPayData不合法!");
                }

                WxPayData data = new WxPayData(pay);
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xml);
                    foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                        data[node.Name] = node.InnerText;//获取xml的键值对到WxPayData内部的数据中

                    //2015-06-29 错误是没有签名
                    if (data["return_code"] != "SUCCESS")
                        return data;
                    data.CheckSign();//验证签名,不通过会抛异常
                }
                catch (WxPayException ex)
                {
                    throw new WxPayException(ex.Message);
                }

                return data;
            }

            /// <summary>
            /// Dictionary格式转化成url参数格式
            /// </summary>
            /// <returns>url格式串, 该串不包含sign字段值</returns>
            public string ToUrl()
            {
                int index = 0;
                StringBuilder buff = new StringBuilder();
                foreach (KeyValuePair<string, string> pair in m_values)
                {
                    //if (pair.Value == null)
                    //{
                    //    //Log.Error(GetType().ToString(), "WxPayData内部含有值为null的字段!");
                    //    throw new WxPayException("WxPayData内部含有值为null的字段!");
                    //}

                    if (!"sign".Equals(pair.Key) && !string.IsNullOrEmpty(pair.Value))
                    {
                        if (index++ > 0)
                            buff.Append('&');
                        buff.Append(pair.Key).Append('=').Append(pair.Value);
                    }
                }
                return buff.ToString();
            }

            /// <summary>
            /// Dictionary格式化成Json
            /// </summary>
            /// <returns>json串数据</returns>
            public string ToJson()
            {
                return JsonValue.Serialize(m_values);
            }

            /// <summary>
            /// values格式化成能在Web页面上显示的结果（因为web页面上不能直接输出xml格式的字符串）
            /// </summary>
            /// <returns></returns>
            public string ToPrintStr()
            {
                StringBuilder str = new StringBuilder();
                foreach (KeyValuePair<string, string> pair in m_values)
                {
                    if (string.IsNullOrEmpty(pair.Value))
                    {
                        //Log.Error(GetType().ToString(), "WxPayData内部含有值为null的字段!");
                        throw new WxPayException("WxPayData内部含有值为null的字段!");
                    }
                    str.Append(pair.Key).Append('=').Append(pair.Value).Append("<br>");
                }
                //Log.Debug(GetType().ToString(), string.Concat("Print in Web Page : ", str));
                return str.ToString();
            }

            /// <summary>
            /// 生成签名，详见签名生成算法
            /// </summary>
            /// <returns>签名, sign字段不参加签名</returns>
            public string MakeSign()
            {
                //转url格式
                StringBuilder str = new StringBuilder(ToUrl());
                //在string后加入API KEY
                str.Append("&key=").Append(WxPayConfig.KEY);
                //MD5加密
                using (MD5 md5 = MD5.Create())
                {
                    byte[] bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str.ToString()));
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in bs)
                        sb.Append(b.ToString("x2"));
                    //所有字符转为大写
                    return sb.ToString().ToUpper();
                }
            }

            /// <summary>
            /// 检测签名是否正确
            /// </summary>
            /// <returns>正确返回true，错误抛异常</returns>
            public bool CheckSign()
            {
                //如果没有设置签名，则跳过检测
                if (!IsSet("sign"))
                {
                    //Log.Error(GetType().ToString(), "WxPayData签名存在但不合法!");
                    throw new WxPayException("WxPayData签名存在但不合法!");
                }
                //获取接收到的签名
                string return_sign;
                //如果设置了签名但是签名为空，则抛异常
                if (!m_values.TryGetValue("sign", out return_sign) || string.IsNullOrEmpty(return_sign))
                {
                    //Log.Error(GetType().ToString(), "WxPayData签名存在但不合法!");
                    throw new WxPayException("WxPayData签名存在但不合法!");
                }

                //在本地计算新的签名
                if (return_sign.Equals(MakeSign()))
                    return true;

                //Log.Error(GetType().ToString(), "WxPayData签名验证错误!");
                throw new WxPayException("WxPayData签名验证错误!");
            }

            /// <summary>
            /// 获取Dictionary
            /// </summary>
            /// <returns></returns>
            public SortedDictionary<string, string> GetValues()
            {
                return m_values;
            }
        }
        internal sealed class JsApiPay
        {
            /// <summary>
            /// 保存页面对象，因为要在类的方法中使用Page的Request对象
            /// </summary>
            private HttpContext page { get; set; }

            /// <summary>
            /// openid用于调用统一下单接口
            /// </summary>
            public string openid { get; set; }

            /// <summary>
            /// access_token用于获取收货地址js函数入口参数
            /// </summary>
            public string access_token { get; set; }

            public string subject { get; set; }

            /// <summary>
            /// 商品金额，用于统一下单
            /// </summary>
            public int total_fee { get; set; }

            public string trade_no { get; set; }

            /// <summary>
            /// 统一下单接口返回结果
            /// </summary>
            public WxPayData unifiedOrderResult { get; set; }

            private Wxpay _pay;

            public JsApiPay(Wxpay pay, HttpContext page)
            {
                _pay = pay;
                this.page = page;
            }

            private WxPayConfig WxPayConfig
            {
                get { return _pay.Config; }
            }
            private WxPayLog Log
            {
                get { return _pay.LogEx; }
            }

            /**
            * 
            * 网页授权获取用户基本信息的全部过程
            * 详情请参看网页授权获取用户基本信息：http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html
            * 第一步：利用url跳转获取code
            * 第二步：利用code去获取openid和access_token
            * 
            */
            public void GetOpenidAndAccessToken()
            {
                if (!string.IsNullOrEmpty(page.Request.QueryString["code"]))
                {
                    //获取code码，以获取openid和access_token
                    string code = page.Request.QueryString["code"];
                    //Log.Debug(GetType().ToString(), string.Concat("Get code : ", code));
                    GetOpenidAndAccessTokenFromCode(code);
                }
                else
                {
                    //构造网页授权获取code的URL
                    string host = page.Request.Url.Host;
                    string path = page.Request.Path;
                    string redirect_uri = HttpUtility.UrlEncode(string.Concat("http://", host, path));
                    WxPayData data = new WxPayData(_pay);
                    data.SetValue("appid", WxPayConfig.APPID);
                    data.SetValue("redirect_uri", redirect_uri);
                    data.SetValue("response_type", "code");
                    data.SetValue("scope", "snsapi_base");
                    data.SetValue("state", string.Concat("STATE", "#wechat_redirect"));
                    string url = string.Concat("https://open.weixin.qq.com/connect/oauth2/authorize?", data.ToUrl());
                    //Log.Debug(GetType().ToString(), string.Concat("Will Redirect to URL : ", url));
                    try
                    {
                        //触发微信返回code码         
                        page.Response.Redirect(url);//Redirect函数会抛出ThreadAbortException异常，不用处理这个异常
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                    }
                }
            }


            /**
            * 
            * 通过code换取网页授权access_token和openid的返回数据，正确时返回的JSON数据包如下：
            * {
            *  "access_token":"ACCESS_TOKEN",
            *  "expires_in":7200,
            *  "refresh_token":"REFRESH_TOKEN",
            *  "openid":"OPENID",
            *  "scope":"SCOPE",
            *  "unionid": "o6_bmasdasdsad6_2sgVt7hMZOPfL"
            * }
            * 其中access_token可用于获取共享收货地址
            * openid是微信支付jsapi支付接口统一下单时必须的参数
            * 更详细的说明请参考网页授权获取用户基本信息：http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html
            * @失败时抛异常WxPayException
            */
            public void GetOpenidAndAccessTokenFromCode(string code)
            {
                try
                {
                    //构造获取openid及access_token的url
                    WxPayData data = new WxPayData(_pay);
                    data.SetValue("appid", WxPayConfig.APPID);
                    data.SetValue("secret", WxPayConfig.APPSECRET);
                    data.SetValue("code", code);
                    data.SetValue("grant_type", "authorization_code");
                    string url = string.Concat("https://api.weixin.qq.com/sns/oauth2/access_token?", data.ToUrl());

                    //请求url以获取数据
                    string result = HttpService.Get(_pay, url);

                    //Log.Debug(GetType().ToString(), string.Concat("GetOpenidAndAccessTokenFromCode response : ", result));

                    //保存access_token，用于收货地址获取
                    JsonValue json = JsonValue.LoadJson(result);
                    JsonObject jd = (JsonObject)json;
                    access_token = (JsonString)jd["access_token"];

                    //获取用户openid
                    openid = (JsonString)jd["openid"];

                    //Log.Debug(GetType().ToString(), string.Concat("Get openid : ", openid));
                    //Log.Debug(GetType().ToString(), string.Concat("Get access_token : ", access_token));
                }
                catch (Exception ex)
                {
                    //Log.Error(GetType().ToString(), ex.ToString());
                    throw new WxPayException(ex.ToString());
                }
            }

            /**
             * 调用统一下单，获得下单结果
             * @return 统一下单结果
             * @失败时抛异常WxPayException
             */
            public WxPayData GetUnifiedOrderResult()
            {
                //统一下单
                //DateTime now = DateTime.Now;
                WxPayData data = new WxPayData(_pay);
                data.SetValue("body", subject);//商品或支付单简要描述
                //data.SetValue("attach", "test");//附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据
                data.SetValue("out_trade_no", trade_no);//商户系统内部的订单号,32个字符内、可包含字母
                data.SetValue("total_fee", total_fee.ToString());//订单总金额，单位为分
                //data.SetValue("time_start", now.ToString("yyyyMMddHHmmss"));//订单生成时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010
                //data.SetValue("time_expire", now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//订单失效时间，格式为yyyyMMddHHmmss，如2009年12月27日9点10分10秒表示为20091227091010
                //data.SetValue("goods_tag", "test");//商品标记，代金券或立减优惠功能的参数
                data.SetValue("trade_type", "JSAPI");//取值如下：JSAPI，NATIVE，APP
                data.SetValue("openid", openid);//用户在商户appid下的唯一标识

                WxPayData result = WxPayApi.UnifiedOrder(_pay, data);
                if (!result.IsSet("appid") || !result.IsSet("prepay_id"))
                {
                    //Log.Error(GetType().ToString(), "UnifiedOrder response error!");
                    throw new WxPayException("UnifiedOrder response error!");
                }

                unifiedOrderResult = result;
                return result;
            }

            /**
            *  
            * 从统一下单成功返回的数据中获取微信浏览器调起jsapi支付所需的参数，
            * 微信浏览器调起JSAPI时的输入参数格式如下：
            * {
            *   "appId" : "wx2421b1c4370ec43b",     //公众号名称，由商户传入     
            *   "timeStamp":" 1395712654",         //时间戳，自1970年以来的秒数     
            *   "nonceStr" : "e61463f8efa94090b1f366cccfbbb444", //随机串     
            *   "package" : "prepay_id=u802345jgfjsdfgsdg888",     
            *   "signType" : "MD5",         //微信签名方式:    
            *   "paySign" : "70EA570631E4BB79628FBCA90534C63FF7FADD89" //微信签名 
            * }
            * @return string 微信浏览器调起JSAPI时的输入参数，json格式可以直接做参数用
            * 更详细的说明请参考网页端调起支付API：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_7
            * 
            */
            public WxPayData GetJsApiParameters()
            {
                //Log.Debug(GetType().ToString(), "JsApiPay::GetJsApiParam is processing...");

                WxPayData jsApiParam = new WxPayData(_pay);
                jsApiParam.SetValue("appId", unifiedOrderResult.GetValue("appid"));
                jsApiParam.SetValue("timeStamp", WxPayApi.GenerateTimeStamp());
                jsApiParam.SetValue("nonceStr", WxPayApi.GenerateNonceStr());
                jsApiParam.SetValue("package", string.Concat("prepay_id=", unifiedOrderResult.GetValue("prepay_id")));
                jsApiParam.SetValue("signType", "MD5");
                jsApiParam.SetValue("paySign", jsApiParam.MakeSign());

                //string parameters = jsApiParam.ToJson();
                //Log.Debug(GetType().ToString(), string.Concat("Get jsApiParam : ", parameters));
                //return parameters;

                return jsApiParam;
            }


            /**
            * 
            * 获取收货地址js函数入口参数,详情请参考收货地址共享接口：http://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=7_9
            * @return string 共享收货地址js函数需要的参数，json格式可以直接做参数使用
            */
            public string GetEditAddressParameters()
            {
                string parameter = "";
                try
                {
                    string host = page.Request.Url.Host;
                    string path = page.Request.Path;
                    string queryString = page.Request.Url.Query;
                    //这个地方要注意，参与签名的是网页授权获取用户信息时微信后台回传的完整url
                    string url = string.Concat("http://", host, path, queryString);

                    //构造需要用SHA1算法加密的数据
                    WxPayData signData = new WxPayData(_pay);
                    signData.SetValue("appid", WxPayConfig.APPID);
                    signData.SetValue("url", url);
                    signData.SetValue("timestamp", WxPayApi.GenerateTimeStamp());
                    signData.SetValue("noncestr", WxPayApi.GenerateNonceStr());
                    signData.SetValue("accesstoken", access_token);
                    string param = signData.ToUrl();

                    //Log.Debug(GetType().ToString(), string.Concat("SHA1 encrypt param : ", param));
                    //SHA1加密
                    string addrSign = FormsAuthentication.HashPasswordForStoringInConfigFile(param, "SHA1");
                    //Log.Debug(GetType().ToString(), string.Concat("SHA1 encrypt result : ", addrSign));

                    //获取收货地址js函数入口参数
                    WxPayData afterData = new WxPayData(_pay);
                    afterData.SetValue("appId", WxPayConfig.APPID);
                    afterData.SetValue("scope", "jsapi_address");
                    afterData.SetValue("signType", "sha1");
                    afterData.SetValue("addrSign", addrSign);
                    afterData.SetValue("timeStamp", signData.GetValue("timestamp"));
                    afterData.SetValue("nonceStr", signData.GetValue("noncestr"));

                    //转为json格式
                    parameter = afterData.ToJson();
                    //Log.Debug(GetType().ToString(), string.Concat("Get EditAddressParam : ", parameter));
                }
                catch (Exception ex)
                {
                    //Log.Error(GetType().ToString(), ex.ToString());
                    throw new WxPayException(ex.ToString());
                }

                return parameter;
            }
        }
        internal sealed class NativePay
        {
            private Wxpay _pay;

            public NativePay(Wxpay pay)
            {
                _pay = pay;
            }

            private WxPayConfig WxPayConfig
            {
                get { return _pay.Config; }
            }
            private WxPayLog Log
            {
                get { return _pay.LogEx; }
            }

            public string subject { get; set; }
            /// <summary>
            /// 商品金额，用于统一下单
            /// </summary>
            public int total_fee { get; set; }

            public string trade_no { get; set; }

            ///**
            //* 生成扫描支付模式一URL
            //* @param productId 商品ID
            //* @return 模式一URL
            //*/
            //public string GetPrePayUrl(string productId)
            //{
            //    //Log.Info(this.GetType().ToString(), "Native pay mode 1 url is producing...");

            //    WxPayData data = new WxPayData(_pay);
            //    data.SetValue("appid", WxPayConfig.APPID);//公众帐号id
            //    data.SetValue("mch_id", WxPayConfig.MCHID);//商户号
            //    data.SetValue("time_stamp", WxPayApi.GenerateTimeStamp());//时间戳
            //    data.SetValue("nonce_str", WxPayApi.GenerateNonceStr());//随机字符串
            //    data.SetValue("product_id", productId);//商品ID
            //    data.SetValue("sign", data.MakeSign());//签名
            //    string str = ToUrlParams(data.GetValues());//转换为URL串
            //    string url = "weixin://wxpay/bizpayurl?" + str;

            //    //Log.Info(this.GetType().ToString(), "Get native pay mode 1 url : " + url);
            //    return url;
            //}

            /**
            * 生成直接支付url，支付url有效期为2小时,模式二
            * @param productId 商品ID
            * @return 模式二URL
            */
            public string GetPayUrl()
            {
                //Log.Info(this.GetType().ToString(), "Native pay mode 2 url is producing...");

                WxPayData data = new WxPayData(_pay);
                data.SetValue("body", subject);//商品描述
                //data.SetValue("attach", "test");//附加数据
                data.SetValue("out_trade_no", trade_no);//随机字符串
                data.SetValue("total_fee", total_fee.ToString());//总金额
                //data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
                //data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
                //data.SetValue("goods_tag", "jjj");//商品标记
                data.SetValue("trade_type", "NATIVE");//交易类型
                data.SetValue("product_id", trade_no);//商品ID

                WxPayData result = WxPayApi.UnifiedOrder(_pay, data);//调用统一下单接口
                string url = result.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接

                //Log.Info(this.GetType().ToString(), "Get native pay mode 2 url : " + url);
                return url;
            }

            ///**
            //* 参数数组转换为url格式
            //* @param map 参数名与参数值的映射表
            //* @return URL字符串
            //*/
            //private string ToUrlParams(SortedDictionary<string, string> map)
            //{
            //    int index = 0;
            //    StringBuilder buff = new StringBuilder();
            //    foreach (KeyValuePair<string, string> pair in map)
            //    {
            //        if (index++ > 0)
            //            buff.Append('&');
            //        buff.Append(pair.Key).Append('=').Append(pair.Value);
            //    }
            //    return buff.ToString();
            //}
        }
        internal abstract class Notify
        {
            public Controller page { get; set; }

            protected Wxpay _pay;

            protected Notify(Wxpay pay, Controller page)
            {
                _pay = pay;
                this.page = page;
            }

            protected WxPayConfig WxPayConfig
            {
                get { return _pay.Config; }
            }
            protected WxPayLog Log
            {
                get { return _pay.LogEx; }
            }

            /// <summary>
            /// 接收从微信支付后台发送过来的数据并验证签名
            /// </summary>
            /// <returns>微信支付后台返回的数据</returns>
            public static bool GetNotifyData(Wxpay pay, Controller page, out WxPayData data)
            {
                StringBuilder builder = new StringBuilder();
                //接收从微信后台POST过来的数据
                using (Stream s = page.Request.InputStream)
                {
                    int count;
                    byte[] buffer = new byte[1024];
                    while ((count = s.Read(buffer, 0, 1024)) > 0)
                        builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
                }

                //Log.Info(GetType().ToString(), string.Concat("Receive data from WeChat : ", builder.ToString()));
                pay.LogEx.Log("Receive data from WeChat", builder.ToString());

                //转换数据格式并验证签名
                try
                {
                    data = WxPayData.FromXml(pay, builder.ToString());
                    return true;
                }
                catch (WxPayException ex)
                {
                    //若签名错误，则立即返回结果给微信支付后台
                    data = new WxPayData(pay);
                    data.SetValue("return_code", "FAIL");
                    data.SetValue("return_msg", ex.Message);
                    //Log.Error(GetType().ToString(), string.Concat("Sign check error : ", data.ToXml()));
                    //page.Response.Write(data.ToXml());
                    //page.Response.End();
                    return false;
                }

                //Log.Info(GetType().ToString(), "Check sign success");
                //return data;
            }

            //派生类需要重写这个方法，进行不同的回调处理
            public abstract ResultType ProcessNotify(WxPayData notifyData, out WxPayData res);
        }
        //internal sealed class NativeNotify : Notify
        //{
        //    public NativeNotify(Wxpay pay, HttpContext page)
        //        : base(pay, page)
        //    {
        //    }

        //    public override ResultType ProcessNotify(WxPayData notifyData, out WxPayData res)
        //    {
        //        //WxPayData notifyData = GetNotifyData();

        //        //检查openid和product_id是否返回
        //        if (!notifyData.IsSet("openid") || !notifyData.IsSet("product_id"))
        //        {
        //            res = new WxPayData(_pay);
        //            res.SetValue("return_code", "FAIL");
        //            res.SetValue("return_msg", "回调数据异常");
        //            //Log.Info(GetType().ToString(), string.Concat("The data WeChat post is error : ", res.ToXml()));
        //            //page.Response.Write(res.ToXml());
        //            //page.Response.End();
        //            return ResultType.NotMatch;
        //        }

        //        //调统一下单接口，获得下单结果
        //        string openid = notifyData.GetValue("openid");
        //        string product_id = notifyData.GetValue("product_id");
        //        WxPayData unifiedOrderResult = new WxPayData(_pay);
        //        try
        //        {
        //            unifiedOrderResult = UnifiedOrder(openid, product_id);
        //        }
        //        catch (Exception)//若在调统一下单接口时抛异常，立即返回结果给微信支付后台
        //        {
        //            WxPayData res = new WxPayData(_pay);
        //            res.SetValue("return_code", "FAIL");
        //            res.SetValue("return_msg", "统一下单失败");
        //            Log.Error(GetType().ToString(), string.Concat("UnifiedOrder failure : ", res.ToXml()));
        //            page.Response.Write(res.ToXml());
        //            page.Response.End();
        //            return;
        //        }

        //        //若下单失败，则立即返回结果给微信支付后台
        //        if (!unifiedOrderResult.IsSet("appid") || !unifiedOrderResult.IsSet("mch_id") || !unifiedOrderResult.IsSet("prepay_id"))
        //        {
        //            WxPayData res = new WxPayData(_pay);
        //            res.SetValue("return_code", "FAIL");
        //            res.SetValue("return_msg", "统一下单失败");
        //            Log.Error(GetType().ToString(), string.Concat("UnifiedOrder failure : ", res.ToXml()));
        //            page.Response.Write(res.ToXml());
        //            page.Response.End();
        //            return;
        //        }

        //        //统一下单成功,则返回成功结果给微信支付后台
        //        WxPayData data = new WxPayData(_pay);
        //        data.SetValue("return_code", "SUCCESS");
        //        data.SetValue("return_msg", "OK");
        //        data.SetValue("appid", WxPayConfig.APPID);
        //        data.SetValue("mch_id", WxPayConfig.MCHID);
        //        data.SetValue("nonce_str", WxPayApi.GenerateNonceStr());
        //        data.SetValue("prepay_id", unifiedOrderResult.GetValue("prepay_id"));
        //        data.SetValue("result_code", "SUCCESS");
        //        data.SetValue("err_code_des", "OK");
        //        data.SetValue("sign", data.MakeSign());

        //        Log.Info(GetType().ToString(), string.Concat("UnifiedOrder success , send data to WeChat : ", data.ToXml()));
        //        page.Response.Write(data.ToXml());
        //        page.Response.End();
        //    }

        //    private WxPayData UnifiedOrder(string openId, string productId)
        //    {
        //        //统一下单
        //        WxPayData req = new WxPayData(_pay);
        //        req.SetValue("body", "test");
        //        req.SetValue("attach", "test");
        //        req.SetValue("out_trade_no", WxPayApi.GenerateOutTradeNo(_pay));
        //        req.SetValue("total_fee", 1);
        //        req.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
        //        req.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
        //        req.SetValue("goods_tag", "test");
        //        req.SetValue("trade_type", "NATIVE");
        //        req.SetValue("openid", openId);
        //        req.SetValue("product_id", productId);
        //        WxPayData result = WxPayApi.UnifiedOrder(_pay, req);
        //        return result;
        //    }
        //}
        internal sealed class ResultNotify : Notify
        {
            public ResultNotify(Wxpay pay, Controller page)
                : base(pay, page)
            {
            }

            public override ResultType ProcessNotify(WxPayData notifyData, out WxPayData res)
            {
                //WxPayData notifyData = GetNotifyData();

                string transaction_id;
                //检查支付结果中transaction_id是否存在
                if (!notifyData.TryGetValue("transaction_id", out transaction_id))
                {
                    //若transaction_id不存在，则立即返回结果给微信支付后台
                    res = new WxPayData(_pay);
                    res.SetValue("return_code", "FAIL");
                    res.SetValue("return_msg", "支付结果中微信订单号不存在");
                    //Log.Error(GetType().ToString(), string.Concat("The Pay result is error : ", res.ToXml()));
                    //page.Response.Write(res.ToXml());
                    //page.Response.End();
                    return ResultType.NotMatch;
                }

                //查询订单，判断订单真实性
                if (!QueryOrder(transaction_id))
                {
                    //若订单查询失败，则立即返回结果给微信支付后台
                    res = new WxPayData(_pay);
                    res.SetValue("return_code", "FAIL");
                    res.SetValue("return_msg", "订单查询失败");
                    //Log.Error(GetType().ToString(), string.Concat("Order query failure : ", res.ToXml()));
                    //page.Response.Write(res.ToXml());
                    //page.Response.End();
                    return ResultType.Failed;
                }
                //查询订单成功
                else
                {
                    res = new WxPayData(_pay);
                    res.SetValue("return_code", "SUCCESS");
                    res.SetValue("return_msg", "OK");
                    //Log.Info(GetType().ToString(), string.Concat("order query success : ", res.ToXml()));
                    //page.Response.Write(res.ToXml());
                    //page.Response.End();
                    return ResultType.Success;
                }
            }

            //查询订单
            private bool QueryOrder(string transaction_id)
            {
                WxPayData req = new WxPayData(_pay);
                req.SetValue("transaction_id", transaction_id);
                WxPayData res = WxPayApi.OrderQuery(_pay, req);
                if (res.GetValue("return_code") == "SUCCESS" && res.GetValue("result_code") == "SUCCESS")
                    return true;
                return false;
            }
        }
        internal static class HttpService
        {
            public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                //直接确认，否则打不开    
                return true;
            }

            public static string Post(Wxpay pay, string xml, string url, bool isUseCert, int timeout)
            {
                //System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接

                //string result = "";//返回结果

                //HttpWebRequest request = null;
                //HttpWebResponse response = null;
                //Stream reqStream = null;

                //try
                //{
                //设置最大连接数
                //ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.Timeout = timeout * 1000;

                //设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);              //网关服务器端口:端口
                //request.Proxy = proxy;

                //设置POST的数据类型和长度
                request.ContentType = "text/xml";
                byte[] data = Encoding.UTF8.GetBytes(xml);
                request.ContentLength = data.Length;

                //是否使用证书
                if (isUseCert)
                {
                    //string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    //X509Certificate2 cert = new X509Certificate2(path + WxPayConfig.SSLCERT_PATH, WxPayConfig.SSLCERT_PASSWORD);
                    X509Certificate2 cert = new X509Certificate2(pay.Config.SSLCERT_DATA, pay.Config.SSLCERT_PASSWORD);
                    request.ClientCertificates.Add(cert);
                    //pay.LogEx.Debug("WxPayApi", "PostXml used cert");
                }

                //往服务器写入数据
                using (Stream reqStream = request.GetRequestStream())
                    reqStream.Write(data, 0, data.Length);

                //获取服务端返回
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream s = response.GetResponseStream())
                    {
                        //获取服务端返回数据
                        using (StreamReader sr = new StreamReader(s, Encoding.UTF8))
                            return sr.ReadToEnd().Trim();
                    }
                }
                //}
                //catch (ThreadAbortException e)
                //{
                //    pay.LogEx.Error("HttpService", "Thread - caught ThreadAbortException - resetting.");
                //    pay.LogEx.Error("Exception message: {0}", e.Message);
                //    //Thread.ResetAbort();
                //    throw new WxPayException(e.ToString());
                //}
                //catch (WebException e)
                //{
                //    pay.LogEx.Error("HttpService", e.ToString());
                //    if (e.Status == WebExceptionStatus.ProtocolError)
                //    {
                //        pay.LogEx.Error("HttpService", string.Concat("StatusCode : ", ((HttpWebResponse)e.Response).StatusCode));
                //        pay.LogEx.Error("HttpService", string.Concat("StatusDescription : ", ((HttpWebResponse)e.Response).StatusDescription));
                //    }
                //    throw new WxPayException(e.ToString());
                //}
                //catch (Exception e)
                //{
                //    pay.LogEx.Error("HttpService", e.ToString());
                //    throw new WxPayException(e.ToString());
                //}
            }

            /// <summary>
            /// 处理http GET请求，返回数据
            /// </summary>
            /// <param name="url">请求的url地址</param>
            /// <returns>http GET成功后返回的数据，失败抛WebException异常</returns>
            public static string Get(Wxpay pay, string url)
            {
                //System.GC.Collect();
                //string result = "";

                //HttpWebRequest request = null;
                //HttpWebResponse response = null;

                //请求url以获取数据
                //try
                //{
                //设置最大连接数
                //ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";

                //设置代理
                //WebProxy proxy = new WebProxy();
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);
                //request.Proxy = proxy;

                //获取服务器返回
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream s = response.GetResponseStream())
                    {
                        //获取HTTP返回数据
                        using (StreamReader sr = new StreamReader(s, Encoding.UTF8))
                            return sr.ReadToEnd().Trim();
                    }
                }
                //}
                //catch (ThreadAbortException e)
                //{
                //    pay.LogEx.Error("HttpService", "Thread - caught ThreadAbortException - resetting.");
                //    pay.LogEx.Error("Exception message: {0}", e.Message);
                //    //Thread.ResetAbort();
                //    throw new WxPayException(e.ToString());
                //}
                //catch (WebException e)
                //{
                //    pay.LogEx.Error("HttpService", e.ToString());
                //    if (e.Status == WebExceptionStatus.ProtocolError)
                //    {
                //        pay.LogEx.Error("HttpService", string.Concat("StatusCode : ", ((HttpWebResponse)e.Response).StatusCode));
                //        pay.LogEx.Error("HttpService", string.Concat("StatusDescription : ", ((HttpWebResponse)e.Response).StatusDescription));
                //    }
                //    throw new WxPayException(e.ToString());
                //}
                //catch (Exception e)
                //{
                //    pay.LogEx.Error("HttpService", e.ToString());
                //    throw new WxPayException(e.ToString());
                //}
            }
        }
        internal static class WxPayApi
        {
            /**
            * 提交被扫支付API
            * 收银员使用扫码设备读取微信用户刷卡授权码以后，二维码或条码信息传送至商户收银台，
            * 由商户收银台或者商户后台调用该接口发起支付。
            * @param WxPayData inputObj 提交给被扫支付API的参数
            * @param int timeOut 超时时间
            * @throws WxPayException
            * @return 成功时返回调用结果，其他抛异常
            */
            public static WxPayData Micropay(Wxpay pay, WxPayData inputObj, int timeOut = 10)
            {
                string url = "https://api.mch.weixin.qq.com/pay/micropay";

                //检测必填参数
                if (!inputObj.IsSet("body"))
                    throw new WxPayException("提交被扫支付API接口中，缺少必填参数body！");
                else if (!inputObj.IsSet("out_trade_no"))
                    throw new WxPayException("提交被扫支付API接口中，缺少必填参数out_trade_no！");
                else if (!inputObj.IsSet("total_fee"))
                    throw new WxPayException("提交被扫支付API接口中，缺少必填参数total_fee！");
                else if (!inputObj.IsSet("auth_code"))
                    throw new WxPayException("提交被扫支付API接口中，缺少必填参数auth_code！");

                inputObj.SetValue("spbill_create_ip", pay.Config.IP);//终端ip
                inputObj.SetValue("appid", pay.Config.APPID);//公众账号ID
                inputObj.SetValue("mch_id", pay.Config.MCHID);//商户号
                inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
                inputObj.SetValue("sign", inputObj.MakeSign());//签名
                string xml = inputObj.ToXml();

                //var start = DateTime.Now;//请求开始时间

                //pay.LogEx.Debug("WxPayApi", string.Concat("MicroPay request : ", xml));
                pay.LogEx.Log("MicroPay request", xml);
                string response = HttpService.Post(pay, xml, url, false, timeOut);//调用HTTP通信接口以提交数据到API
                pay.LogEx.Log("MicroPay response", response);
                //pay.LogEx.Debug("WxPayApi", string.Concat("MicroPay response : ", response));

                //var end = DateTime.Now;
                //int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

                //将xml格式的结果转换为对象以返回
                WxPayData result = WxPayData.FromXml(pay, response);

                //ReportCostTime(pay, url, timeCost, result);//测速上报

                return result;
            }


            /**
            *    
            * 查询订单
            * @param WxPayData inputObj 提交给查询订单API的参数
            * @param int timeOut 超时时间
            * @throws WxPayException
            * @return 成功时返回订单查询结果，其他抛异常
            */
            public static WxPayData OrderQuery(Wxpay pay, WxPayData inputObj, int timeOut = 6)
            {
                string url = "https://api.mch.weixin.qq.com/pay/orderquery";
                //检测必填参数
                if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
                    throw new WxPayException("订单查询接口中，out_trade_no、transaction_id至少填一个！");

                inputObj.SetValue("appid", pay.Config.APPID);//公众账号ID
                inputObj.SetValue("mch_id", pay.Config.MCHID);//商户号
                inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
                inputObj.SetValue("sign", inputObj.MakeSign());//签名

                string xml = inputObj.ToXml();

                //var start = DateTime.Now;

                //pay.LogEx.Debug("WxPayApi", string.Concat("OrderQuery request : ", xml));
                pay.LogEx.Log("OrderQuery request", xml);
                string response = HttpService.Post(pay, xml, url, false, timeOut);//调用HTTP通信接口提交数据
                pay.LogEx.Log("OrderQuery response", response);
                //pay.LogEx.Debug("WxPayApi", string.Concat("OrderQuery response : ", response));

                //var end = DateTime.Now;
                //int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

                //将xml格式的数据转化为对象以返回
                WxPayData result = WxPayData.FromXml(pay, response);

                //ReportCostTime(pay, url, timeCost, result);//测速上报

                return result;
            }


            /**
            * 
            * 撤销订单API接口
            * @param WxPayData inputObj 提交给撤销订单API接口的参数，out_trade_no和transaction_id必填一个
            * @param int timeOut 接口超时时间
            * @throws WxPayException
            * @return 成功时返回API调用结果，其他抛异常
            */
            public static WxPayData Reverse(Wxpay pay, WxPayData inputObj, int timeOut = 6)
            {
                string url = "https://api.mch.weixin.qq.com/secapi/pay/reverse";
                //检测必填参数
                if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
                    throw new WxPayException("撤销订单API接口中，参数out_trade_no和transaction_id必须填写一个！");

                inputObj.SetValue("appid", pay.Config.APPID);//公众账号ID
                inputObj.SetValue("mch_id", pay.Config.MCHID);//商户号
                inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
                inputObj.SetValue("sign", inputObj.MakeSign());//签名
                string xml = inputObj.ToXml();

                //var start = DateTime.Now;//请求开始时间

                //pay.LogEx.Debug("WxPayApi", string.Concat("Reverse request : ", xml));
                pay.LogEx.Log("Reverse request", xml);
                string response = HttpService.Post(pay, xml, url, true, timeOut);
                pay.LogEx.Log("Reverse response", response);
                //pay.LogEx.Debug("WxPayApi", string.Concat("Reverse response : ", response));

                //var end = DateTime.Now;
                //int timeCost = (int)((end - start).TotalMilliseconds);

                WxPayData result = WxPayData.FromXml(pay, response);

                //ReportCostTime(pay, url, timeCost, result);//测速上报

                return result;
            }


            /**
            * 
            * 申请退款
            * @param WxPayData inputObj 提交给申请退款API的参数
            * @param int timeOut 超时时间
            * @throws WxPayException
            * @return 成功时返回接口调用结果，其他抛异常
            */
            public static WxPayData Refund(Wxpay pay, WxPayData inputObj, int timeOut = 6)
            {
                string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
                //检测必填参数
                if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
                    throw new WxPayException("退款申请接口中，out_trade_no、transaction_id至少填一个！");
                else if (!inputObj.IsSet("out_refund_no"))
                    throw new WxPayException("退款申请接口中，缺少必填参数out_refund_no！");
                else if (!inputObj.IsSet("total_fee"))
                    throw new WxPayException("退款申请接口中，缺少必填参数total_fee！");
                else if (!inputObj.IsSet("refund_fee"))
                    throw new WxPayException("退款申请接口中，缺少必填参数refund_fee！");
                else if (!inputObj.IsSet("op_user_id"))
                    throw new WxPayException("退款申请接口中，缺少必填参数op_user_id！");

                inputObj.SetValue("appid", pay.Config.APPID);//公众账号ID
                inputObj.SetValue("mch_id", pay.Config.MCHID);//商户号
                inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
                inputObj.SetValue("sign", inputObj.MakeSign());//签名

                string xml = inputObj.ToXml();
                //var start = DateTime.Now;

                //pay.LogEx.Debug("WxPayApi", string.Concat("Refund request : ", xml));
                pay.LogEx.Log("Refund request", xml);
                string response = HttpService.Post(pay, xml, url, true, timeOut);//调用HTTP通信接口提交数据到API
                pay.LogEx.Log("Refund response", response);
                //pay.LogEx.Debug("WxPayApi", string.Concat("Refund response : ", response));

                //var end = DateTime.Now;
                //int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

                //将xml格式的结果转换为对象以返回
                WxPayData result = WxPayData.FromXml(pay, response);

                //ReportCostTime(pay, url, timeCost, result);//测速上报

                return result;
            }


            /**
            * 
            * 查询退款
            * 提交退款申请后，通过该接口查询退款状态。退款有一定延时，
            * 用零钱支付的退款20分钟内到账，银行卡支付的退款3个工作日后重新查询退款状态。
            * out_refund_no、out_trade_no、transaction_id、refund_id四个参数必填一个
            * @param WxPayData inputObj 提交给查询退款API的参数
            * @param int timeOut 接口超时时间
            * @throws WxPayException
            * @return 成功时返回，其他抛异常
            */
            public static WxPayData RefundQuery(Wxpay pay, WxPayData inputObj, int timeOut = 6)
            {
                string url = "https://api.mch.weixin.qq.com/pay/refundquery";
                //检测必填参数
                if (!inputObj.IsSet("out_refund_no") && !inputObj.IsSet("out_trade_no") &&
                    !inputObj.IsSet("transaction_id") && !inputObj.IsSet("refund_id"))
                {
                    throw new WxPayException("退款查询接口中，out_refund_no、out_trade_no、transaction_id、refund_id四个参数必填一个！");
                }

                inputObj.SetValue("appid", pay.Config.APPID);//公众账号ID
                inputObj.SetValue("mch_id", pay.Config.MCHID);//商户号
                inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
                inputObj.SetValue("sign", inputObj.MakeSign());//签名

                string xml = inputObj.ToXml();

                //var start = DateTime.Now;//请求开始时间

                //pay.LogEx.Debug("WxPayApi", string.Concat("RefundQuery request : ", xml));
                pay.LogEx.Log("RefundQuery request", xml);
                string response = HttpService.Post(pay, xml, url, false, timeOut);//调用HTTP通信接口以提交数据到API
                pay.LogEx.Log("RefundQuery response", response);
                //pay.LogEx.Debug("WxPayApi", string.Concat("RefundQuery response : ", response));

                //var end = DateTime.Now;
                //int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

                //将xml格式的结果转换为对象以返回
                WxPayData result = WxPayData.FromXml(pay, response);

                //ReportCostTime(pay, url, timeCost, result);//测速上报

                return result;
            }


            /**
            * 下载对账单
            * @param WxPayData inputObj 提交给下载对账单API的参数
            * @param int timeOut 接口超时时间
            * @throws WxPayException
            * @return 成功时返回，其他抛异常
            */
            public static WxPayData DownloadBill(Wxpay pay, WxPayData inputObj, int timeOut = 6)
            {
                string url = "https://api.mch.weixin.qq.com/pay/downloadbill";
                //检测必填参数
                if (!inputObj.IsSet("bill_date"))
                    throw new WxPayException("对账单接口中，缺少必填参数bill_date！");

                inputObj.SetValue("appid", pay.Config.APPID);//公众账号ID
                inputObj.SetValue("mch_id", pay.Config.MCHID);//商户号
                inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
                inputObj.SetValue("sign", inputObj.MakeSign());//签名

                string xml = inputObj.ToXml();

                //pay.LogEx.Debug("WxPayApi", string.Concat("DownloadBill request : ", xml));
                pay.LogEx.Log("DownloadBill request", xml);
                string response = HttpService.Post(pay, xml, url, false, timeOut);//调用HTTP通信接口以提交数据到API
                pay.LogEx.Log("DownloadBill response", response);
                //pay.LogEx.Debug("WxPayApi", string.Concat("DownloadBill result : ", response));

                WxPayData result = new WxPayData(pay);
                //若接口调用失败会返回xml格式的结果
                if (response.Substring(0, 5) == "<xml>")
                    result = WxPayData.FromXml(pay, response);
                //接口调用成功则返回非xml格式的数据
                else
                    result.SetValue("result", response);

                return result;
            }


            /**
            * 
            * 转换短链接
            * 该接口主要用于扫码原生支付模式一中的二维码链接转成短链接(weixin://wxpay/s/XXXXXX)，
            * 减小二维码数据量，提升扫描速度和精确度。
            * @param WxPayData inputObj 提交给转换短连接API的参数
            * @param int timeOut 接口超时时间
            * @throws WxPayException
            * @return 成功时返回，其他抛异常
            */
            public static WxPayData ShortUrl(Wxpay pay, WxPayData inputObj, int timeOut = 6)
            {
                string url = "https://api.mch.weixin.qq.com/tools/shorturl";
                //检测必填参数
                if (!inputObj.IsSet("long_url"))
                    throw new WxPayException("需要转换的URL，签名用原串，传输需URL encode！");

                inputObj.SetValue("appid", pay.Config.APPID);//公众账号ID
                inputObj.SetValue("mch_id", pay.Config.MCHID);//商户号
                inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串	
                inputObj.SetValue("sign", inputObj.MakeSign());//签名
                string xml = inputObj.ToXml();

                //var start = DateTime.Now;//请求开始时间

                //pay.LogEx.Debug("WxPayApi", string.Concat("ShortUrl request : ", xml));
                pay.LogEx.Log("ShortUrl request", xml);
                string response = HttpService.Post(pay, xml, url, false, timeOut);
                pay.LogEx.Log("ShortUrl response", response);
                //pay.LogEx.Debug("WxPayApi", string.Concat("ShortUrl response : ", response));

                //var end = DateTime.Now;
                //int timeCost = (int)((end - start).TotalMilliseconds);

                WxPayData result = WxPayData.FromXml(pay, response);
                //ReportCostTime(pay, url, timeCost, result);//测速上报

                return result;
            }


            /**
            * 
            * 统一下单
            * @param WxPaydata inputObj 提交给统一下单API的参数
            * @param int timeOut 超时时间
            * @throws WxPayException
            * @return 成功时返回，其他抛异常
            */
            public static WxPayData UnifiedOrder(Wxpay pay, WxPayData inputObj, int timeOut = 6)
            {
                string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

                //检测必填参数
                if (!inputObj.IsSet("out_trade_no"))
                    throw new WxPayException("缺少统一支付接口必填参数out_trade_no！");
                else if (!inputObj.IsSet("body"))
                    throw new WxPayException("缺少统一支付接口必填参数body！");
                else if (!inputObj.IsSet("total_fee"))
                    throw new WxPayException("缺少统一支付接口必填参数total_fee！");
                else if (!inputObj.IsSet("trade_type"))
                    throw new WxPayException("缺少统一支付接口必填参数trade_type！");

                //关联参数
                if ("JSAPI".Equals(inputObj.GetValue("trade_type")) && !inputObj.IsSet("openid"))
                    throw new WxPayException("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
                if ("NATIVE".Equals(inputObj.GetValue("trade_type")) && !inputObj.IsSet("product_id"))
                    throw new WxPayException("统一支付接口中，缺少必填参数product_id！trade_type为NATIVE时，product_id为必填参数！");

                //异步通知url未设置，则使用配置文件中的url
                if (!inputObj.IsSet("notify_url"))
                    inputObj.SetValue("notify_url", pay.Config.NOTIFY_URL);//异步通知url

                inputObj.SetValue("appid", pay.Config.APPID);//公众账号ID
                inputObj.SetValue("mch_id", pay.Config.MCHID);//商户号
                inputObj.SetValue("spbill_create_ip", pay.Config.IP);//终端ip	  	    
                inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串

                //签名
                inputObj.SetValue("sign", inputObj.MakeSign());
                string xml = inputObj.ToXml();

                //var start = DateTime.Now;

                //pay.LogEx.Debug("WxPayApi", string.Concat("UnfiedOrder request : ", xml));
                pay.LogEx.Log("UnfiedOrder request", xml);
                string response = HttpService.Post(pay, xml, url, false, timeOut);
                pay.LogEx.Log("UnfiedOrder response", response);
                //pay.LogEx.Debug("WxPayApi", string.Concat("UnfiedOrder response : ", response));

                //var end = DateTime.Now;
                //int timeCost = (int)((end - start).TotalMilliseconds);

                WxPayData result = WxPayData.FromXml(pay, response);

                //ReportCostTime(pay, url, timeCost, result);//测速上报

                return result;
            }


            /**
            * 
            * 关闭订单
            * @param WxPayData inputObj 提交给关闭订单API的参数
            * @param int timeOut 接口超时时间
            * @throws WxPayException
            * @return 成功时返回，其他抛异常
            */
            public static WxPayData CloseOrder(Wxpay pay, WxPayData inputObj, int timeOut = 6)
            {
                string url = "https://api.mch.weixin.qq.com/pay/closeorder";
                //检测必填参数
                if (!inputObj.IsSet("out_trade_no"))
                    throw new WxPayException("关闭订单接口中，out_trade_no必填！");

                inputObj.SetValue("appid", pay.Config.APPID);//公众账号ID
                inputObj.SetValue("mch_id", pay.Config.MCHID);//商户号
                inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串		
                inputObj.SetValue("sign", inputObj.MakeSign());//签名
                string xml = inputObj.ToXml();

                //var start = DateTime.Now;//请求开始时间
                pay.LogEx.Log("CloseOrder request", xml);
                string response = HttpService.Post(pay, xml, url, false, timeOut);
                pay.LogEx.Log("CloseOrder response", response);
                //var end = DateTime.Now;
                //int timeCost = (int)((end - start).TotalMilliseconds);

                WxPayData result = WxPayData.FromXml(pay, response);

                //ReportCostTime(pay, url, timeCost, result);//测速上报

                return result;
            }


            ///**
            //* 
            //* 测速上报
            //* @param string interface_url 接口URL
            //* @param int timeCost 接口耗时
            //* @param WxPayData inputObj参数数组
            //*/
            //private static void ReportCostTime(Wxpay pay, string interface_url, int timeCost, WxPayData inputObj)
            //{
            //    //如果不需要进行上报
            //    if (pay.Config.REPORT_LEVENL == 0)
            //    {
            //        return;
            //    }

            //    //如果仅失败上报
            //    if (pay.Config.REPORT_LEVENL == 1 && inputObj.IsSet("return_code") && inputObj.GetValue("return_code").ToString() == "SUCCESS" &&
            //     inputObj.IsSet("result_code") && inputObj.GetValue("result_code").ToString() == "SUCCESS")
            //    {
            //        return;
            //    }

            //    //上报逻辑
            //    WxPayData data = new WxPayData(pay);
            //    data.SetValue("interface_url", interface_url);
            //    data.SetValue("execute_time_", timeCost.ToString());
            //    //返回状态码
            //    if (inputObj.IsSet("return_code"))
            //    {
            //        data.SetValue("return_code", inputObj.GetValue("return_code"));
            //    }
            //    //返回信息
            //    if (inputObj.IsSet("return_msg"))
            //    {
            //        data.SetValue("return_msg", inputObj.GetValue("return_msg"));
            //    }
            //    //业务结果
            //    if (inputObj.IsSet("result_code"))
            //    {
            //        data.SetValue("result_code", inputObj.GetValue("result_code"));
            //    }
            //    //错误代码
            //    if (inputObj.IsSet("err_code"))
            //    {
            //        data.SetValue("err_code", inputObj.GetValue("err_code"));
            //    }
            //    //错误代码描述
            //    if (inputObj.IsSet("err_code_des"))
            //    {
            //        data.SetValue("err_code_des", inputObj.GetValue("err_code_des"));
            //    }
            //    //商户订单号
            //    if (inputObj.IsSet("out_trade_no"))
            //    {
            //        data.SetValue("out_trade_no", inputObj.GetValue("out_trade_no"));
            //    }
            //    //设备号
            //    if (inputObj.IsSet("device_info"))
            //    {
            //        data.SetValue("device_info", inputObj.GetValue("device_info"));
            //    }

            //    try
            //    {
            //        Report(pay, data);
            //    }
            //    catch (WxPayException)
            //    {
            //        //不做任何处理
            //    }
            //}


            ///**
            //* 
            //* 测速上报接口实现
            //* @param WxPayData inputObj 提交给测速上报接口的参数
            //* @param int timeOut 测速上报接口超时时间
            //* @throws WxPayException
            //* @return 成功时返回测速上报接口返回的结果，其他抛异常
            //*/
            //public static WxPayData Report(Wxpay pay, WxPayData inputObj, int timeOut = 1)
            //{
            //    string url = "https://api.mch.weixin.qq.com/payitil/report";
            //    //检测必填参数
            //    if (!inputObj.IsSet("interface_url"))
            //    {
            //        throw new WxPayException("接口URL，缺少必填参数interface_url！");
            //    }
            //    if (!inputObj.IsSet("return_code"))
            //    {
            //        throw new WxPayException("返回状态码，缺少必填参数return_code！");
            //    }
            //    if (!inputObj.IsSet("result_code"))
            //    {
            //        throw new WxPayException("业务结果，缺少必填参数result_code！");
            //    }
            //    if (!inputObj.IsSet("user_ip"))
            //    {
            //        throw new WxPayException("访问接口IP，缺少必填参数user_ip！");
            //    }
            //    if (!inputObj.IsSet("execute_time_"))
            //    {
            //        throw new WxPayException("接口耗时，缺少必填参数execute_time_！");
            //    }

            //    inputObj.SetValue("appid", pay.Config.APPID);//公众账号ID
            //    inputObj.SetValue("mch_id", pay.Config.MCHID);//商户号
            //    inputObj.SetValue("user_ip", pay.Config.IP);//终端ip
            //    inputObj.SetValue("time", DateTime.Now.ToString("yyyyMMddHHmmss"));//商户上报时间	 
            //    inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            //    inputObj.SetValue("sign", inputObj.MakeSign());//签名
            //    string xml = inputObj.ToXml();

            //    //pay.LogEx.Info("WxPayApi", string.Concat("Report request : ", xml));

            //    string response = HttpService.Post(pay, xml, url, false, timeOut);

            //    //pay.LogEx.Info("WxPayApi", string.Concat("Report response : ", response));

            //    return WxPayData.FromXml(pay, response);
            //}

            ///**
            //* 根据当前系统时间加随机序列来生成订单号
            // * @return 订单号
            //*/
            //public static string GenerateOutTradeNo(Wxpay pay)
            //{
            //    var ran = new Random();
            //    return string.Format("{0}{1}{2}", pay.Config.MCHID, DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
            //}

            /**
            * 生成时间戳，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数
             * @return 时间戳
            */
            public static string GenerateTimeStamp()
            {
                TimeSpan ts = DateTime.UtcNow - (new DateTime(1970, 1, 1, 0, 0, 0, 0));
                return Convert.ToInt64(ts.TotalSeconds).ToString();
            }

            /**
            * 生成随机串，随机串包含字母或数字
            * @return 随机串
            */
            public static string GenerateNonceStr()
            {
                return Guid.NewGuid().ToString("N");
            }
        }

        private WxPayConfig _config;
        private WxPayLog _log;
        private string _tradeNo;

        public Wxpay()
        {
            _config = new WxPayConfig(this);
            _log = new WxPayLog(this);
            _tradeNo = string.Empty;
        }

        private WxPayConfig Config
        {
            get { return _config; }
        }
        private WxPayLog LogEx
        {
            get { return _log; }
        }
        private string TradeNo
        {
            get { return _tradeNo; }
        }

        public override string Name
        {
            get { return "微信支付"; }
        }
        protected override string GatewayUrl
        {
            get { return null; }
        }
        public override bool IsNeedCallback
        {
            get { return false; }
        }

        public override bool AsyncCallback(Controller context, out PaymentResult result)
        {
            WxPayData data;
            bool value = false;
            PayResult presult = new PayResult();
            if (Notify.GetNotifyData(this, context, out data))
            {
                _tradeNo = data.GetValue("out_trade_no");

                WxPayData temp;
                ResultNotify resultNotify = new ResultNotify(this, context);
                ResultType type = resultNotify.ProcessNotify(data, out temp);
                if (type == ResultType.Success)
                {
                    presult.TradeNo = _tradeNo;
                    presult.PayTradeNo = data.GetValue("transaction_id");
                    presult.Status = data.GetValue("result_code");
                    presult.TotalFee = new Money(int.Parse(data.GetValue("total_fee")) / 100.0);
                    value = true;
                }
                //if (type == ResultType.NotMatch)
                //{
                //NativeNotify nativeNatify = new NativeNotify(this);
                //nativeNatify.ProcessNotify();
                //}
                data = temp;
            }
            context.Response.Write(data.ToXml());
            presult.Message = data.GetValue("return_msg");
            result = presult;
            return value;
        }
        public override void AsyncStop(Controller context)
        {
        }
        public override bool Callback(Controller context, out PaymentResult result)
        {
            RefundResult rresult = new RefundResult()
            {
                Message = context.Request.Form["return_msg"]
            };
            if ("SUCCESS".Equals(context.Request.Form["return_code"]))
            {
                if ("SUCCESS".Equals(context.Request.Form["result_code"]))
                {
                    WxPayData data = new WxPayData(this);
                    data.SetValue("refund_id", context.Request.Form["refund_id"]);//微信退款单号，优先级最高
                    //data.SetValue("out_refund_no", out_refund_no);//商户退款单号，优先级第二
                    //data.SetValue("transaction_id", transaction_id);//微信订单号，优先级第三
                    //data.SetValue("out_trade_no", out_trade_no);//商户订单号，优先级最低
                    WxPayData temp = WxPayApi.RefundQuery(this, data);//提交退款查询给API，接收返回数据
                    rresult.Message = temp["return_msg"];
                    if ("SUCCESS".Equals(temp["return_code"]))
                    {
                        if ("SUCCESS".Equals(temp["result_code"]))
                        {
                            rresult.SuccessNum = int.Parse(temp["refund_count"]);
                            if (rresult.SuccessNum > 0)
                            {
                                List<RefundInfo> list = new List<RefundInfo>(rresult.SuccessNum);
                                for (int i = 0; i < rresult.SuccessNum; ++i)
                                {
                                    list.Add(new RefundInfo()
                                    {
                                        PayTradeNo = temp[string.Concat("refund_id_", i)],
                                        Status = "SUCCESS".Equals(temp[string.Concat("refund_status_", i)]),
                                        TotalFee = new Money(int.Parse(temp[string.Concat("refund_fee_", i)]) / 100.0)
                                    });
                                }
                                rresult.BatchNo = temp["out_trade_no"];
                                rresult.NotifyId = temp["transaction_id"];
                                rresult.Results.AddRange(list);
                                result = rresult;
                                return true;
                            }
                            else
                            {
                                rresult.Message = "退款笔数为0";
                            }
                        }
                        else
                        {
                            rresult.Message = context.Request.Form["err_code_des"];
                        }
                    }
                }
                else
                {
                    rresult.Message = context.Request.Form["err_code_des"];
                }
            }
            result = rresult;
            return false;
        }
        public override string NewBatchNo(int no)
        {
            throw new NotSupportedException();
        }
        public override SortedDictionary<string, string> PackData(IPayOrder order)
        {
            _tradeNo = order.TradeNo;

            JsApiPay jsApiPay = new JsApiPay(this, HttpContext.Current);
            jsApiPay.trade_no = order.TradeNo;
            jsApiPay.subject = order.Subject;
            jsApiPay.total_fee = decimal.ToInt32(Money.ToDecimal(order.TotalFee * 100));
            jsApiPay.openid = order.OpenId;
            jsApiPay.GetUnifiedOrderResult();
            return jsApiPay.GetJsApiParameters().GetValues();//获取H5调起JS API参数   
        }
        public override SortedDictionary<string, string> PackData(string batchNo, params IRefundOrder[] orders)
        {
            if (orders == null)
                throw new ArgumentNullException("orders");
            if (orders.Length != 1)
                throw new ArgumentException();

            IRefundOrder order = orders[0];
            WxPayData data = new WxPayData(this);
            data.SetValue("transaction_id", order.PayTradeNo);//微信订单号存在的条件下，则已微信订单号为准
            //data.SetValue("out_trade_no", out_trade_no);//微信订单号不存在，才根据商户订单号去退款
            data.SetValue("total_fee", decimal.ToInt32(Money.ToDecimal(order.TotalFee * 100)).ToString());//订单总金额
            data.SetValue("refund_fee", decimal.ToInt32(Money.ToDecimal(order.TotalFee * 100)).ToString());//退款金额
            data.SetValue("out_refund_no", batchNo);//随机生成商户退款单号
            data.SetValue("op_user_id", Config.MCHID);//操作员，默认为商户号
            WxPayData result = WxPayApi.Refund(this, data);//提交退款申请给API，接收返回数据
            return result.GetValues();
        }
        public override string Submit(Controller context, SortedDictionary<string, string> para, string button, string url)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append(@"<script type=""text/javascript"">
function jsApiCall() {
    WeixinJSBridge.invoke('getBrandWCPayRequest', ").Append((new WxPayData(this, para)).ToJson()).Append(@", function (res) {
        //WeixinJSBridge.log(res.err_msg);
        //alert(res.err_code + res.err_desc + res.err_msg);
        //switch(res.err_msg) {
        //case 'get_brand_wcpay_request:ok':
        //    break;
        //case 'get_brand_wcpay_request:cancel':
        //    break;
        //case 'get_brand_wcpay_request:fail':
        //    break;
        //}
        window.location.href='").Append(url).Append(@"';
    });
}
function callpay() {
    if (typeof WeixinJSBridge == ""undefined"") {
        if (document.addEventListener) {
            document.addEventListener('WeixinJSBridgeReady', jsApiCall, false);
        }
        else if (document.attachEvent) {
            document.attachEvent('WeixinJSBridgeReady', jsApiCall);
            document.attachEvent('onWeixinJSBridgeReady', jsApiCall);
        }
    }
    else {
        jsApiCall();
    }
}
callpay();
</script>");
            return sbHtml.ToString();
        }
        public override string Refund(Controller context, SortedDictionary<string, string> para, string button)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("<form id='wxsubmit' name='alipaysubmit' action='").Append(CallbackUrl).Append("' method='post'>");
            foreach (KeyValuePair<string, string> temp in para)
                sbHtml.Append("<input type='hidden' name='").Append(temp.Key).Append("' value='").Append(temp.Value).Append("'/>");
            sbHtml.Append("<input type='submit' value='").Append(button).Append("' style='display:none;'></form>");
            sbHtml.Append("<script>document.forms['wxsubmit'].submit();</script>");
            return sbHtml.ToString();
        }

        public override string MakeQR(IPayOrder order)
        {
            NativePay pay = new NativePay(this)
            {
                subject = order.Subject,
                trade_no = order.TradeNo,
                total_fee = decimal.ToInt32(Money.ToDecimal(order.TotalFee * 100))
            };
            return pay.GetPayUrl();
        }
    }
}
