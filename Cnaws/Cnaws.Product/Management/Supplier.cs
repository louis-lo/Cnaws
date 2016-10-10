using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Management;
using M = Cnaws.Product.Modules;
using System.Collections.Generic;

namespace Cnaws.Product.Management
{
    public sealed class Supplier : ManagementController
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

        public void Index()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (CheckPost("supplier"))
                        NotFound();
                }
            }
        }

        public void List(int state, int page = 1)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                    SetResult(M.Supplier.GetPage(DataSource, state, page, 10));
            }
        }

        public void Info(long userId)
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (CheckPost("supplier", () =>
                     {
                         this["Supplier"] = M.Supplier.GetById(DataSource, userId);
                     }))
                    {
                        NotFound();
                    }
                }
            }
        }

        public void Approved()
        {
            if (CheckAjax())
            {
                if (CheckRight())
                {
                    if (IsPost)
                    {
                        M.Supplier value = DbTable.Load<M.Supplier>(Request.Form);
                        SetResult(value.Approved(DataSource), () =>
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
    }
}
