using System;
using System.Collections.Generic;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.Product.Modules;

namespace Cnaws.Product.Management
{
    public sealed class ProductAttribute : ManagementController
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

        public void Index(int id = 0)
        {
            if (CheckRight())
            {
                if (CheckPost("productattribute", () =>
                {
                    this["Id"] = id;
                    this["Parents"] = M.ProductCategory.GetAllParentsById(DataSource, id);
                    this["AllCategory"] = M.ProductCategory.GetAll(DataSource, -1);
                }))
                    NotFound();
            }
        }
        //public void AllCategory()
        //{
        //    if (CheckAjax())
        //    {
        //        if (CheckRight())
        //        {
        //            IList<M.ProductCategory> list = M.ProductCategory.GetAll(DataSource, -1);
        //            if (IsPost)
        //                SetResult(list);
        //            else
        //                SetJavascript("allCategory", list);
        //        }
        //    }
        //}
        public void List(int categoryId = -1, int page = 1)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.ProductAttribute.GetPage(DataSource, categoryId, page, 10));
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
                        M.ProductAttribute attr = new M.ProductAttribute()
                        {
                            Name = Request["Name"],
                            CategoryId = int.Parse(Request["CategoryId"]),
                            SortNum = int.Parse(Request["SortNum"])
                        };
                        SetResult(attr.Insert(DataSource), () =>
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
                        M.ProductAttribute attr = new M.ProductAttribute()
                        {
                            Id = int.Parse(Request["Id"]),
                            Name = Request["Name"],
                            SortNum = int.Parse(Request["SortNum"])
                        };
                        SetResult(attr.Update(DataSource), () =>
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
                        M.ProductAttribute attr = new M.ProductAttribute()
                        {
                            Id = int.Parse(Request["Id"])
                        };
                        SetResult(attr.Delete(DataSource), () =>
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
