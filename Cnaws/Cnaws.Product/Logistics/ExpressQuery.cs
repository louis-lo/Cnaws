using Cnaws.Data;
using Cnaws.Product.Modules;
using Cnaws.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.Security;
using Cnaws.Json;

namespace Cnaws.Product.Logistics
{
    #region 快递查询
    /// <summary>
    /// 快递查询
    /// </summary>
    public class ExpressQuery
    {
        #region 查询
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="com">快递公司代码</param>
        /// <param name="nu">快递单号</param>
        /// <returns></returns>
        public static string QueryReturnJson(string com, string nu)
        {
            if (string.IsNullOrEmpty(com) || string.IsNullOrEmpty(nu))
            {
                return null;
            }
            else
            {
                string url = string.Format("http://poll.kuaidi100.com/poll/query.do");
                WebRequest request = WebRequest.Create(url);
                string key = "UOZmyvtH4077";
                string customer = "73BA3C8E3677E25114A2D88915395980";
                string param = "{";
                param += "\"com\":\"" + com + "\",";
                param += "\"num\":\"" + nu + "\",";
                param += "\"from\":\"\",";
                param += "\"to\":\"\"";
                param += "}";
                string sign = MD5(param + key + customer);
                string data = string.Format(@"customer={0}&sign={1}&param={2}", customer, sign, param);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(data);
                }
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="com">快递公司代码</param>
        /// <param name="nu">快递单号</param>
        /// <returns></returns>
        public static ExpressInfo Query(string com, string nu)
        {
            return new JavaScriptSerializer().Deserialize<ExpressInfo>(QueryReturnJson(com, nu));
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="orderId">订单号</param>
        /// <returns></returns>
        public static ExpressInfo Query(DataSource ds, string orderId, out ProductLogistics log)
        {
            log = ProductLogistics.GetByOrder(ds, orderId);
            if (log != null)
                return Query(log.ProviderKey, log.BillNo);
            else
                return new ExpressInfo();
        }
        public static ExpressInfo Query(string providerDetailed)
        {
            return new JavaScriptSerializer().Deserialize<ExpressInfo>(providerDetailed);
        }
        public static ExpressInfo Query2(string com, string nu)
        {
            if (string.IsNullOrEmpty(com) || string.IsNullOrEmpty(nu))
            {
                return null;
            }
            else
            {
                string url = string.Format("http://api.kuaidi100.com/api?id={0}&com={1}&nu={2}", "UOZmyvtH4077", com, nu);
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        //string result = reader.ReadToEnd();
                        string result = "{\"message\": \"ok\",\"state\": \"0\",\"status\": \"200\",\"condition\": \"F00\",\"ischeck\": \"0\",\"com\": \"yuantong\",\"nu\": \"V030344422\",\"data\": [{\"context\": \"上海分拨中心/装件入车扫描 \",\"time\": \"2012-08-28 16:33:19\",\"ftime\": \"2012-08-28 16:33:19\"},{\"context\": \"上海分拨中心/下车扫描 \",\"time\": \"2012-08-27 23:22:42\",\"ftime\": \"2012-08-27 23:22:42\"}]}";
                        return new JavaScriptSerializer().Deserialize<ExpressInfo>(result);
                    }
                }
            }
        }
        #endregion
        public static string MD5(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] InBytes = Encoding.GetEncoding("UTF-8").GetBytes(str);
            byte[] OutBytes = md5.ComputeHash(InBytes);
            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
            return OutString.ToUpper();
        }
    } 
    #endregion

    #region 快递信息
    /// <summary>
    /// 快递信息
    /// </summary>
    public class ExpressInfo
    {
        /// <summary>
        /// 物流公司编号
        /// </summary>
        public string com { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string nu { get; set; }

        /// <summary>
        /// 快递单当前的状态 ：　 
        /// 0：在途，即货物处于运输过程中
        /// 1：揽件，货物已由快递公司揽收并且产生了第一条跟踪信息
        /// 2：疑难，货物寄送过程出了问题
        /// 3：签收，收件人已签收
        /// 4：退签，即货物由于用户拒签、超区等原因退回，而且发件人已经签收
        /// 5：派件，即快递正在进行同城派件
        /// 6：退回，货物正处于退回发件人的途中
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// 查询结果状态： 
        /// 0：物流单暂无结果 
        /// 1：查询成功
        /// 2：接口出现异常
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 快递信息明细
        /// </summary>
        public IList<ExpressInfoDetailed> data { get; set; }

        public string message { get; set; }
    }
    #endregion

    #region 快递信息明细
    /// <summary>
    /// 快递信息明细
    /// </summary>
    public class ExpressInfoDetailed
    {
        /// <summary>
        /// 每条跟踪信息的时间
        /// </summary>
        public string time = "";//{ get; set; }

        /// <summary>
        /// 每条跟综信息的描述
        /// </summary>
        public string context = "";//{ get; set; }

        public string ftime = "";//{ get; set; }
    }
    #endregion
}
