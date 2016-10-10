using System;
using System.IO;
using System.Web;
using System.Text;
using System.Collections.Generic;

namespace Cnaws.Management
{
    public sealed class ManagementRight
    {
        private string _name;
        private string _right;

        public ManagementRight(string name, string right)
        {
            _name = name;
            _right = right;
        }

        public string Name
        {
            get { return _name; }
        }
        public string Right
        {
            get { return _right; }
        }
    }

    public abstract class ManagementRights
    {
        private List<ManagementRight> _rights;

        public ManagementRights()
        {
            _rights = new List<ManagementRight>();
            InitRights();
        }

        public IList<ManagementRight> Rights
        {
            get { return _rights; }
        }

        protected abstract void InitRights();

        protected void AddRight(string name, string right)
        {
            Rights.Add(new ManagementRight(name, right));
        }

        internal static IList<ManagementRight> GetAll(HttpContext context)
        {
            int index;
            Type type;
            string asm;
            ManagementRights instance;
            List<ManagementRight> rights = new List<ManagementRight>();
            DirectoryInfo dir = new DirectoryInfo(context.Server.MapPath("~/Bin"));
            foreach (FileInfo file in dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
            {
                index = file.Name.LastIndexOf('.');
                asm = file.Name.Substring(0, index);
                type = Type.GetType(string.Concat(asm, asm.EndsWith(".Management") ? string.Empty : ".Management", ".RightList,", asm), false, true);
                if (type != null)
                {
                    instance = Activator.CreateInstance(type) as ManagementRights;
                    if (instance != null && instance.Rights != null && instance.Rights.Count > 0)
                        rights.AddRange(instance.Rights);
                }
            }
            return rights;
        }
    }

    public sealed class RightList : ManagementRights
    {
        protected override void InitRights()
        {
            AddRight("系统设置", "management.system");
            AddRight("权限管理", "management.admin");
            AddRight("日志管理", "management.log");
            //AddRight("用户管理", "management.user");
        }
    }
}
