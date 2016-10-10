using Cnaws.Templates;
using Cnaws.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Cnaws.Pay
{
    public enum PaymentType
    {
        Pay = 0,
        Refund = 1
    }
    public interface IOrder
    {
        string TradeNo { get; }
        Money TotalFee { get; }
    }
    public interface IPayOrder : IOrder
    {
        string Subject { get; }
        PayStatus Status { get; }
        string OpenId { get; }
    }
    public interface IRefundOrder : IOrder
    {
        string PayProvider { get; }
        string PayTradeNo { get; }
        string Subject { get; }
    }
    public interface IPayLog
    {
        void Write(string provider, string id, string log);
    }
    public abstract class PaymentResult
    {
        public abstract PaymentType Type { get; }
        public string NotifyId { get; set; }
        public string Message { get; set; }
    }
    public sealed class PayResult : PaymentResult
    {
        public override PaymentType Type
        {
            get { return PaymentType.Pay; }
        }
        public string TradeNo { get; set; }
        public string PayTradeNo { get; set; }
        public string Status { get; set; }
        public Money TotalFee { get; set; }
    }
    public sealed class RefundInfo
    {
        public string PayTradeNo { get; set; }
        public Money TotalFee { get; set; }
        public bool Status { get; set; }
        //public string Account { get; set; }
        //public Money Fees { get; set; }
        //public bool FeesStatus { get; set; }
    }
    public sealed class RefundResult : PaymentResult
    {
        private List<RefundInfo> _results;
        
        public RefundResult(params RefundInfo[] items)
        {
            _results = new List<RefundInfo>();
            if (items != null)
                _results.AddRange(items);
        }

        public override PaymentType Type
        {
            get { return PaymentType.Refund; }
        }
        public string BatchNo { get; set; }
        public int SuccessNum { get; set; }
        public List<RefundInfo> Results
        {
            get { return _results; }
        }
    }

    public abstract class PayProvider
    {
        protected PayProvider()
        {
        }

        public string Key
        {
            get { return GetType().Name; }
        }
        /// <summary>
        /// 提交模式
        /// </summary>
        protected virtual string Method
        {
            get { return "post"; }
        }
        /// <summary>
        /// 字符集
        /// </summary>
        protected virtual string Charset
        {
            get { return "utf-8"; }
        }
        /// <summary>
        /// 支付插件名称
        /// </summary>
        public abstract string Name
        {
            get;
        }
        /// <summary>
        /// 版本
        /// </summary>
        public virtual Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }
        /// <summary>
        /// 提交地址
        /// </summary>
        protected abstract string GatewayUrl
        {
            get;
        }
        /// <summary>
        /// 支付完成后，回调地址
        /// </summary>
        public string CallbackUrl
        {
            get; set;
        }
        /// <summary>
        /// 异步通知地址
        /// </summary>
        public string AsyncCallbackUrl
        {
            get; set;
        }
        /// <summary>
        /// 合作身份者id
        /// </summary>
        public string Partner
        {
            get; set;
        }
        /// <summary>
        /// 合作身份者id
        /// </summary>
        public string PartnerId
        {
            get; set;
        }
        /// <summary>
        /// 安全检验码key
        /// </summary>
        public virtual string PartnerKey
        {
            get; set;
        }
        public virtual string PartnerSecret
        {
            get; set;
        }
        /// <summary>
        /// 是否需要单独页面表单提交
        /// </summary>
        public virtual bool IsNeedSubmit
        {
            get { return true; }
        }
        /// <summary>
        /// 是否需要在线支付
        /// </summary>
        public virtual bool IsOnlinePay
        {
            get { return true; }
        }
        public virtual bool IsCheckMoney
        {
            get { return true; }
        }
        public virtual bool IsNeedNotify
        {
            get { return true; }
        }
        public virtual bool IsNeedCallback
        {
            get { return true; }
        }
        public IPayLog Log
        {
            get; set;
        }

        public static PayProvider Create(string name)
        {
            try
            {
                Type type = Type.GetType(string.Concat("Cnaws.Pay.Providers.", name, ",Cnaws.Pay"), true, true);
                object result = Activator.CreateInstance(type);
                if (TType<PayProvider>.Type.IsAssignableFrom(result.GetType()))
                    return (PayProvider)result;
            }
            catch (Exception) { }
            return null;
        }

        /// <summary>
        /// 终止异步处理
        /// </summary>
        public virtual void AsyncStop(Controller context)
        {
            context.Response.Write("success");
        }
        /// <summary>
        /// 打包支付信息
        /// </summary>
        /// <returns></returns>
        public abstract SortedDictionary<string, string> PackData(IPayOrder order);
        public abstract string NewBatchNo(int no);
        public abstract SortedDictionary<string, string> PackData(string batchNo, params IRefundOrder[] orders);
        public virtual string Submit(Controller context, SortedDictionary<string, string> para, string button, string url)
        {
            return string.Empty;
        }
        public virtual string Refund(Controller context, SortedDictionary<string, string> para, string button)
        {
            return null;
        }
        /// <summary>
        /// 同步支付回调
        /// </summary>
        public abstract bool Callback(Controller context, out PaymentResult result);
        /// <summary>
        /// 异步支付回调
        /// </summary>
        public abstract bool AsyncCallback(Controller context, out PaymentResult result);
        /// <summary>
        /// 后期与服务同步处理类
        /// </summary>
        public virtual void AfterAsync(Controller context)
        {
        }

        public virtual string MakeQR(IPayOrder order)
        {
            return null;
        }

        protected static bool HttpRequest(string url, out string result, Encoding charset = null, int timeout = 120000)
        {
            return HttpRequest(url, out result, null, charset, timeout);
        }
        protected static bool HttpRequest(string url, out string result, byte[] data, Encoding charset = null, int timeout = 120000)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.Timeout = timeout;
                if (data != null)
                {
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;
                    using (Stream s = request.GetRequestStream())
                        s.Write(data, 0, data.Length);
                }
                else
                {
                    request.Method = "GET";
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream s = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(s, charset ?? Encoding.UTF8))
                            result = reader.ReadToEnd();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                result = string.Concat(ex.Message, Environment.NewLine, ex.StackTrace);
            }
            return false;
        }

        public void WriteLog(string id, string log)
        {
            if (Log != null)
                Log.Write(Key, id, log);
        }
    }
}
