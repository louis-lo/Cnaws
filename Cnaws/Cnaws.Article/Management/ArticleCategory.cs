using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.Article.Modules;

namespace Cnaws.Article.Management
{
    public sealed class ArticleCategory : ManagementController
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
                    if (CheckPost("articlecategory"))
                        NotFound();
                }
            }
        }
        public void AllCategory()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    IList<M.ArticleCategory> list = M.ArticleCategory.GetAll(DataSource, -1);
                    if (IsPost)
                        SetResult(list);
                    else
                        SetJavascript("allCategory", list);
                }
            }
        }
        public void List(int parentId, int page = 1)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.ArticleCategory.GetPage(DataSource, parentId, page, 10));
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
                        SetResult(M.ArticleCategory.Insert(DataSource, Request["Name"], int.Parse(Request["ParentId"]), int.Parse(Request["SortNum"])), () =>
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
                        SetResult(M.ArticleCategory.Update(DataSource, int.Parse(Request["Id"]), Request["Name"], int.Parse(Request["SortNum"])), () =>
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
                        SetResult(M.ArticleCategory.Delete(DataSource, int.Parse(Request["Id"])), () =>
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
        public void All(int parentId)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.ArticleCategory.GetAll(DataSource, parentId));
            }
        }
    }
}
