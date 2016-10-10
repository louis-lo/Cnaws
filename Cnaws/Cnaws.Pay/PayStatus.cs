using System;

namespace Cnaws.Pay
{
    /// <summary>
    /// 支付状态
    /// </summary>
    public enum PayStatus
    {
        /// <summary>
        /// 支付失败
        /// </summary>
        PayFailed = 0,
        /// <summary>
        /// 等待支付
        /// </summary>
        Paying = 1,
        /// <summary>
        /// 等待验证
        /// </summary>
        PayNotifying = 2,
        /// <summary>
        /// 支付成功
        /// </summary>
        PaySuccess = 3,
        /// <summary>
        /// 退款失败
        /// </summary>
        RefundFailed = 4,
        /// <summary>
        /// 等待退款
        /// </summary>
        RefundNotifying = 5,
        /// <summary>
        /// 退款成功
        /// </summary>
        RefundSuccess = 6
    }
}
