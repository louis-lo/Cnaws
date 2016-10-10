using System;
using Cnaws.Web;
using Cnaws.Data;
using M = Cnaws.Pay.Modules;
using System.Collections.Generic;

namespace Cnaws.Pay.Controllers
{
    public abstract class PaymentBase : DataController
    {
        protected virtual string SubmitText
        {
            get { return "确认"; }
        }
        protected virtual string ReturnUrl
        {
            get { return GetUrl("/bought"); }
        }

        [Authorize(true)]
        public void Index()
        {
            OnIndex();
        }
        protected virtual void OnIndex()
        {
            Render("payment.html");
        }

        protected virtual bool CheckProvider(PayProvider provider)
        {
            return true;
        }
        protected PayProvider LoadProvider(string provider)
        {
            PayProvider result = PayProvider.Create(provider);
            if (result != null && CheckProvider(result))
            {
                M.Payment pay = M.Payment.GetById(DataSource, result.Key);
                if (pay != null && pay.Enabled)
                {
                    Uri uri = Request.Url;
                    string url = string.Concat(uri.Scheme, "://", uri.DnsSafeHost, uri.Port != 80 ? string.Concat(":", uri.Port.ToString()) : string.Empty);
                    result.Partner = pay.Partner;
                    result.PartnerId = pay.PartnerId;
                    result.PartnerKey = pay.PartnerKey;
                    result.PartnerSecret = pay.PartnerSecret;
                    result.CallbackUrl = string.Concat(url, GetUrl("/", GetType().Name.ToLower(), "/callback/", provider));
                    result.AsyncCallbackUrl = string.Concat(url, GetUrl("/", GetType().Name.ToLower(), "/notify/", provider));
                    result.Log = GetPayLog();
                    return result;
                }
            }
            return null;
        }
        protected virtual IPayLog GetPayLog()
        {
            return new PaySqlLog(DataSource);
        }

        [Authorize(true)]
        public void Submit(string provider)
        {
            PayProvider pay = LoadProvider(provider);
            if (pay != null)
            {
                if (pay.IsNeedSubmit)
                {
                    try
                    {
                        IPayOrder order = GetPayOrder(pay.Key);
                        if (order == null)
                            throw new Exception("获取订单信息错误！");
                        if (pay.IsOnlinePay)
                        {
                            OnSubmit(pay.Submit(this, pay.PackData(order), SubmitText, ReturnUrl));
                        }
                        else
                        {
                            PaymentResult result;
                            bool value;
                            if (pay.IsCheckMoney)
                            {
                                value = CheckMoney(order, out result);
                            }
                            else
                            {
                                result = new PayResult()
                                {
                                    TradeNo = order.TradeNo,
                                    PayTradeNo = order.TradeNo,
                                    Status = "Success",
                                    TotalFee = order.TotalFee
                                };
                                value = true;
                            }
                            OnCallback(pay, value, result);
                            OnRedirect(this, pay, result, value);
                            //try { Response.Redirect(ReturnUrl, true); }
                            //catch (Exception) { }
                        }
                    }
                    catch (Exception ex)
                    {
                        OnError(ex.Message);
                    }
                }
                else
                {
                    OnError(string.Concat("第三方支付\"", provider, "\"不支持提交！"));
                }
            }
            else
            {
                OnError(string.Concat("第三方支付\"", provider, "\"不被支持或已禁用！"));
            }
        }
        [Authorize(true)]
        public void Refund()
        {
            IRefundOrder order = GetRefundOrder();
            if (order != null)
            {
                PayProvider pay = LoadProvider(order.PayProvider);
                if (pay != null)
                {
                    if (pay.IsOnlinePay)
                    {
                        OnSubmit(pay.Refund(this, pay.PackData(order.TradeNo, order), SubmitText));
                    }
                    else
                    {
                        RefundResult result = new RefundResult(new RefundInfo()
                        {
                            PayTradeNo = order.PayTradeNo,
                            TotalFee = order.TotalFee,
                            Status = true
                        });
                        OnCallback(pay, true, result);
                        OnRedirect(this, pay, result, true);
                        //try { Response.Redirect(ReturnUrl, true); }
                        //catch (Exception) { }
                    }
                }
                else
                {
                    OnError(string.Concat("第三方支付\"", order.PayProvider, "\"不被支持或已禁用！"));
                }
            }
            else
            {
                OnError(string.Concat("获取订单信息错误！"));
            }
        }

