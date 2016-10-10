using Cnaws.Json;
using Cnaws.Web.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Cnaws.Web.Controllers
{
    public class FileSystem : Controller
    {
        private static readonly bool ENABLE;
        private static readonly string PATH;
        private static readonly string URL;
        private static readonly ImageMarkType MARK;
        private static readonly string TEXT;
        private static readonly ImageMarkRegion REGION;
        private static readonly int WIDTH;
        private static readonly int HEIGHT;

        static FileSystem()
        {
            FileSystemSection section = FileSystemSection.GetSection();
            ENABLE = section.Enable;
            PATH = section.Path;
            URL = section.Url;
            MARK = section.Mark;
            TEXT = section.Text;
            REGION = section.Region;
            WIDTH = section.Width;
            HEIGHT = section.Height;
            if (string.IsNullOrEmpty(PATH))
                PATH = Utility.UploadDir;
        }

        protected virtual bool CheckRight(Arguments args = null)
        {
            //string origin = Request.Headers["Origin"];
            //if (!string.IsNullOrEmpty(origin))
            //{
            //    string token = Request.QueryString["token"];
            //    if (!string.IsNullOrEmpty(token))
            //    {
            //        //Response.Headers.Add("Access-Control-Allow-Origin", origin);
            //        //Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST");
            //        PassportAuthentication.SetAuthToken(token, Context);
            //    }
            //}
            string token = Request.QueryString["token"];
            if (!string.IsNullOrEmpty(token))
                PassportAuthentication.SetAuthToken(token, Context);

            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                return true;

            if (args != null && args.Count > 0)
            {
                token = string.Join("/", args.ToArray());
                PassportAuthentication.SetAuthToken(token, Context);
                if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                    return true;
            }

            Unauthorized();
            return false;
        }

        #region file_manager
        protected sealed class file_or_dir
        {
            public bool is_dir;
            public bool has_file;
            public long filesize;
            public bool is_photo;
            public string filetype;
            public string filename;
            public string datetime;
        }
        protected sealed class file_manager_result
        {
            public string moveup_dir_path;
            public string current_dir_path;
            public string current_url;
            public int total_count;
            public List<file_or_dir> file_list;

            public file_manager_result()
            {
                file_list = new List<file_or_dir>();
            }
        }
        protected virtual string GetDirectory(long value)
        {
            return value.ToString();
        }

        private string GetPath(params object[] args)
        {
            return (new Uri(string.Concat(args))).LocalPath;
        }
        private void InitPathAndUrl(out string path, out string url)
        {
            Uri uri;
            string dir, sub;
            if (User.Identity.IsAdmin)
            {
                dir = string.Empty;
                sub = string.Empty;
            }
            else
            {
                dir = "user/";
                sub = string.Concat(GetDirectory(User.Identity.Id), "/");
            }

            uri = new Uri(PATH, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri)
                path = GetPath(PATH.TrimEnd(Path.DirectorySeparatorChar), Path.DirectorySeparatorChar, dir, sub);
            else
                path = GetPath(Server.MapPath("~").TrimEnd(Path.DirectorySeparatorChar), Path.DirectorySeparatorChar, PATH.TrimEnd(Path.DirectorySeparatorChar), Path.DirectorySeparatorChar, dir, sub);

            if (string.IsNullOrEmpty(URL))
                url = string.Concat(Application.Settings.RootUrl, PATH.Trim('/'), '/', dir, sub);
            else
                url = string.Concat(URL.TrimEnd('/'), '/', dir, sub);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        private void FileManagerRequest()
        {
            string currentPath = string.Empty;
            string currentUrl = string.Empty;
            string currentDirPath = string.Empty;
            string moveupDirPath = string.Empty;
            string rootUrl;
            string dirPath;
            InitPathAndUrl(out dirPath, out rootUrl);

            string dirName = Request.QueryString["dir"];
            if (!string.IsNullOrEmpty(dirName))
            {
                if (Array.IndexOf("image,flash,media,file".Split(','), dirName) == -1)
                {
                    Response.Write("Invalid Directory name.");
                    End();
                    return;
                }
                dirPath = string.Concat(dirPath, dirName, "/");
                rootUrl = string.Concat(rootUrl, dirName, "/");
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
            }

            string path = Request.QueryString["path"] ?? string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                currentPath = dirPath;
                currentUrl = rootUrl;
                currentDirPath = string.Empty;
                moveupDirPath = string.Empty;
            }
            else
            {
                currentPath = string.Concat(dirPath, path);
                currentUrl = string.Concat(rootUrl, path);
                currentDirPath = path;
                moveupDirPath = Regex.Replace(currentDirPath, @"(.*?)[^\/]+\/$", "$1");
            }

            string order = Request.QueryString["order"] ?? string.Empty;
            order = order.ToLower();

            //不允许使用..移动到上一级目录
            if (Regex.IsMatch(path, @"\.\."))
            {
                Response.Write("Access is not allowed.");
                End();
                return;
            }
            //最后一个字符不是/
            if (path != string.Empty && !path.EndsWith("/"))
            {
                Response.Write("Parameter is not valid.");
                End();
                return;
            }
            //目录不存在或不是目录
            if (!Directory.Exists(currentPath))
            {
                Response.Write("Directory does not exist.");
                End();
                return;
            }

            //遍历目录取得文件信息
            string[] dirList = Directory.GetDirectories(currentPath);
            string[] fileList = Directory.GetFiles(currentPath);
            switch (order)
            {
                case "size":
                    Array.Sort(dirList, new NameSorter());
                    Array.Sort(fileList, new SizeSorter());
                    break;
                case "type":
                    Array.Sort(dirList, new NameSorter());
                    Array.Sort(fileList, new TypeSorter());
                    break;
                case "name":
                default:
                    Array.Sort(dirList, new NameSorter());
                    Array.Sort(fileList, new NameSorter());
                    break;
            }

            file_manager_result result = new file_manager_result();
            result.moveup_dir_path = moveupDirPath;
            result.current_dir_path = currentDirPath;
            result.current_url = currentUrl;
            result.total_count = dirList.Length + fileList.Length;
            for (int i = 0; i < dirList.Length; i++)
            {
                DirectoryInfo dir = new DirectoryInfo(dirList[i]);
                file_or_dir hash = new file_or_dir();
                hash.is_dir = true;
                hash.has_file = (dir.GetFileSystemInfos().Length > 0);
                hash.filesize = 0;
                hash.is_photo = false;
                hash.filetype = "";
                hash.filename = dir.Name;
                hash.datetime = dir.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                result.file_list.Add(hash);
            }
            for (int i = 0; i < fileList.Length; i++)
            {
                FileInfo file = new FileInfo(fileList[i]);
                file_or_dir hash = new file_or_dir();
                hash.is_dir = false;
                hash.has_file = false;
                hash.filesize = file.Length;
                hash.is_photo = (Array.IndexOf("png,jpg,gif,bmp,jpeg".Split(','), file.Extension.Substring(1).ToLower()) >= 0);
                hash.filetype = file.Extension.Substring(1);
                hash.filename = file.Name;
                hash.datetime = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                result.file_list.Add(hash);
            }
            RenderFileManagerResult(Context, result);
        }
        private sealed class NameSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                    return 0;
                if (x == null)
                    return -1;
                if (y == null)
                    return 1;
                FileInfo xInfo = new FileInfo(x.ToString());
                FileInfo yInfo = new FileInfo(y.ToString());
                return xInfo.FullName.CompareTo(yInfo.FullName);
            }
        }
        private sealed class SizeSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                    return 0;
                if (x == null)
                    return -1;
                if (y == null)
                    return 1;
                FileInfo xInfo = new FileInfo(x.ToString());
                FileInfo yInfo = new FileInfo(y.ToString());
                return xInfo.Length.CompareTo(yInfo.Length);
            }
        }
        private sealed class TypeSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                    return 0;
                if (x == null)
                    return -1;
                if (y == null)
                    return 1;
                FileInfo xInfo = new FileInfo(x.ToString());
                FileInfo yInfo = new FileInfo(y.ToString());
                return xInfo.Extension.CompareTo(yInfo.Extension);
            }
        }
        public void FileManager()
        {
            if (CheckRight())
                FileManagerRequest();
        }
        protected virtual void RenderFileManagerResult(HttpContext context, file_manager_result result)
        {
            context.Response.ContentType = "application/x-javascript";
            context.Response.Write(JsonValue.Serialize<file_manager_result>(result));
        }
        #endregion

        #region upload
        private sealed class upload_error_result
        {
            public int error;
            public string message;
        }
        protected sealed class upload_result
        {
            public int error;
            public string url;
        }
        ///// <summary>      
        ///// Creating a Watermarked Photograph with GDI+ for .NET      
        ///// </summary>      
        ///// <param name="rSrcImgPath">原始图片的物理路径</param>      
        ///// <param name="rMarkImgPath">水印图片的物理路径</param>      
        ///// <param name="rMarkText">水印文字（不显示水印文字设为空串）</param>      
        ///// <param name="rDstImgPath">输出合成后的图片的物理路径</param>      
        //public void BuildWatermark(string rSrcImgPath, string rMarkImgPath, string rMarkText, string rDstImgPath)
        //{
        //    //以下（代码）从一个指定文件创建了一个Image 对象，然后为它的 Width 和 Height定义变量。      
        //    //这些长度待会被用来建立一个以24 bits 每像素的格式作为颜色数据的Bitmap对象。      
        //    Image imgPhoto = Image.FromFile(rSrcImgPath);
        //    int phWidth = imgPhoto.Width;
        //    int phHeight = imgPhoto.Height;
        //    Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);
        //    bmPhoto.SetResolution(72, 72);
        //    Graphics grPhoto = Graphics.FromImage(bmPhoto);
        //    //这个代码载入水印图片，水印图片已经被保存为一个BMP文件，以绿色(A=0,R=0,G=255,B=0)作为背景颜色。      
        //    //再一次，会为它的Width 和Height定义一个变量。      
        //    Image imgWatermark = new Bitmap(rMarkImgPath);
        //    int wmWidth = imgWatermark.Width;
        //    int wmHeight = imgWatermark.Height;
        //    //这个代码以100%它的原始大小绘制imgPhoto 到Graphics 对象的（x=0,y=0）位置。      
        //    //以后所有的绘图都将发生在原来照片的顶部。      
        //    grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
        //    grPhoto.DrawImage(
        //         imgPhoto,
        //         new Rectangle(0, 0, phWidth, phHeight),
        //         0,
        //         0,
        //         phWidth,
        //         phHeight,
        //         GraphicsUnit.Pixel);
        //    //为了最大化版权信息的大小，我们将测试7种不同的字体大小来决定我们能为我们的照片宽度使用的可能的最大大小。      
        //    //为了有效地完成这个，我们将定义一个整型数组，接着遍历这些整型值测量不同大小的版权字符串。      
        //    //一旦我们决定了可能的最大大小，我们就退出循环，绘制文本      
        //    int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };
        //    Font crFont = null;
        //    SizeF crSize = new SizeF();
        //    for (int i = 0; i < 7; i++)
        //    {
        //        crFont = new Font("arial", sizes[i],
        //              FontStyle.Bold);
        //        crSize = grPhoto.MeasureString(rMarkText,
        //              crFont);
        //        if ((ushort)crSize.Width < (ushort)phWidth)
        //            break;
        //    }
        //    //因为所有的照片都有各种各样的高度，所以就决定了从图象底部开始的5%的位置开始。      
        //    //使用rMarkText字符串的高度来决定绘制字符串合适的Y坐标轴。      
        //    //通过计算图像的中心来决定X轴，然后定义一个StringFormat 对象，设置StringAlignment 为Center。      
        //    int yPixlesFromBottom = (int)(phHeight * .05);
        //    float yPosFromBottom = ((phHeight -
        //         yPixlesFromBottom) - (crSize.Height / 2));
        //    float xCenterOfImg = (phWidth / 2);
        //    StringFormat StrFormat = new StringFormat();
        //    StrFormat.Alignment = StringAlignment.Center;
        //    //现在我们已经有了所有所需的位置坐标来使用60%黑色的一个Color(alpha值153)创建一个SolidBrush 。      
        //    //在偏离右边1像素，底部1像素的合适位置绘制版权字符串。      
        //    //这段偏离将用来创建阴影效果。使用Brush重复这样一个过程，在前一个绘制的文本顶部绘制同样的文本。      
        //    SolidBrush semiTransBrush2 =
        //         new SolidBrush(Color.FromArgb(153, 0, 0, 0));
        //    grPhoto.DrawString(rMarkText,
        //         crFont,
        //         semiTransBrush2,
        //         new PointF(xCenterOfImg + 1, yPosFromBottom + 1),
        //         StrFormat);
        //    SolidBrush semiTransBrush = new SolidBrush(
        //         Color.FromArgb(153, 255, 255, 255));
        //    grPhoto.DrawString(rMarkText,
        //         crFont,
        //         semiTransBrush,
        //         new PointF(xCenterOfImg, yPosFromBottom),
        //         StrFormat);
        //    //根据前面修改后的照片创建一个Bitmap。把这个Bitmap载入到一个新的Graphic对象。      
        //    Bitmap bmWatermark = new Bitmap(bmPhoto);
        //    bmWatermark.SetResolution(
        //         imgPhoto.HorizontalResolution,
        //         imgPhoto.VerticalResolution);
        //    Graphics grWatermark =
        //         Graphics.FromImage(bmWatermark);
        //    //通过定义一个ImageAttributes 对象并设置它的两个属性，我们就是实现了两个颜色的处理，以达到半透明的水印效果。      
        //    //处理水印图象的第一步是把背景图案变为透明的(Alpha=0, R=0, G=0, B=0)。我们使用一个Colormap 和定义一个RemapTable来做这个。      
        //    //就像前面展示的，我的水印被定义为100%绿色背景，我们将搜到这个颜色，然后取代为透明。      
        //    ImageAttributes imageAttributes =
        //         new ImageAttributes();
        //    ColorMap colorMap = new ColorMap();
        //    colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
        //    colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
        //    ColorMap[] remapTable = { colorMap };
        //    //第二个颜色处理用来改变水印的不透明性。      
        //    //通过应用包含提供了坐标的RGBA空间的5x5矩阵来做这个。      
        //    //通过设定第三行、第三列为0.3f我们就达到了一个不透明的水平。结果是水印会轻微地显示在图象底下一些。      
        //    imageAttributes.SetRemapTable(remapTable,
        //         ColorAdjustType.Bitmap);
        //    float[][] colorMatrixElements = {
        //                                         new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
        //                                         new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
        //                                         new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
        //                                         new float[] {0.0f,  0.0f,  0.0f,  0.3f, 0.0f},
        //                                         new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
        //                                    };
        //    ColorMatrix wmColorMatrix = new
        //         ColorMatrix(colorMatrixElements);
        //    imageAttributes.SetColorMatrix(wmColorMatrix,
        //         ColorMatrixFlag.Default,
        //         ColorAdjustType.Bitmap);
        //    //随着两个颜色处理加入到imageAttributes 对象，我们现在就能在照片右手边上绘制水印了。      
        //    //我们会偏离10像素到底部，10像素到左边。      
        //    int markWidth;
        //    int markHeight;
        //    //mark比原来的图宽      
        //    if (phWidth <= wmWidth)
        //    {
        //        markWidth = phWidth - 10;
        //        markHeight = (markWidth * wmHeight) / wmWidth;
        //    }
        //    else if (phHeight <= wmHeight)
        //    {
        //        markHeight = phHeight - 10;
        //        markWidth = (markHeight * wmWidth) / wmHeight;
        //    }
        //    else
        //    {
        //        markWidth = wmWidth;
        //        markHeight = wmHeight;
        //    }
        //    int xPosOfWm = ((phWidth - markWidth) - 10);
        //    int yPosOfWm = 10;
        //    grWatermark.DrawImage(imgWatermark,
        //         new Rectangle(xPosOfWm, yPosOfWm, markWidth,
        //         markHeight),
        //         0,
        //         0,
        //         wmWidth,
        //         wmHeight,
        //         GraphicsUnit.Pixel,
        //         imageAttributes);
        //    //最后的步骤将是使用新的Bitmap取代原来的Image。 销毁两个Graphic对象，然后把Image 保存到文件系统。      
        //    imgPhoto = bmWatermark;
        //    grPhoto.Dispose();
        //    grWatermark.Dispose();
        //    imgPhoto.Save(rDstImgPath, ImageFormat.Jpeg);
        //    imgPhoto.Dispose();
        //    imgWatermark.Dispose();
        //}
        private void UploadRequest(bool mark, bool scale)
        {
            try
            {
                if (ENABLE)
                {
                    string saveUrl;
                    string dirPath;
                    InitPathAndUrl(out dirPath, out saveUrl);

                    //定义允许上传的文件扩展名
                    Hashtable extTable = new Hashtable();
                    extTable.Add("image", "png,jpg,gif,bmp,jpeg");
                    extTable.Add("flash", "swf,flv");
                    extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
                    extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2");

                    //最大文件大小
                    //int maxSize = 1000000;

                    HttpPostedFile imgFile = Request.Files["imgFile"];
                    if (imgFile == null)
                        throw new Exception("请选择文件。");

                    if (!Directory.Exists(dirPath))
                        throw new Exception("上传目录不存在。");

                    string dirName = Request.QueryString["dir"] ?? string.Empty;
                    if (string.IsNullOrEmpty(dirName))
                        dirName = "image";
                    if (!extTable.ContainsKey(dirName))
                        throw new Exception("目录名不正确。");

                    string fileName = imgFile.FileName;
                    string fileExt = Path.GetExtension(fileName).ToLower();

                    //if (imgFile.InputStream == null || imgFile.InputStream.Length > maxSize)
                    //    throw new Exception("上传文件大小超过限制。");

                    if (string.IsNullOrEmpty(fileExt) || Array.IndexOf(((string)extTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
                        throw new Exception(string.Concat("上传文件扩展名是不允许的扩展名。\n只允许", ((string)extTable[dirName]), "格式。"));

                    dirPath = string.Concat(dirPath, dirName, "/");
                    saveUrl = string.Concat(saveUrl, dirName, "/");
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);

                    string ymd = DateTime.Now.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
                    dirPath = string.Concat(dirPath, ymd, "/");
                    saveUrl = string.Concat(saveUrl, ymd, "/");
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);

                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
                    string filePath = dirPath + newFileName;

                    if ("image".Equals(dirName))
                    {
                        using (Image img = Image.FromStream(imgFile.InputStream))
                        {
                            if (MARK != ImageMarkType.None && mark)
                            {
                                using (Graphics g = Graphics.FromImage(img))
                                {
                                    if (MARK == ImageMarkType.Image)
                                    {
                                        using (Bitmap bm = new Bitmap(Server.MapPath(TEXT)))
                                        {
                                            int height = img.Height / 10;
                                            int width = bm.Width * height / bm.Height;
                                            Rectangle rect = new Rectangle(0, 0, width, height);
                                            switch (REGION)
                                            {
                                                case ImageMarkRegion.TopRight:
                                                    rect.Offset(img.Width - width, 0);
                                                    break;
                                                case ImageMarkRegion.BottomLeft:
                                                    rect.Offset(0, img.Height - height);
                                                    break;
                                                case ImageMarkRegion.BottomRight:
                                                    rect.Offset(img.Width - width, img.Height - height);
                                                    break;
                                            }
                                            g.DrawImage(bm, rect);
                                        }
                                    }
                                    else
                                    {
                                        int height = img.Height / 15;
                                        using (Font f = new Font("arial", height))
                                        {
                                            SizeF size = g.MeasureString(TEXT, f).ToSize();
                                            RectangleF rect = new RectangleF(0, 0, size.Width, size.Height);
                                            switch (REGION)
                                            {
                                                case ImageMarkRegion.TopRight:
                                                    rect.Offset(img.Width - size.Width, 0);
                                                    break;
                                                case ImageMarkRegion.BottomLeft:
                                                    rect.Offset(0, img.Height - size.Height);
                                                    break;
                                                case ImageMarkRegion.BottomRight:
                                                    rect.Offset(img.Width - size.Width, img.Height - size.Height);
                                                    break;
                                            }
                                            using (Brush b = new SolidBrush(Color.FromArgb(153, 0, 0, 0)))
                                                g.DrawString(TEXT, f, b, rect);
                                            rect.Offset(-1, -1);
                                            using (Brush b = new SolidBrush(Color.FromArgb(153, 255, 255, 255)))
                                                g.DrawString(TEXT, f, b, rect);
                                        }
                                    }
                                }
                            }

                            if ((WIDTH > 0 || HEIGHT > 0) && scale)
                            {
                                float cx = WIDTH;
                                float cy = HEIGHT;
                                if (cx <= 0)
                                    cx = cy / img.Height * img.Width;
                                if (cy <= 0)
                                    cy = cx / img.Width * img.Height;
                                using (Bitmap temp = new Bitmap((int)cx, (int)cy))
                                {
                                    using (Graphics g = Graphics.FromImage(temp))
                                        g.DrawImage(img, new RectangleF(0, 0, cx, cy), new RectangleF(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                                    temp.Save(filePath, img.RawFormat);
                                }
                            }
                            else
                            {
                                img.Save(filePath, img.RawFormat);
                            }
                        }
                    }
                    else
                    {
                        imgFile.SaveAs(filePath);
                    }

                    upload_result hash = new upload_result();
                    hash.error = 0;
                    hash.url = string.Concat(saveUrl, newFileName);
                    RenderUploadResult(Context, hash);
                }
                else
                {
                    showError(Context, "该站点不允许上传");
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                showError(Context, ex.Message);
            }
        }
        private void showError(HttpContext context, string message)
        {
            upload_error_result hash = new upload_error_result();
            hash.error = 1;
            hash.message = message;
            context.Response.ContentType = "text/html";
            context.Response.Write(JsonValue.Serialize<upload_error_result>(hash));
            End();
        }
        public void Upload(bool mark = false, bool scale = false, Arguments args = null)
        {
            if (CheckRight(args))
                UploadRequest(mark, scale);
        }
        public void UploadEx()
        {
            string origin = Context.Request.Headers["Origin"];
            if (!string.IsNullOrEmpty(origin))
                Context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
            Context.Response.ContentType = "text/html";
            Context.Response.Write(HttpUtility.UrlDecode(Context.Request.Url.Query.TrimStart('?')));
            End();
        }
        protected virtual void RenderUploadResult(HttpContext context, upload_result result)
        {
            Uri url = context.Request.Url;
            Uri referer = context.Request.UrlReferrer;
            if (referer != null
                && (!string.Equals(url.DnsSafeHost, referer.DnsSafeHost)
                || url.Port != referer.Port))
            {
                if ("1".Equals(context.Request.Headers["Upgrade-Insecure-Requests"]))
                {
                    string target = string.Concat(referer.Scheme, "://", referer.DnsSafeHost, referer.Port != 80 ? string.Concat(":", referer.Port.ToString()) : string.Empty, "/filesystem/uploadex", Utility.DefaultExt, '?', HttpUtility.UrlEncode(JsonValue.Serialize(result)));
                    Redirect(target);
                    End();
                    return;
                }
            }

            context.Response.ContentType = "text/html";
            context.Response.Write(JsonValue.Serialize(result));
            End();
        }
        #endregion
    }
}
