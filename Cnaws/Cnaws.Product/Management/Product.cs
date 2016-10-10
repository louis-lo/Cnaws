using System;
using System.Web;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.Product.Modules;

namespace Cnaws.Product.Management
{
    public sealed class Product : ManagementController
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
                    if (CheckPost("product"))
                        NotFound();
                }
            }
        }
        public void List(int categoryId, int page = 1, string q = "")
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.Product.GetPage(DataSource, categoryId, q, page, 10));
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

        //public void Add()
        //{
        //    if (CheckAjax())
        //    {
        //        if (CheckRight())
        //        {
        //            if (IsPost)
        //            {
        //                SetResult(M.Product.Insert(DataSource,
        //                    Request.Form["Title"],
        //                    Request.Form["Image"],
        //                    Request.Form["Content"],
        //                    int.Parse(Request.Form["Category"]),
        //                    DateTime.Parse(Request.Form["CreationDate"]),
        //                    Request.Form["Keywords"],
        //                    Request.Form["Description"],
        //                    "on".Equals(Request.Form["Visibility"]),
        //                    "on".Equals(Request.Form["Top"]),
        //                    int.Parse(Request.Form["Style"]),
        //                    Request.Form["Color"],
        //                    Request.Form["Author"],
        //                    Request.Form["Referer"]), () =>
        //                    {
        //                        WritePostLog("ADD");
        //                    });
        //            }
        //            else
        //            {
        //                NotFound();
        //            }
        //        }
        //    }
        //}
        public void Update()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (IsPost)
                    {
                        M.Product value = DbTable.Load<M.Product>(Request.Form);
                        SetResult(value.Update(DataSource, ColumnMode.Include, "CategoryId", "Recommend", "CategoryBest", "SortNum"), () =>
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
        public void State()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (IsPost)
                    {
                        M.Product value = DbTable.Load<M.Product>(Request.Form);
                        if (value.State == M.ProductState.Sale)
                            value.SaleTime = DateTime.Now;
                        SetResult(value.Update(DataSource, ColumnMode.Include, "State", "SaleTime"), () =>
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
        public void DState()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (IsPost)
                    {
                        M.Product value = DbTable.Load<M.Product>(Request.Form);
                        SetResult(value.Update(DataSource, ColumnMode.Include, "DiscountState"), () =>
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
        //public void Mod()
        //{
        //    if (CheckAjax())
        //    {
        //        if (CheckRight())
        //        {
        //            if (IsPost)
        //            {
        //                M.Product value = DbTable.Load<M.Product>(Request.Form);
        //                SetResult(value.Update(DataSource), () =>
        //                {
        //                    WritePostLog("MOD");
        //                });
        //            }
        //            else
        //            {
        //                NotFound();
        //            }
        //        }
        //    }
        //}
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
                    SetResult(M.Product.GetById(DataSource, id));
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