        public void MakeQR(string provider)
        {
             PayProvider pay = LoadProvider(provider);
            if (pay != null)
            {
                try
                {
                    IPayOrder order = GetPayOrder(pay.Key);
                    if (order == null)
                        throw new Exception("获取订单信息错误！");
                    string url = pay.MakeQR(order);
                    if (string.IsNullOrEmpty(url))
                        throw new Exception(string.Concat("第三方支付\"", provider, "\"暂不支持扫码支付或未实现！"));
                    OnMakeQR(url, order.TradeNo);
                }
                catch (Exception ex)
                {
                    OnError(ex.Message);
                }
            }
            else
            {
                OnError(string.Concat("第三方支付\"", provider, "\"不被支持或已禁用！"));
            }
        }
        /// <summary>
        /// 二维码返回处理
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="orderid">订单号</param>
        protected virtual void OnMakeQR(string url, string orderid = "")
        {
            SetResult(true, url);
        }

        protected virtual IPayOrder GetPayOrder(string provider)
        {
            return M.PayRecord.Create(DataSource, long.Parse(Request.Form["UserId"]), Request.Form["OpenId"], Request.Form["Title"], provider, Money.Parse(Request.Form["Money"]), int.Parse(Request.Form["Type"]), Request.Form["TargetId"]);
        }
        protected virtual IRefundOrder GetRefundOrder()
        {
            try
            {
                return M.PayRecord.GetByRefund(DataSource, Request.Form["OrderId"]);
            }
            catch (Exception) { }
            return null;
        }
        protected virtual bool CheckMoney(IPayOrder order, out PaymentResult result)
        {
            result = null;
            return false;
        }
        protected virtual void OnSubmit(string html)
        {
            Response.Write(html);
        }
        protected virtual void OnError(string message)
        {
            Response.Write(string.Concat("<script type=\"text/javascript\">alert('", message, "');window.history.go(-1);</script>"));
            End();
        }
        public void Callback(string provider)
        {
            PayProvider pay = LoadProvider(provider);
            if (pay != null)
            {
                PaymentResult result;
                if (pay.Callback(this, out result))
                {
                    OnCallback(pay, true, result);
                    OnRedirect(this, pay, result, true);
                }
                else
                {
                    OnCallback(pay, false, result);
                    OnRedirect(this, pay, result, false);
                }

            }
            else
            {
                OnError(string.Concat("第三方支付\"", provider, "\"不被支持或已禁用！"));
            }
        }
        public virtual void OnRedirect(Controller context, PayProvider pay, PaymentResult result, bool payType)
        {
            try { context.Response.Redirect(ReturnUrl, true); }
            catch (Exception) { }
        }

