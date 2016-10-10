using Cnaws.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace Cnaws.Sms.Providers
{
    internal sealed class Yuntongxun : SmsProvider
    {
        private const string RestUrl = "app.cloopen.com";
        private const string RestPort = "8883";
        private const string SoftVer = "2013-12-26";

        private StringBuilder _message;

        //private sealed class XmlResult
        //{
        //    public static readonly XmlResult Default = new XmlResult(172002, "无返回");

        //    private int statusCode;
        //    private string statusMsg;
        //    private object data;

        //    public XmlResult(int code, string msg, object obj = null)
        //    {
        //        statusCode = code;
        //        statusMsg = msg;
        //        data = obj;
        //    }

        //    public int StatusCode
        //    {
        //        get { return statusCode; }
        //    }
        //    public string StatusMessage
        //    {
        //        get { return statusMsg; }
        //    }
        //    public object Data
        //    {
        //        get { return data; }
        //    }
        //    public object this[string name]
        //    {
        //        get { return TFields<XmlResult>.Fields[name].GetValue(this); }
        //    }
        //}
        //private sealed class DataParser
        //{
        //    private string _name;
        //    private object _data;

        //    public DataParser(string name)
        //    {
        //        _name = name;
        //        _data = null;
        //    }

        //    public XmlResult BuildResult(int code, string msg)
        //    {
        //        return new XmlResult(code, msg, _data);
        //    }
        //    public void Parse(int len, XmlNode item)
        //    {
        //        if (_name != null && len == _name.Length && item.Name == _name)
        //        {
        //            Dictionary<string, object> tmp = new Dictionary<string, object>();
        //            foreach (XmlNode subItem in item.ChildNodes)
        //                tmp.Add(subItem.Name, subItem.InnerText);
        //            _data = new Dictionary<string, object> { { item.Name, tmp } };
        //        }
        //    }
        //}

        public Yuntongxun()
        {
            _message = new StringBuilder();
        }

        public override string Name
        {
            get { return "云通讯"; }
        }

        private void CheckAll()
        {
            if (string.IsNullOrEmpty(Account))
                throw new ArgumentNullException("Account");
            if (string.IsNullOrEmpty(Token))
                throw new ArgumentNullException("Token");
            if (string.IsNullOrEmpty(AppId))
                throw new ArgumentNullException("AppId");
        }
        private string GetDate()
        {
            return DateTime.Now.ToString("yyyyMMddhhmmss");
        }
        private static string MD5Encrypt(string source)
        {
            byte[] data = Encoding.Default.GetBytes(source);
            using (MD5 md5Hasher = MD5.Create())
                data = md5Hasher.ComputeHash(data);
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; ++i)
                sBuilder.Append(data[i].ToString("X2"));
            return sBuilder.ToString();
        }
        private void AppendMessage(string message)
        {
            _message.AppendLine(message);
        }
        private Uri BuildUrl(string action, string date)
        {
            string sigstr = MD5Encrypt(string.Concat(Account, Token, date));
            string uriStr = string.Concat("https://", RestUrl, ":", RestPort, "/", SoftVer, "/", "Accounts", "/", Account, "/", action, "?sig=", sigstr);
            AppendMessage(string.Concat("url = ", uriStr));
            return new Uri(uriStr);
        }
        private byte[] BuildSMSMessageData(string to, string body)
        {
            StringBuilder data = new StringBuilder();
            data.Append("<?xml version='1.0' encoding='utf-8'?><SMSMessage>");
            data.Append("<to>").Append(to).Append("</to>");
            data.Append("<body>").Append(body).Append("</body>");
            data.Append("<appId>").Append(AppId).Append("</appId>");
            data.Append("</SMSMessage>");
            string temp = data.ToString();
            AppendMessage(string.Concat("requestBody = ", temp));
            return Encoding.UTF8.GetBytes(temp);
        }
        private byte[] BuildTemplateSMSData(string to, string templateId, string[] data)
        {
            StringBuilder bodyData = new StringBuilder();
            bodyData.Append("<?xml version='1.0' encoding='utf-8'?><TemplateSMS>");
            bodyData.Append("<to>").Append(to).Append("</to>");
            bodyData.Append("<appId>").Append(AppId).Append("</appId>");
            bodyData.Append("<templateId>").Append(templateId).Append("</templateId>");
            if (data != null && data.Length > 0)
            {
                bodyData.Append("<datas>");
                foreach (string item in data)
                    bodyData.Append("<data>").Append(item).Append("</data>");
                bodyData.Append("</datas>");
            }
            bodyData.Append("</TemplateSMS>");
            string temp = bodyData.ToString();
            AppendMessage(string.Concat("requestBody = ", temp));
            return Encoding.UTF8.GetBytes(temp);
        }
        private bool CertificateValidationResult(object obj, X509Certificate cer, X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }
        private void SetCertificateValidationCallBack()
        {
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationResult;
        }
        private HttpWebRequest CreateRequest(Uri address, string date, bool isPost = false, byte[] data = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.CreateHttp(address);
            SetCertificateValidationCallBack();
            request.Method = isPost ? "POST" : "GET";
            byte[] myByte = Encoding.UTF8.GetBytes(string.Concat(Account, ":", date));
            string authStr = Convert.ToBase64String(myByte);
            request.Headers.Add("Authorization", authStr);
            request.Accept = "application/xml";
            request.ContentType = "application/xml;charset=utf-8";
            if (isPost)
            {
                using (Stream postStream = request.GetRequestStream())
                    postStream.Write(data, 0, data.Length);
            }
            return request;
        }
        private void/*XmlResult*/ ParseResult(HttpWebRequest request/*, DataParser parser*/)
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseStr = reader.ReadToEnd();
                    AppendMessage(string.Concat("responseBody = ", responseStr));
                    //if (responseStr != null && responseStr.Length > 0)
                    //{
                    //    int len;
                    //    int code = 0;
                    //    string msg = "成功";
                    //    XmlDocument resultXml = new XmlDocument();
                    //    resultXml.LoadXml(responseStr);
                    //    using (XmlNodeList nodeList = resultXml.SelectSingleNode("Response").ChildNodes)
                    //    {
                    //        foreach (XmlNode item in nodeList)
                    //        {
                    //            len = item.Name.Length;
                    //            if (len == 9 && item.Name == "statusMsg")
                    //                msg = item.InnerText;
                    //            else if (len == 10 && item.Name == "statusCode")
                    //                code = int.Parse(item.InnerText.Trim());
                    //            else
                    //                parser.Parse(len, item);
                    //        }
                    //    }
                    //    return parser.BuildResult(code, msg);
                    //}
                }
            }
            //return XmlResult.Default;
        }

        public override void Send(long[] to, string body, params string[] arguments)
        {
            CheckAll();
            if (to == null || to.Length == 0)
                throw new ArgumentNullException("to");
            if (string.IsNullOrEmpty(body))
                throw new ArgumentNullException("body");
            try
            {
                string date = GetDate();
                Uri address = BuildUrl("SMS/Messages", date);
                string mobiles = string.Join(",", to);
                string content = body;
                if (arguments != null && arguments.Length > 0)
                    content = string.Format(body, arguments);
                byte[] data = BuildSMSMessageData(mobiles, content);
                HttpWebRequest request = CreateRequest(address, date, true, data);
                /*XmlResult result = */ParseResult(request);//, new DataParser("SMSMessage"));
            }
            catch (Exception e)
            {
                AppendMessage(string.Concat("error = ", e.Message, Environment.NewLine, e.StackTrace));
                throw e;
            }
            finally
            {
                WriteLog(string.Concat("SendSMS", Environment.NewLine, _message.ToString()));
            }
        }
        public override void Send(long to, string body, params string[] arguments)
        {
            Send(new long[] { to }, body, arguments);
        }
        public override void SendTemplate(long[] to, string tempId, params string[] arguments)
        {
            CheckAll();
            if (to == null)
                throw new ArgumentNullException("to");
            if (tempId == null)
                throw new ArgumentNullException("tempId");
            try
            {
                string date = GetDate();
                Uri address = BuildUrl("SMS/TemplateSMS", date);
                string mobiles = string.Join(",", to);
                byte[] sdata = BuildTemplateSMSData(mobiles, tempId, arguments);
                HttpWebRequest request = CreateRequest(address, date, true, sdata);
                /*XmlResult result = */ParseResult(request);//, new DataParser("TemplateSMS"));
            }
            catch (Exception e)
            {
                AppendMessage(string.Concat("error = ", e.Message, Environment.NewLine, e.StackTrace));
                throw e;
            }
            finally
            {
                WriteLog(string.Concat("SendTemplateSMS", Environment.NewLine, _message.ToString()));
            }
        }
        public override void SendTemplate(long to, string tempId, params string[] arguments)
        {
            SendTemplate(new long[] { to }, tempId, arguments);
        }
    }
}
