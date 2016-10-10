using System;
using System.Web;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.Banner.Modules;

namespace Cnaws.Banner.Management
{
    public sealed class Banner : ManagementController
    {
        private static readonly Version VERSION = new Version(1, 0, 0, 0);

        protected override Version Version
        {
            get { return VERSION; }
        }

        protected override string Namespace
        {
            get { return "Cnaws.Banner"; }
        }

        public void Index()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (CheckPost("banner"))
                        NotFound();
                }
            }
        }
        public void List(int page = 1)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.Banner.GetPage(DataSource, page, 10));
            }
        }

        public void Add()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (IsPost)
                    {
                        SetResult(DbTable.Load<M.Banner>(Request.Form).Insert(DataSource), () =>
                        {
                            WritePostLog("ADD");
                        });
                    }
                    else
                    {
                        NotFound();
                    }
                }
            }
        }
        public void Mod()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (IsPost)
                    {
                        SetResult(DbTable.Load<M.Banner>(Request.Form).Update(DataSource), () =>
                        {
                            WritePostLog("MOD");
                        });
                    }
                    else
                    {
                        NotFound();
                    }
                }
            }
        }
        public void Del()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (IsPost)
                    {
                        SetResult(DbTable.Load<M.Banner>(Request.Form).Delete(DataSource), () =>
                        {
                            WritePostLog("DEL");
                        });
                    }
                    else
                    {
                        NotFound();
                    }
                }
            }
        }
    }
}
