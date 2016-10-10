using System;
using System.IO;
using System.Web;
using System.Text;
using System.Reflection;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;
using System.Collections.Generic;
using Cnaws.Management.Modules;
using System.Text.RegularExpressions;
using System.Data.Common;

namespace Cnaws.Web.Installer.Controllers
{
    public sealed class Install : ResourceDataController
    {
        protected override Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }
        protected override string Namespace
        {
            get { return "Cnaws.Web.Installer"; }
        }

        protected override string GetVirtualUrl(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Concat(Application.Settings.RootUrl, "install");
            return GetUrl(string.Concat("/install/", path));
        }
        protected override string GetVirtualRes(string path)
        {
            return GetRes(string.Concat("/install/", path));
        }

        private bool SystemFolderCheck(string foldername)
        {
            try
            {
                string mapPath = Server.MapPath("~/" + foldername);
                DirectoryInfo dir = new DirectoryInfo(mapPath);
                if (!dir.Exists)
                    dir.Create();
                using (FileStream stream = new FileStream(mapPath + @"\a.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    stream.Close();
                if (File.Exists(mapPath + @"\a.txt"))
                {
                    File.Delete(mapPath + @"\a.txt");
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        private static bool TempTest()
        {
            bool flag;
            string str = Guid.NewGuid().ToString();
            string path = Path.GetTempPath() + str;
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine(DateTime.Now);
                }
                using (StreamReader reader = new StreamReader(path))
                {
                    reader.ReadLine();
                    flag = true;
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        private void InvokeInstall(Type[] types, MethodInfo install)
        {
            MethodInfo method;
            foreach (Type type in types)
            {
                method = install.MakeGenericMethod(type);
                method.Invoke(null, new object[] { DataSource });
            }
        }
        private sealed class TableName : IDbReader
        {
            public string Name = null;

            void IDbReader.ReadRow(DbDataReader reader)
            {
                Name = reader.GetString(0);
            }
        }
        private Dictionary<string, bool> GetAllTables()
        {
            if (DataSource.Type == DataSourceType.MSSQL)
            {
                IList<TableName> list = DataSource.ExecuteReader<TableName>("SELECT [name] FROM [sys].[tables];");
                Dictionary<string, bool> dict = new Dictionary<string, bool>(list.Count);
                foreach (TableName tn in list)
                    dict.Add(tn.Name, true);
                return dict;
            }
            return new Dictionary<string, bool>();
        }
        private void InstallDatabase()
        {
            string module = Request.Form["Module"];
            if (!string.IsNullOrEmpty(module))
            {
                MethodInfo install;
                string[] modules = module.Split(',');
                Type[] types = Array.ConvertAll(modules, new Converter<string, Type>((x) =>
                {
                    return Type.GetType(x.Replace(';', ','));
                }));
                install = TType<Module>.Type.GetMethod("Install", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod);
                InvokeInstall(types, install);
                install = TType<Module>.Type.GetMethod("InstallInit", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod);
                InvokeInstall(types, install);
            }
        }

        public void Index()
        {
            if ("POST".Equals(Request.HttpMethod))
                Step(int.Parse(Request.Form["Step"]));
            else
                Step(1);
        }
        private void Step(int step)
        {
            this["Version"] = TType<HttpModule>.Type.Assembly.GetName().Version.ToString(3);
            switch (step)
            {
                case 2:
                    {
                        int error = 0;
                        StringBuilder builder = new StringBuilder();
                        string str = "App_Data,Bin,themes,uploads,runtime";
                        foreach (string str2 in str.Split(','))
                        {
                            if (!SystemFolderCheck(str2))
                            {
                                builder.Append(string.Concat("<li><cite><img src=\"", GetRes("static/images/error.gif"), "\" alt=\"失败\"/></cite><a href=\"#\">对 ", str2, " 目录没有写入和删除权限!</a></li>"));
                                error = 1;
                            }
                            else
                            {
                                builder.Append(string.Concat("<li><cite><img src=\"", GetRes("static/images/ok.gif"), "\" alt=\"成功\"/></cite><a href=\"#\">对 ", str2, " 目录权限验证通过!</a></li>"));
                            }
                        }
                        if (!TempTest())
                        {
                            builder.Append(string.Concat("<li><cite><img src=\"", GetRes("static/images/error.gif"), "\" alt=\"失败\"/></cite><a href=\"#\">>您没有对 ", Path.GetTempPath(), " 文件夹访问权限，详情参见安装文档.</a></li>"));
                            error = 1;
                        }
                        else
                        {
                            builder.Append(string.Concat("<li><cite><img src=\"", GetRes("static/images/ok.gif"), "\" alt=\"成功\"/></cite><a href=\"#\">反序列化验证通过!</a></li>"));
                        }
                        this["SystemValidCheck"] = builder.ToString();
                        this["IsError"] = error.ToString();
                    }
                    break;
                case 3:
                    {
                        bool unused;
                        string ns;
                        Assembly asm;
                        Module module;
                        string table;
                        Regex regex = new Regex(@",\s*");
                        StringBuilder sb = new StringBuilder();
                        Dictionary<string, bool> tables = GetAllTables();
                        foreach (FileInfo file in ModuleInfo.GetAllFiles(Context))
                        {
                            ns = string.Concat(file.Name.Substring(0, file.Name.Length - file.Extension.Length + 1), "Modules");
                            asm = Assembly.LoadFrom(file.FullName);
                            foreach (TypeInfo type in asm.DefinedTypes)
                            {
                                if (type.IsClass &&
                                    type.IsPublic &&
                                    !type.IsAbstract &&
                                    string.Equals(type.Namespace, ns, StringComparison.OrdinalIgnoreCase) &&
                                    TType<Module>.Type.IsAssignableFrom(type.UnderlyingSystemType))
                                {
                                    module = (Module)Activator.CreateInstance(type.UnderlyingSystemType);
                                    if (module.CanInstall)
                                    {
                                        table = module.GetTableName();
                                        sb.Append("<li><input id=\"");
                                        sb.Append(type.UnderlyingSystemType.Name);
                                        sb.Append("\" name=\"Module\" class=\"module\" type=\"checkbox\" checked=\"checked\" value=\"");
                                        sb.Append(regex.Replace(type.UnderlyingSystemType.ToString(true), ";"));
                                        sb.Append("\" /><label for=\"");
                                        sb.Append(type.UnderlyingSystemType.Name);
                                        sb.Append("\">");
                                        sb.Append(table);
                                        sb.Append("</label><span>");
                                        if (tables.TryGetValue(table, out unused))
                                            sb.Append("已存在");
                                        sb.Append("</span></li>");
                                    }
                                }
                            }
                        }
                        this["Modules"] = sb.ToString();
                    }
                    break;
                case 4:
                    InstallDatabase();
                    break;
                case 5:
                    {
                        string username = Request.Form["adminName"];
                        if (string.IsNullOrEmpty(username))
                            throw new ArgumentException("管理员名称格式错误", "adminName");
                        string password = Request.Form["adminPassword"];
                        if (string.IsNullOrEmpty(password))
                            throw new ArgumentException("管理员密码格式错误", "adminPassword");
                        string confirmpwd = Request.Form["confirmPassword"];
                        if (!password.Equals(confirmpwd))
                            throw new ArgumentException("两次密码不一致", "confirmPassword");
                        (new Admin()
                        {
                            Name = username,
                            Password = password,
                            RoleId = 1,
                            CreationDate = DateTime.Now,
                            Email = "admin@admin.com"
                        }).Insert(DataSource);
                        this["UserName"] = username;
                        this["Password"] = password;
                    }
                    break;
            }
            RenderTemplate(string.Concat("install_", step.ToString(), ".html"));
        }

        public void Static(string name, Arguments args)
        {
            RenderResource(name, args, true);
        }

#if(DEBUG)
        public void Db(string name, Arguments args)
        {
            InstallDatabase();
            (new Admin()
            {
                Name = "admin",
                Password = "admin888",
                RoleId = 1,
                CreationDate = DateTime.Now,
                Email = "admin@admin.com"
            }).Insert(DataSource);
        }
#endif
    }
}
