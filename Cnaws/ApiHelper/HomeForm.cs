using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cnaws.Json;
using Cnaws.Web;
using System.Reflection;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;
using Cnaws.Data;
using System.Security.Policy;
using Cnaws;

namespace ApiHelper
{
    [ComVisible(true)]
    public partial class HomeForm : Form
    {
        public HomeForm()
        {
            InitializeComponent();

            browser.ObjectForScripting = this;
        }
        private void Clear()
        {
#if (DEBUG)
            Controller.ApiMethods.Clear();
            Controller.ApiTypes.Clear();
#endif
        }

        public void Alert(object value)
        {
            MessageBox.Show(this, value.ToString(), Text);
        }
        public void LoadProject()
        {
            try
            {
                Clear();
                if (fileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    FileInfo file = new FileInfo(fileDialog.FileName);
                    string ns = string.Concat(file.Name.Substring(0, file.Name.Length - file.Extension.Length + 1), "Controllers");
                    Assembly asm = Assembly.LoadFrom(file.FullName);
                    foreach (TypeInfo type in asm.DefinedTypes)
                    {
                        if (type.IsClass && type.IsPublic && !type.IsAbstract && string.Equals(type.Namespace, ns, StringComparison.OrdinalIgnoreCase))
                        {
                            MethodInfo[] methods = type.UnderlyingSystemType.GetMethods(BindingFlags.Public | BindingFlags.Static);
                            foreach (MethodInfo method in methods)
                            {
                                if (method.Name.Length > 9 && method.Name.EndsWith("Helper"))
                                    method.Invoke(null, null);
                            }
                        }
                    }
                    StringBuilder sb = new StringBuilder();

#if (DEBUG)
                    foreach (Controller.ApiMethod method in Controller.ApiMethods)
                    {
                        if (method.GetUrl().ToLower().IndexOf("api") <= 0)
                        {
                            sb.Append("<dl>");

                            sb.Append("<dt>").Append(method.Summary).Append("</dt>");

                            sb.Append("<dd class=\"url\"><p class=\"title\">地址：</p><p>").Append(method.GetUrl()).Append("<a href=\"javascript:;\">测试</a></p></dd>");

                            sb.Append("<dd class=\"args\"><p class=\"title\">参数：</p><table border=\"1\" cellpadding=\"0\" cellspacing=\"0\"><tr><th>参数名</th><th>参数类型</th><th>参数说明</th></tr>");
                            foreach (Controller.ApiArgument arg in method.Arguments)
                                sb.Append("<tr><td class=\"name\">").Append(arg.Name).Append("</td><td class=\"type\">").Append(GetTypeString(arg.Value)).Append("</td><td class=\"sum\">").Append(arg.Summary).Append("</td></tr>");
                            sb.Append("</table></dd>");

                            sb.Append("<dd class=\"result\"><p class=\"title\">返回值：</p><table border=\"1\" cellpadding=\"0\" cellspacing=\"0\"><tr><th>code</th><th>code说明</th><th>data类型</th><th>data说明</th></tr>");
                            foreach (Controller.ApiResult result in method.Results)
                                sb.Append("<tr><td class=\"code\">").Append(result.Code).Append("</td><td class=\"codes\">").Append(result.CodeSummary).Append("</td><td class=\"data\">").Append(GetTypeString(result.Data)).Append("</td><td class=\"datas\">").Append(result.DataSummary).Append("</td></tr>");
                            sb.Append("</table></dd>");

                            sb.Append("</dl>");
                        }
                    }

                    bool temp;
                    Dictionary<Type, bool> dict = new Dictionary<Type, bool>();
                    Reloop:
                    try
                    {
                        foreach (Type type in Controller.ApiTypes.Values)
                        {
                            if (type.IsPrimitive || typeof(string) == type || typeof(DateTime) == type)
                                continue;

                            if (!dict.TryGetValue(type, out temp))
                            {
                                sb.Append(LoadType(type));
                                dict.Add(type, true);
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        goto Reloop;
                    }
#endif

                    //browser.Document.InvokeScript("BindProject", new object[] { sb.ToString() });
                    browser.DocumentText = Properties.Resources.MainPage.Replace("/*$style$*/", Properties.Resources.css).Replace("/*$jquery$*/", Properties.Resources.jquery_1_11_1_min).Replace("/*$script$*/", Properties.Resources.js).Replace("/*$content$*/", sb.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private static string GetId(Type type)
        {
            return string.Concat("type_", type.FullName.GetHashCode());
        }
        //public string LoadType(string s)
        //{
        //    return LoadType(Controller.ApiTypes[s]);
        //}
        private string LoadType(Type type)
        {
            if (type.IsPrimitive || typeof(string) == type || typeof(DateTime) == type || typeof(Money) == type)
                return null;

            StringBuilder sb = new StringBuilder();
            sb.Append("<table id=\"").Append(GetId(type)).Append("\" border=\"1\" cellpadding=\"0\" cellspacing=\"0\" style=\"display:none\">");
            if (type.IsEnum)
            {
                sb.Append("<tr><th>值</th><th>说明</th></tr>");
                string[] array = null;
                string[] names = Enum.GetNames(type);
                DescriptionAttribute attr = type.GetCustomAttribute<DescriptionAttribute>(false);
                if (attr != null)
                    array = attr.Description.Split('\t');
                foreach (string name in names)
                {
                    string summary = null;
                    if (attr != null)
                        summary = array[(int)Enum.Parse(type, name)];
                    sb.Append("<tr><td>").Append(name).Append("</td><td>").Append(summary).Append("</td></tr>");
                }
            }
            else
            {
                sb.Append("<tr><th>名称</th><th>类型</th><th>说明</th></tr>");
                Dictionary<string, FieldInfo> fs = type.GetStaticAllNameSetFields<DataColumnAttribute>();
                foreach (KeyValuePair<string, FieldInfo> pair in fs)
                {
                    string summary = null;
                    DescriptionAttribute attr = pair.Value.GetCustomAttribute<DescriptionAttribute>(false);
                    if (attr != null)
                        summary = attr.Description;
#if (DEBUG)
                    Controller.ApiTypes[pair.Value.FieldType.FullName] = pair.Value.FieldType;
#endif
                    sb.Append("<tr><td>").Append(pair.Key).Append("</td><td>").Append(GetTypeString(pair.Value.FieldType)).Append("</td><td>").Append(summary).Append("</td></tr>");
                }
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        private string GetTypeString(Type type, bool loop = false)
        {
            if (type == null)
                return null;

            if (type.IsPrimitive || typeof(string) == type || typeof(DateTime) == type)
                return type.Name;

            if (type.IsArray)
            {
                Type t = type.GetElementType();
                if (t.IsPrimitive || typeof(string) == t || typeof(DateTime) == t)
                    return string.Concat(t.Name, "[]");

#if (DEBUG)
                Controller.ApiTypes[t.FullName] = t;
#endif
                return string.Concat("<a href=\"javascript:void(0)\" typedata=\"\" onclick=\"Helper.LoadType('", t.Name, "','", GetId(t), "')\">", t.Name, "</a>", "[]");
            }

            TypesInfo info = new TypesInfo(type);
            if (info.IsIList || info.IsItList)
            {
                Type[] types;
                if (info.IsItList)
                    types = info.TList;
                else
                    types = type.GetGenericArguments();
                if (types.Length == 1)
                {
                    Type t = types[0];
                    if (t.IsPrimitive || typeof(string) == t || typeof(DateTime) == t)
                        return string.Concat(t.Name, "[]");

#if (DEBUG)
                    Controller.ApiTypes[t.FullName] = t;
#endif
                    return string.Concat("<a href=\"javascript:void(0)\" typedata=\"\" onclick=\"Helper.LoadType('", t.Name, "','", GetId(t), "')\">", t.Name, "</a>", "[]");
                }
            }

            string name = type.Name;
            if (type.IsGenericType)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(type.Name.Split('`')[0]);
                sb.Append("&lt;");
                int i = 0;
                foreach (Type t in type.GenericTypeArguments)
                {
                    if (i++ > 0)
                        sb.Append(", ");
                    sb.Append(GetTypeString(t, true));
                }
                sb.Append("&gt;");
                name = sb.ToString();
            }
            if (loop)
                return name;
            return string.Concat("<a href=\"javascript:void(0)\" typedata=\"\" onclick=\"Helper.LoadType('", name, "','", GetId(type), "')\">", name, "</a>");
        }
        public void Save()
        {
            if (saveDialog.ShowDialog(this) == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    using (StreamWriter w = new StreamWriter(fs))
                        w.Write(browser.DocumentText.Replace("var CLIENT = true;", "var CLIENT = false;"));
                }
            }
        }

        private void HomeForm_Load(object sender, EventArgs e)
        {
            browser.DocumentText = Properties.Resources.MainPage.Replace("/*$style$*/", Properties.Resources.css).Replace("/*$jquery$*/", Properties.Resources.jquery_1_11_1_min).Replace("/*$script$*/", Properties.Resources.js).Replace("/*$content$*/", string.Empty);
        }
        public void DoResize()
        {
            browser.Document.InvokeScript("OnResize", new object[] { browser.Width, browser.Height });
        }
        private void HomeForm_Resize(object sender, EventArgs e)
        {
            DoResize();
        }
        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            DoResize();
        }
    }
}
