using System;
using System.Web;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.Article.Modules;

namespace Cnaws.Article.Management
{
    public sealed class Article : ManagementController
    {
        private static readonly Version VERSION = new Version(1, 0, 0, 0);

        protected override Version Version
        {
            get { return VERSION; }
        }

        protected override string Namespace
        {
            get { return "Cnaws.Article"; }
        }

        public void Index()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (CheckPost("article"))
                        NotFound();
                }
            }
        }
        public void List(int categoryId, int page = 1)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.Article.GetPage(DataSource, categoryId, page, 10));
            }
        }

        public void Categories(int parentId)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.ArticleCategory.GetAll(DataSource, parentId));
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
                        SetResult(M.Article.Insert(DataSource,
                            Request.Form["Title"],
                            Request.Form["Image"],
                            HttpUtility.UrlDecode(Request.Form["Content"]),
                            int.Parse(Request.Form["Category"]),
                            DateTime.Parse(Request.Form["CreationDate"]),
                            Request.Form["Keywords"],
                            Request.Form["Description"],
                            "on".Equals(Request.Form["Visibility"]),
                            "on".Equals(Request.Form["Top"]),
                            int.Parse(Request.Form["Style"]),
                            Request.Form["Color"],
                            Request.Form["Author"],
                            Request.Form["Referer"]), () =>
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
        public void Update()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (IsPost)
                    {
                        SetResult(M.Article.Update(DataSource,
                            int.Parse(Request.Form["Id"]),
                            Request.Form["Title"],
                            "true".Equals(Request.Form["Visibility"]),
                            "true".Equals(Request.Form["Top"])), () =>
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
        public void Mod()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (IsPost)
                    {
                        SetResult(M.Article.Update(DataSource,
                            int.Parse(Request.Form["Id"]),
                            Request.Form["Title"],
                            Request.Form["Image"],
                            HttpUtility.UrlDecode(Request.Form["Content"]),
                            int.Parse(Request.Form["Category"]),
                            DateTime.Parse(Request.Form["CreationDate"]),
                            Request.Form["Keywords"],
                            Request.Form["Description"],
                            "on".Equals(Request.Form["Visibility"]),
                            "on".Equals(Request.Form["Top"]),
                            int.Parse(Request.Form["Style"]),
                            Request.Form["Color"],
                            Request.Form["Author"],
                            Request.Form["Referer"]), () =>
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
                        SetResult(M.Article.Delete(DataSource, int.Parse(Request.Form["Id"])), () =>
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
        public void Get(int id)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.Article.GetById(DataSource, id));
            }
        }
        public void Parents(int id)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    List<int> list = new List<int>();
                    M.ArticleCategory ac = M.ArticleCategory.GetById(DataSource, id);
                    list.Add(ac.Id);
                    while (ac.ParentId != 0)
                    {
                        ac = M.ArticleCategory.GetById(DataSource, ac.ParentId);
                        list.Insert(0, ac.Id);
                    }
                    SetResult(list.ToArray());
                }
            }
        }
    }
}
