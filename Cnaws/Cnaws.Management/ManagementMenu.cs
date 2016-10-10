using System;
using System.IO;
using System.Web;
using System.Text;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Json;

namespace Cnaws.Management
{
    public sealed class ManagementMenu
    {
        [NonSerialized]
        private int _channel;
        [NonSerialized]
        private int _number;

        [JsonField("name")]
        private string _name;
        [JsonField("sub")]
        private List<ManagementSubMenu> _list;

        public ManagementMenu(int channel, int number, string name)
        {
            _channel = channel;
            _number = number;

            _name = name;
            _list = new List<ManagementSubMenu>();
        }

        public int Channel
        {
            get { return _channel; }
        }
        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public string Name
        {
            get { return _name; }
        }
        public List<ManagementSubMenu> Items
        {
            get { return _list; }
        }
        internal void LoadConfig(MemuConfig config)
        {
            List<ManagementSubMenu> items = new List<ManagementSubMenu>(_list.Count);
            for(int i = 0; i < _list.Count; ++i)
            {
                if (!config.IsHideSubMenu(_channel, _number, i))
                    items.Add(_list[i]);
            }
            _list = items;
        }

        public ManagementMenu AddSubMenu(string name, string url, string icon = null)
        {
            Items.Add(new ManagementSubMenu(name, url));
            if (!string.IsNullOrEmpty(icon))
                ManagementMenus.AddQuickMenu(new ManagementQuickMenu(name, url, icon));
            return this;
        }
    }
    public sealed class ManagementSubMenu
    {
        [JsonField("name")]
        private string _name;
        [JsonField("url")]
        private string _url;

        public ManagementSubMenu(string name, string url)
        {
            _name = name;
            _url = url;
        }

        public string Name
        {
            get { return _name; }
        }
        public string Url
        {
            get { return _url; }
        }
    }
    internal class ManagementMainMenu
    {
        [JsonField("name")]
        protected string _name;
        [JsonField("icon")]
        protected string _icon;

        public ManagementMainMenu(string name, string icon)
        {
            _name = name;
            _icon = icon;
        }

        internal virtual void InitMenu(Controller ctl)
        {
            _icon = ctl.GetRes(_icon);
        }

        public string Name
        {
            get { return _name; }
        }
        public string Icon
        {
            get { return _icon; }
        }
    }
    internal sealed class ManagementQuickMenu : ManagementMainMenu
    {
        [JsonField("url")]
        private string _url;

        public ManagementQuickMenu(string name, string url, string icon)
            : base(name, icon)
        {
            _url = url;
        }

        public string Url
        {
            get { return _url; }
        }
    }

    public abstract class ManagementMenus
    {
        private static readonly List<ManagementQuickMenu> QUICKMENUS;

        private List<ManagementMenu> _menus;

        static ManagementMenus()
        {
            QUICKMENUS = new List<ManagementQuickMenu>();
        }
        public ManagementMenus()
        {
            _menus = new List<ManagementMenu>();
            InitMenus();
        }

        internal static void AddQuickMenu(ManagementQuickMenu menu)
        {
            QUICKMENUS.Add(menu);
        }

        public IList<ManagementMenu> Menus
        {
            get { return _menus; }
        }

        protected abstract void InitMenus();

        protected ManagementMenu AddMenu(int channel, int number, string name)
        {
            ManagementMenu menu = new ManagementMenu(channel, number, name);
            _menus.Add(menu);
            return menu;
        }

        private static int Compare(ManagementMenu x, ManagementMenu y)
        {
            return x.Number - y.Number;
        }