        protected virtual void OnCallback(PayProvider provider, bool success, PaymentResult result)
        {
            #region Pay
            if (result.Type == PaymentType.Pay)
            {
                PayResult presult = (PayResult)result;
                if (!string.IsNullOrEmpty(presult.TradeNo))
                {
                    DataSource.Begin();
                    try
                    {
                        M.PayRecord pr = M.PayRecord.GetById(DataSource, presult.TradeNo, result.Type);
                        if (pr == null)
                            throw new ArgumentException("订单号错误！");
                        if (pr.Status >= PayStatus.Paying && pr.Status < PayStatus.PaySuccess)
                        {
                            PayStatus status = pr.Status;
                            pr.PayId = presult.PayTradeNo;
                            if (provider.IsOnlinePay)
                                pr.Money = presult.TotalFee;
                            if (success)
                            {
                                if (provider.IsNeedNotify)
                                    pr.Status = PayStatus.PayNotifying;
                                else
                                    pr.Status = PayStatus.PaySuccess;
                            }
                            else
                            {
                                if (provider.IsOnlinePay && !provider.IsNeedNotify)
                                {
                                    pr.Status = PayStatus.PayFailed;
                                }
                                else
                                {
                                    if (!provider.IsOnlinePay)
                                        throw new Exception("余额不足！");
                                }
                            }
                            if (pr.UpdateStatus(DataSource, status) == DataStatus.Success)
                            {
                                if (success && pr.Status == PayStatus.PaySuccess)
                                {
                                    if (!OnModifyMoney(provider, pr.PayType, pr.UserId, presult.TotalFee, pr.Id, pr.Title, pr.Type, pr.TargetId))
                                        throw new ArgumentException("充值失败！");
                                }
                            }
                            else
                            {
                                throw new ArgumentException("更新订单失败！");
                            }
                        }
                        DataSource.Commit();
                    }
                    catch (Exception ex)
                    {
                        DataSource.Rollback();
                        OnError(ex.Message);
                    }
                }
                else
                {
                    OnError("订单号为空！");
                }
            }
            #endregion

            #region Refund
            else if (result.Type == PaymentType.Refund)
            {
                //RefundResult presult = (RefundResult)result;
                //if (!string.IsNullOrEmpty(presult.BatchNo))
                //{
                //    DataSource.Begin();
                //    try
                //    {
                //        M.PayRecord pr = M.PayRecord.GetById(DataSource, presult.BatchNo, result.Type);
                //        if (pr == null)
                //            throw new ArgumentException("订单号错误！");
                //        if (pr.Status == PayStatus.RefundNotifying)
                //        {
                //            PayStatus status = pr.Status;
                //            if (provider.IsOnlinePay)
                //                pr.Money = presult.TotalFee;
                //            if (success)
                //            {
                //                if (provider.IsNeedNotify)
                //                    pr.Status = PayStatus.PayNotifying;
                //                else
                //                    pr.Status = PayStatus.PaySuccess;
                //            }
                //            else
                //            {
                //                if (provider.IsOnlinePay && !provider.IsNeedNotify)
                //                {
                //                    pr.Status = PayStatus.PayFailed;
                //                }
                //                else
                //                {
                //                    if (!provider.IsOnlinePay)
                //                        throw new Exception("余额不足！");
                //                }
                //            }
                //            if (pr.UpdateStatus(DataSource, status) == DataStatus.Success)
                //            {
                //                if (success && pr.Status == PayStatus.PaySuccess)
                //                {
                //                    if (!OnModifyMoney(provider, pr.PayType, pr.UserId, presult.TotalFee, pr.Id, pr.Title, pr.Type, pr.TargetId))
                //                        throw new ArgumentException("充值失败！");
                //                }
                //            }
                //            else
                //            {
                //                throw new ArgumentException("更新订单失败！");
                //            }
                //        }
                //        DataSource.Commit();
                //    }
                //    catch (Exception ex)
                //    {
                //        DataSource.Rollback();
                //        OnError(ex.Message);
                //    }
                //}
                //else
                //{
                //    OnError("订单号为空！");
                //}
            }
            #endregion

            else
            {
                throw new ArgumentException("支付类型错误！");
            }
        }
        protected abstract bool OnModifyMoney(PayProvider provider, PaymentType payment, long user, Money money, string trade, string title, int type, string targetId);
        public void Notify(string provider)
        {
            PayProvider pay = LoadProvider(provider);
            if (pay != null)
            {
                PaymentResult result;
                if (pay.AsyncCallback(this, out result))
                {
                    if (OnNotify(pay, true, result))
                        pay.AsyncStop(this);
                }
                else
                {
                    OnNotify(pay, false, result);
                }
            }
            //else
            //{
            //    OnNotifyError(string.Concat("第三方支付\"", provider, "\"不被支持或已禁用！"));
            //}
        }
        protected virtual bool OnNotify(PayProvider provider, bool success, PaymentResult result)
        {
            if (result.Type == PaymentType.Pay)
            {
                PayResult presult = (PayResult)result;
                if (!string.IsNullOrEmpty(presult.TradeNo))
                {
                    DataSource.Begin();
                    try
                    {
                        M.PayRecord pr = M.PayRecord.GetById(DataSource, presult.TradeNo, presult.Type);
                        if (pr == null)
                            throw new ArgumentException("订单号错误！");
                        if (pr.Status >= PayStatus.Paying && pr.Status < PayStatus.PaySuccess)
                        {
                            PayStatus status = pr.Status;
                            pr.PayId = presult.PayTradeNo;
                            if (provider.IsOnlinePay)
                                pr.Money = presult.TotalFee;
                            if (success)
                                pr.Status = PayStatus.PaySuccess;
                            else
                                pr.Status = PayStatus.PayFailed;
                            if (pr.UpdateStatus(DataSource, status) == DataStatus.Success)
                            {
                                if (success && pr.Status == PayStatus.PaySuccess)
                                {
                                    if (!OnModifyMoney(provider, pr.PayType, pr.UserId, presult.TotalFee, pr.Id, pr.Title, pr.Type, pr.TargetId))
                                        throw new ArgumentException("充值失败！");
                                }
                            }
                            else
                            {
                                throw new ArgumentException("更细订单失败！");
                            }
                        }
                        else
                        {
                            if (pr.Status != PayStatus.PaySuccess)
                                throw new ArgumentException("订单号错误！");
                        }
                        DataSource.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        DataSource.Rollback();
                        //OnNotifyError(provider, ex.Message);
                    }
                }
                //else
                //{
                //    OnNotifyError(provider, "订单号为空！");
                //}
            }
            else
            {
                RefundResult rresult = (RefundResult)result;
                if (!string.IsNullOrEmpty(rresult.BatchNo))
                {
                    if (rresult.SuccessNum == 1 && rresult.Results.Count == 1)
                    {
                        RefundInfo info = rresult.Results[0];
                        DataSource.Begin();
                        try
                        {
                            M.PayRecord pr = M.PayRecord.GetById(DataSource, rresult.BatchNo, rresult.Type);
                            if (pr == null)
                                throw new ArgumentException("批次号错误！");
                            if (pr.PayId != info.PayTradeNo)
                                throw new ArgumentException("订单号错误！");
                            if (pr.Status == PayStatus.RefundNotifying)
                            {
                                if (provider.IsOnlinePay)
                                    pr.Money = info.TotalFee;
                                if (success && info.Status)
                                    pr.Status = PayStatus.RefundSuccess;
                                else
                                    pr.Status = PayStatus.RefundFailed;
                                if (pr.UpdateStatus(DataSource, PayStatus.RefundNotifying) == DataStatus.Success)
                                {
                                    if (success && info.Status && pr.Status == PayStatus.RefundSuccess)
                                    {
                                        if (!OnModifyMoney(provider, pr.PayType, pr.UserId, info.TotalFee, pr.Id, pr.Title, pr.Type, pr.TargetId))
                                            throw new ArgumentException("退款失败！");
                                    }
                                }
                                else
                                {
                                    throw new ArgumentException("更新订单失败！");
                                }
                            }
                            else
                            {
                                if (pr.Status != PayStatus.RefundSuccess)
                                    throw new ArgumentException("批次号错误！");
                            }
                            DataSource.Commit();
                            return true;
                        }
                        catch (Exception)
                        {
                            DataSource.Rollback();
                            //OnNotifyError(provider, ex.Message);
                        }
                    }
                    //else
                    //{
                    //    OnNotifyError(provider, "退款数量错误！");
                    //}
                }
                //else
                //{
                //    OnNotifyError(provider, "批次号为空！");
                //}
            }
            return false;
        }
    }
}
