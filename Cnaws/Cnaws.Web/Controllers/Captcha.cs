using System;
using System.IO;
using System.Web;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Web.Configuration;
using Cnaws.ExtensionMethods;
using Cnaws.Web.Configuration;

namespace Cnaws.Web.Controllers
{
    public sealed class Captcha : Controller
    {
        private static readonly CaptchaSection CAPTCHA_SECTION;
        private static Random CAPTCHA_RANDOM;
        private static PrivateFontCollection CAPTCHA_FONTS;

        static Captcha()
        {
            CAPTCHA_SECTION = CaptchaSection.GetSection();
            CAPTCHA_RANDOM = new Random();
            CAPTCHA_FONTS = new PrivateFontCollection();
            int len = Properties.Resources.Font.Length;
            IntPtr p = Marshal.AllocHGlobal(len);
            Marshal.Copy(Properties.Resources.Font, 0, p, len);
            CAPTCHA_FONTS.AddMemoryFont(p, len);
            Marshal.FreeHGlobal(p);
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public static bool CheckCaptcha(string name, string code)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[string.Concat(CAPTCHA_SECTION.CookiePrefix, name)];
            if (cookie != null)
            {
                byte[] bytes = PassportAuthentication.DecodeCookie(cookie.Value);
                DateTime time = new DateTime(BitConverter.ToInt64(bytes, 0));
                if (time.AddMinutes(CAPTCHA_SECTION.Expiration) > DateTime.Now)
                    return string.Equals(code, Encoding.UTF8.GetString(bytes, 8, bytes.Length - 8), StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
        public void CheckCaptcha()
        {
            if (IsAjax && IsPost)
            {
                string captcha = Request.Form["captcha"];
                if (!string.IsNullOrEmpty(captcha))
                    SetResult(CheckCaptcha(Request.Form["name"], captcha));
                else
                    SetResult(false);
            }
            else
            {
                NotFound();
            }
        }

        public void Index()
        {
            Custom(null);
        }
        public void Custom(string name, int width = Utility.CaptchaDefaultWidth, int height = Utility.CaptchaDefaultHeight, int count = Utility.CaptchaDefaultCount)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            string code = RandCode();
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(DateTime.Now.Ticks));
            bytes.AddRange(Encoding.UTF8.GetBytes(code));
            string cookieName = string.Concat(CAPTCHA_SECTION.CookiePrefix, (name ?? string.Empty));
            Response.Cookies[cookieName].Value = PassportAuthentication.EncodeCookie(bytes.ToArray());
            if (!string.IsNullOrEmpty(CAPTCHA_SECTION.CookieDomain))
                Response.Cookies[cookieName].Domain = CAPTCHA_SECTION.CookieDomain;
            CreateImage(Context, code, width, height);
        }

        private static string RandCode()
        {
            char[] code = new char[CAPTCHA_SECTION.DefaultCount];
            for (int i = 0; i < CAPTCHA_SECTION.DefaultCount; ++i)
                code[i] = CAPTCHA_SECTION.Chars[CAPTCHA_RANDOM.Next(CAPTCHA_SECTION.Chars.Length)];
            return new string(code);
        }

        private void CreateImage(HttpContext context, string code, int width, int height)
        {
            int _basefont = (height / 10) * 5;
            int _next = height / 4 - 3;

            Font[] fonts = {
                //new Font(new FontFamily("Arial"),(_basefont + CAPTCHA_RANDOM.Next(_next)),System.Drawing.FontStyle.Strikeout),
                new Font(CAPTCHA_FONTS.Families[0], (_basefont + CAPTCHA_RANDOM.Next(_next)), FontStyle.Regular),
                new Font(CAPTCHA_FONTS.Families[0], (_basefont + CAPTCHA_RANDOM.Next(_next)), FontStyle.Italic)
            };

            try
            {
                int int_ImageWidth = code.Length * width * 3 / 2;
                using (Bitmap image = new Bitmap(int_ImageWidth, height))
                {
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        //白色背景
                        //g.Clear(Color.White);
                        using (LinearGradientBrush b = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), GetRandomColor(), GetRandomColor(), LinearGradientMode.BackwardDiagonal))
                            g.FillRectangle(b, 0, 0, image.Width, image.Height);

                        //随机字体和颜色的验证码字符
                        int _x = -4, _y = 0;
                        for (int int_index = 0; int_index < code.Length; ++int_index)
                        {
                            _x += CAPTCHA_RANDOM.Next(width - width / 3, width + width / 3);
                            _y = CAPTCHA_RANDOM.Next(-2, 3);
                            string str_char = code.Substring(int_index, 1);
                            //str_char = CAPTCHA_RANDOM.Next(2) == 1 ? str_char.ToLower() : str_char.ToUpper();
                            using (Brush newBrush = new SolidBrush(GetRandomColor()))//随机颜色
                                g.DrawString(str_char, fonts[CAPTCHA_RANDOM.Next(fonts.Length)], newBrush, _x, _y);
                        }

                        //画图片的背景噪音线
                        using (Pen p = new Pen(GetRandomColor()))
                        {
                            int x1 = CAPTCHA_RANDOM.Next(image.Width / 8);
                            int x2 = CAPTCHA_RANDOM.Next(image.Width / 8 * 7, image.Width);
                            int y1 = CAPTCHA_RANDOM.Next(image.Height / 4, image.Height / 4 * 2);
                            int y2 = CAPTCHA_RANDOM.Next(image.Height / 4 * 2, image.Height / 4 * 3);
                            g.DrawLine(p, x1, y1, x2, y2);
                        }
                        using (Pen p = new Pen(GetRandomColor()))
                        {
                            int x1 = CAPTCHA_RANDOM.Next(image.Width / 8);
                            int x2 = CAPTCHA_RANDOM.Next(image.Width / 8 * 7, image.Width);
                            int y1 = CAPTCHA_RANDOM.Next(image.Height / 4 * 2, image.Height / 4 * 3);
                            int y2 = CAPTCHA_RANDOM.Next(image.Height / 4, image.Height / 4 * 2);
                            g.DrawLine(p, x1, y1, x2, y2);
                        }

                        //画图片的前景噪音点
                        //for (int i = 0; i < 10; ++i)
                        //{
                        //    int x = CAPTCHA_RANDOM.Next(image.Width);
                        //    int y = CAPTCHA_RANDOM.Next(image.Height);

                        //    image.SetPixel(x, y, Color.FromArgb(CAPTCHA_RANDOM.Next(0, 256), CAPTCHA_RANDOM.Next(0, 256), CAPTCHA_RANDOM.Next(0, 256)));
                        //}
                        //图片扭曲
                        //image = TwistImage(image, true, CAPTCHA_RANDOM.Next(1, 3), CAPTCHA_RANDOM.Next(4, 6));//
                        //灰色边框
                        //g.DrawRectangle(new Pen(Color.LightGray, 1), 0, 0, int_ImageWidth - 1, (letterHeight - 1));
                        //将生成的图片发回客户端
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, ImageFormat.Png);
                            context.Response.ClearContent(); //需要输出图象信息 要修改HTTP头 
                            context.Response.ContentType = "image/png";
                            context.Response.BinaryWrite(ms.ToArray());
                        }
                    }
                }
            }
            finally
            {
                foreach (Font f in fonts)
                    f.Dispose();
                fonts = null;
            }
        }
        ///// <summary>
        ///// 正弦曲线Wave扭曲图片
        ///// </summary>
        ///// <param name="srcBmp">图片路径</param>
        ///// <param name="bXDir">如果扭曲则选择为True</param>
        ///// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        ///// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        ///// <returns></returns>
        //public System.Drawing.Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        //{
        //    double PI = 6.283185307179586476925286766559;
        //    Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
        //    // 将位图背景填充为白色
        //    Graphics graph = Graphics.FromImage(destBmp);
        //    graph.FillRectangle(new SolidBrush(Color.White), 0, 0, destBmp.Width, destBmp.Height);
        //    graph.Dispose();
        //    double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;
        //    for (int i = 0; i < destBmp.Width; i++)
        //    {
        //        for (int j = 0; j < destBmp.Height; j++)
        //        {
        //            double dx = 0;
        //            dx = bXDir ? (PI * (double)j) / dBaseAxisLen : (PI * (double)i) / dBaseAxisLen;
        //            dx += dPhase;
        //            double dy = Math.Sin(dx);

        //            // 取得当前点的颜色
        //            int nOldX = 0, nOldY = 0;
        //            nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
        //            nOldY = bXDir ? j : j + (int)(dy * dMultValue);

        //            Color color = srcBmp.GetPixel(i, j);
        //            if (nOldX >= 0 && nOldX < destBmp.Width
        //             && nOldY >= 0 && nOldY < destBmp.Height)
        //            {
        //                destBmp.SetPixel(nOldX, nOldY, color);
        //            }
        //        }
        //    }
        //    srcBmp.Dispose();
        //    return destBmp;
        //}
        /// <summary>
        /// 字体随机颜色
        /// </summary>
        /// <returns></returns>
        private static Color GetRandomColor()
        {
            return Color.FromArgb(CAPTCHA_RANDOM.Next(0, 256), CAPTCHA_RANDOM.Next(0, 256), CAPTCHA_RANDOM.Next(0, 256));

            //return Color.FromArgb(CAPTCHA_RANDOM.Next(min, max), CAPTCHA_RANDOM.Next(min, max), CAPTCHA_RANDOM.Next(min, max));

            //Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
            //System.Threading.Thread.Sleep(RandomNum_First.Next(50));
            //Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);

            //  //为了在白色背景上显示，尽量生成深色
            //int int_Red = RandomNum_First.Next(180);
            //int int_Green = RandomNum_Sencond.Next(180);
            //int int_Blue = (int_Red + int_Green > 300) ? 0 : 400 - int_Red - int_Green;
            //int_Blue = (int_Blue > 255) ? 255 : int_Blue;
            //return Color.FromArgb(int_Red, int_Green, int_Blue);

            //switch (style)
            //{
            //    case 1: return Color.FromArgb(CAPTCHA_RANDOM.Next(0, 256), CAPTCHA_RANDOM.Next(0, 256), CAPTCHA_RANDOM.Next(0, 256));
            //    case 2: return Color.FromArgb(CAPTCHA_RANDOM.Next(0, 256), CAPTCHA_RANDOM.Next(0, 256), CAPTCHA_RANDOM.Next(0, 256));
            //    default: return Color.FromArgb(CAPTCHA_RANDOM.Next(0, 256), CAPTCHA_RANDOM.Next(0, 256), CAPTCHA_RANDOM.Next(0, 256));
            //}
        }
    }
}