        private static string BuildMainMenus(KeyValuePair<ManagementMainMenu, List<ManagementMenu>>[] list)
        {
            if (list.Length > 0)
            {
                return JsonValue.Serialize(Array.ConvertAll(list, new Converter<KeyValuePair<ManagementMainMenu, List<ManagementMenu>>, ManagementMainMenu>((x) =>
                {
                    return x.Key;
                })));
            }
            return "[]";
        }
        private static string BuildMenus(KeyValuePair<ManagementMainMenu, List<ManagementMenu>>[] list)
        {
            if (list.Length > 0)
            {
                return JsonValue.Serialize(Array.ConvertAll(list, new Converter<KeyValuePair<ManagementMainMenu, List<ManagementMenu>>, List<ManagementMenu>>((x) =>
                {
                    return x.Value;
                })));
            }
            return "[]";
        }
        private static List<KeyValuePair<ManagementMainMenu, List<ManagementMenu>>> BuildMenus(Controller ctl)
        {
            List<KeyValuePair<ManagementMainMenu, List<ManagementMenu>>> list = new List<KeyValuePair<ManagementMainMenu, List<ManagementMenu>>>(6);
            list.Add(new KeyValuePair<ManagementMainMenu, List<ManagementMenu>>(new ManagementMainMenu("全局", "static/images/icon01.png"), new List<ManagementMenu>()));
            list.Add(new KeyValuePair<ManagementMainMenu, List<ManagementMenu>>(new ManagementMainMenu("功能", "static/images/icon06.png"), new List<ManagementMenu>()));
            list.Add(new KeyValuePair<ManagementMainMenu, List<ManagementMenu>>(new ManagementMainMenu("栏目", "static/images/icon05.png"), new List<ManagementMenu>()));
            list.Add(new KeyValuePair<ManagementMainMenu, List<ManagementMenu>>(new ManagementMainMenu("模板", "static/images/icon04.png"), new List<ManagementMenu>()));
            list.Add(new KeyValuePair<ManagementMainMenu, List<ManagementMenu>>(new ManagementMainMenu("用户", "static/images/icon03.png"), new List<ManagementMenu>()));
            list.Add(new KeyValuePair<ManagementMainMenu, List<ManagementMenu>>(new ManagementMainMenu("数据", "static/images/icon02.png"), new List<ManagementMenu>()));

            int index;
            Type type;
            string asm;
            ManagementMenus instance;
            KeyValuePair<ManagementMainMenu, List<ManagementMenu>> pair;
            DirectoryInfo dir = new DirectoryInfo(ctl.Context.Server.MapPath("~/Bin"));
            foreach (FileInfo file in dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
            {
                index = file.Name.LastIndexOf('.');
                asm = file.Name.Substring(0, index);
                type = Type.GetType(string.Concat(asm, asm.EndsWith(".Management") ? string.Empty : ".Management", ".MenuList,", asm), false, true);
                if (type != null)
                {
                    instance = Activator.CreateInstance(type) as ManagementMenus;
                    if (instance != null && instance.Menus != null && instance.Menus.Count > 0)
                    {
                        foreach (ManagementMenu item in instance.Menus)
                        {
                            pair = list[item.Channel];
                            pair.Value.Add(item);
                        }
                    }
                }
            }

            foreach (KeyValuePair<ManagementMainMenu, List<ManagementMenu>> item in list)
            {
                item.Value.Sort(new Comparison<ManagementMenu>(Compare));
                for (int i = 0; i < item.Value.Count; ++i)
                    item.Value[i].Number = i;
            }

            ManagementMenu menu;
            MemuConfig config = new MemuConfig();
            for (int i = list.Count - 1; i >= 0; --i)
            {
                pair = list[i];
                if (config.IsHideMainMenu(i))
                {
                    list.RemoveAt(i);
                }
                else
                {
                    pair.Key.InitMenu(ctl);
                    for(int j = pair.Value.Count - 1; j >= 0; --j)
                    {
                        if (config.IsHideMenu(i, j))
                        {
                            pair.Value.RemoveAt(j);
                        }
                        else
                        {
                            menu = pair.Value[j];
                            for(int k = menu.Items.Count - 1; k >= 0; --k)
                            {
                                if (config.IsHideSubMenu(i, j, k))
                                    menu.Items.RemoveAt(k);
                            }
                        }
                    }
                }
            }

            return list;
        }
        private static string BuildQuickMenus(Controller ctl)
        {
            foreach (ManagementQuickMenu menu in QUICKMENUS)
                menu.InitMenu(ctl);
            return JsonValue.Serialize(QUICKMENUS);
        }
        internal static string GetAll(Controller ctl)
        {
            QUICKMENUS.Clear();

            StringBuilder sb = new StringBuilder();
            KeyValuePair<ManagementMainMenu, List<ManagementMenu>>[] list = BuildMenus(ctl).ToArray();

            sb.Append("var MAINMENUS=");
            sb.Append(BuildMainMenus(list));
            sb.AppendLine(";");
            sb.Append("var MENUS=");
            sb.Append(BuildMenus(list));
            sb.AppendLine(";");
            sb.Append("var QUICKMENUS=");
            sb.Append(BuildQuickMenus(ctl));
            sb.AppendLine(";");
            sb.Append(@"var SELECTED_MENU = null;
function formatUrl(url) {
    return Root_Url + url + Url_Ext;
}
function loadMenus(index) {
    $('#nav').empty();
    var id = 'navlist_' + index;
    var html = '<div id=""' + id + '"" class=""navitem"">';
    for (var i = 0; i < MENUS[index].length; ++i) {
        html += '<div><span>' + MENUS[index][i].name + '</span></div><div><ul>';
        for (var j = 0; j < MENUS[index][i].sub.length; ++j) {
            html += '<li><a id=""menu_' + index + '_' + i + '_' + j + '"" href=""' + formatUrl(MENUS[index][i].sub[j].url) + '"">' + MENUS[index][i].sub[j].name + '</a></li>';
        }
        html += '</ul></div>';
    }
    html += '</div>';
    item = $(html);
    $('#nav').append(item);
    $('#' + id + ' a').click(function () {
        if (SELECTED_MENU == null || SELECTED_MENU != $(this).attr('id')) {
            $('#cnav').html($(this).html());
            if (SELECTED_MENU != null)
                $('#' + SELECTED_MENU).removeClass('selected');
            SELECTED_MENU = $(this).attr('id');
            $(this).addClass('selected');
            loadUrl($(this).attr('href'));
        }
        return false;
    });
    $('#' + id).jqxNavigationBar({ width: 160, height: '100%', expandMode: 'multiple', expandedIndexes: [0] });
    if (SELECTED_MENU != null)
        $('#' + SELECTED_MENU).addClass('selected');
}");
            return sb.ToString();
        }
    }

    public sealed class MenuList : ManagementMenus
    {
        protected override void InitMenus()
        {
            AddMenu(0, 0, "系统设置")
                .AddSubMenu("站点设置", "/system/config")
                .AddSubMenu("站点变量", "/system/site", "static/images/ico01.png")
                .AddSubMenu("登录设置", "/system/passport")
                .AddSubMenu("缓存管理", "/system/cache")
                .AddSubMenu("图形验证码设置", "/system/captcha")
                .AddSubMenu("短信验证码设置", "/system/smscaptcha")
                .AddSubMenu("文件系统设置", "/system/filesystem")
                .AddSubMenu("邮件设置", "/system/email")
                .AddSubMenu("友情链接设置", "/system/friendlink")
                .AddSubMenu("Robots", "/system/robots")
                .AddSubMenu("Sitemap", "/system/sitemap");

            AddMenu(0, 98, "权限管理")
                .AddSubMenu("管理员组", "/admin/role")
                .AddSubMenu("管理员管理", "/admin/admin");

            AddMenu(0, 99, "日志管理")
                .AddSubMenu("日志管理", "/log");

            //AddMenu(2, 0, "栏目管理")
            //   .AddSubMenu("栏目管理", "/channel");

            //AddMenu(3, 0, "模板管理")
            //   .AddSubMenu("模板管理", "/template");

            //AddMenu(4, 0, "用户管理")
            //   .AddSubMenu("用户管理", "/user");

            //AddMenu(4, 1, "用户组管理")
            //   .AddSubMenu("用户组管理", "/userrole");

            AddMenu(5, 0, "数据库管理")
               .AddSubMenu("执行语句", "/database/excute")
               .AddSubMenu("收缩数据库", "/database/vacuum")
               .AddSubMenu("备份数据库", "/database/backup")
               .AddSubMenu("还原数据库", "/database/reduce");

            //AddMenu(5, 1, "虚拟表管理")
            //   .AddSubMenu("虚拟表管理", "/virtualdata");
        }
    }
    internal sealed class MemuConfig
    {
        private Dictionary<int, Dictionary<int, Dictionary<int, bool>>> _menus;

        public MemuConfig()
        {
            _menus = new Dictionary<int, Dictionary<int, Dictionary<int, bool>>>();
            LoadConfig();
        }

        private unsafe void LoadConfig()
        {
            try
            {
                string file = HttpContext.Current.Server.MapPath("~/mm.config");
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = new StreamReader(fs, Application.Current.Settings.FileEncoding))
                    {
                        char* tmp;
                        string line;
                        string method;
                        string arguments;
                        string[] array;
                        while ((line = reader.ReadLine()) != null)
                        {
                            try
                            {
                                method = null;
                                arguments = null;
                                fixed (char* p = line)
                                {
                                    char* begin = p;
                                    char* end = p + line.Length;
                                    tmp = begin;
                                    for (char* cur = begin; cur != end; ++cur)
                                    {
                                        switch (*cur)
                                        {
                                            case '(':
                                                method = new string(tmp, 0, (int)(cur - tmp));
                                                tmp = cur + 1;
                                                break;
                                            case ')':
                                                arguments = new string(tmp, 0, (int)(cur - tmp));
                                                break;
                                        }
                                    }
                                }
                                if (method != null && arguments != null)
                                {
                                    method = method.Trim().ToUpper();
                                    if ("HM".Equals(method))
                                    {
                                        array = arguments.Trim().Split(',');
                                        if (array.Length == 1)
                                            HideMenu(int.Parse(array[0].Trim()));
                                        else if (array.Length == 2)
                                            HideMenu(int.Parse(array[0].Trim()), int.Parse(array[1].Trim()));
                                        else if (array.Length >= 3)
                                            HideMenu(int.Parse(array[0].Trim()), int.Parse(array[1].Trim()), int.Parse(array[2].Trim()));
                                    }
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        public bool IsHideMainMenu(int main)
        {
            Dictionary<int, Dictionary<int, bool>> dict;
            return IsHideMainMenu(main, out dict);
        }
        private bool IsHideMainMenu(int main, out Dictionary<int, Dictionary<int, bool>> dict)
        {
            if (_menus.TryGetValue(main, out dict))
                return dict == null;
            return false;
        }

        public bool IsHideMenu(int main, int menu)
        {
            Dictionary<int, Dictionary<int, bool>> dict;
            return IsHideMenu(main, menu, out dict);
        }
        private bool IsHideMenu(int main, int menu, out Dictionary<int, Dictionary<int, bool>> dict)
        {
            Dictionary<int, bool> list;
            if (_menus.TryGetValue(main, out dict))
            {
                if (dict == null)
                    return true;
                if (dict.TryGetValue(menu, out list))
                    return list == null;
            }
            return false;
        }

        public bool IsHideSubMenu(int main, int menu, int sub)
        {
            Dictionary<int, bool> list;
            Dictionary<int, Dictionary<int, bool>> dict;
            return IsHideSubMenu(main, menu, sub, out dict, out list);
        }
        private bool IsHideSubMenu(int main, int menu, int sub, out Dictionary<int, Dictionary<int, bool>> dict, out Dictionary<int, bool> list)
        {
            bool value;
            list = null;
            if (_menus.TryGetValue(main, out dict))
            {
                if (dict == null)
                    return true;
                if (dict.TryGetValue(menu, out list))
                {
                    if (list == null)
                        return true;
                    if (list.TryGetValue(sub, out value))
                        return true;
                }
            }
            return false;
        }
        
        private void HideMenu(int main)
        {
            _menus[main] = null;
        }
        private void HideMenu(int main, int menu)
        {
            Dictionary<int, Dictionary<int, bool>> dict;
            if (!IsHideMainMenu(main, out dict))
            {
                if (dict == null)
                {
                    dict = new Dictionary<int, Dictionary<int, bool>>();
                    _menus[main] = dict;
                }
                dict[menu] = null;
            }
        }
        private void HideMenu(int main, int menu, int sub)
        {
            Dictionary<int, bool> list;
            Dictionary<int, Dictionary<int, bool>> dict;
            if (!IsHideSubMenu(main, menu, sub, out dict, out list))
            {
                if (dict == null)
                {
                    dict = new Dictionary<int, Dictionary<int, bool>>();
                    _menus[main] = dict;
                }
                if (list == null)
                {
                    list = new Dictionary<int, bool>();
                    dict[menu] = list;
                }
                list[sub] = true;
            }
        }
    }
}
