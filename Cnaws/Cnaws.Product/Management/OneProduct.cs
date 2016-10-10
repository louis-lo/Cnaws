using System;
using System.Web;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.Product.Modules;

namespace Cnaws.Product.Management
{
    public sealed class OneProduct : ManagementController
    {
        private static readonly Version VERSION = new Version(1, 0, 0, 0);

        protected override Version Version
        {
            get { return VERSION; }
        }

        protected override string Namespace
        {
            get { return "Cnaws.Product"; }
        }

        public void Index()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (CheckPost("oneproduct"))
                        NotFound();
                }
            }
        }
        public void List(int categoryId, int page = 1)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.OneProduct.GetPage(DataSource, categoryId, page, 10));
            }
        }
        public void Number()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (CheckPost("oneproduct_number"))
                        NotFound();
                }
            }
        }
        public void NumberList(int state = -1, int page = 1)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.OneProductNumber.GetPage(DataSource, state, page, 10));
            }
        }

        public void AllCategory()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    IList<M.ProductCategory> list = M.ProductCategory.GetAll(DataSource, -1);
                    if (IsPost)
                        SetResult(list);
                    else
                        SetJavascript("allCategory", list);
                }
            }
        }

        public void Categories(int parentId)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.ProductCategory.GetAll(DataSource, parentId));
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
                        M.OneProduct value = DbTable.Load<M.OneProduct>(Request.Form);
                        value.Image = HttpUtility.UrlDecode(value.Image);
                        value.Content = HttpUtility.UrlDecode(value.Content);
                        value.Approved = false;
                        SetResult(value.Insert(DataSource), () =>
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
                        M.OneProduct value = DbTable.Load<M.OneProduct>(Request.Form);
                        SetResult(value.UpdateApproved(DataSource), () =>
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
                        M.OneProduct value = DbTable.Load<M.OneProduct>(Request.Form);
                        value.Image = HttpUtility.UrlDecode(value.Image);
                        value.Content = HttpUtility.UrlDecode(value.Content);
                        SetResult(value.Update(DataSource, ColumnMode.Exclude, "Approved"), () =>
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
        //public void Del()
        //{
        //    if (CheckAjax())
        //    {
        //        if (CheckRight())
        //        {
        //            if (IsPost)
        //            {
        //                SetResult(M.Product.Delete(DataSource, int.Parse(Request.Form["Id"])), () =>
        //                {
        //                    WritePostLog("DEL");
        //                });
        //            }
        //            else
        //            {
        //                NotFound();
        //            }
        //        }
        //    }
        //}
        public void Get(int id)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.OneProduct.GetById(DataSource, id));
            }
        }
        public void Parents(int id)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    List<int> list = new List<int>();
                    M.ProductCategory ac = M.ProductCategory.GetById(DataSource, id);
                    list.Add(ac.Id);
                    while (ac.ParentId != 0)
                    {
                        ac = M.ProductCategory.GetById(DataSource, ac.ParentId);
                        list.Insert(0, ac.Id);
                    }
                    SetResult(list.ToArray());
                }
            }
        }
    }
}
