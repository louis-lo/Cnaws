using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.FriendLink.Modules;

namespace Cnaws.FriendLink.Management
{
    public sealed class FriendLink : ManagementController
    {
        private static readonly Version VERSION = new Version(1, 0, 0, 0);

        protected override Version Version
        {
            get { return VERSION; }
        }

        protected override string Namespace
        {
            get { return "Cnaws.FriendLink"; }
        }

        public void Index(int approved = 1)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (CheckPost("friendlink", () =>
                     {
                         this["Approved"] = approved;
                     }))
                    {
                        NotFound();
                    }
                }
            }
        }
        public void List(int approved = 1, int page = 1)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.FriendLink.GetPage(DataSource, approved == 1, page, 10));
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
                        M.FriendLink link = new M.FriendLink()
                        {
                            Name = Request["Name"],
                            Url = Request["Url"],
                            Image = Request["Image"],
                            SortNum = int.Parse(Request["SortNum"]),
                            Approved = Types.GetBooleanFromString(Request["Approved"])
                        };
                        SetResult(link.Insert(DataSource), () =>
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
                        M.FriendLink link = new M.FriendLink()
                        {
                            Id = int.Parse(Request["Id"]),
                            Name = Request["Name"],
                            Url = Request["Url"],
                            Image = Request["Image"],
                            SortNum = int.Parse(Request["SortNum"]),
                            Approved = Types.GetBooleanFromString(Request["Approved"])
                        };
                        SetResult(link.Update(DataSource), () =>
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
                        M.FriendLink link = new M.FriendLink()
                        {
                            Id = int.Parse(Request["Id"])
                        };
                        SetResult(link.Delete(DataSource), () =>
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
