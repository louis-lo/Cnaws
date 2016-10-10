using System;
using Cnaws.Web;
using M = Cnaws.Management.Modules;
using Cnaws.Data;

namespace Cnaws.Management
{
    internal sealed class ManagementPassport : PassportProvider
    {
        private sealed class ManagementPrincipal : PassportPrincipal
        {
            public ManagementPrincipal(PassportIdentity identity)
                : base(identity)
            {
            }

            public override bool IsInRole(string role)
            {
                return false;
            }

            public override bool HasRight(string right)
            {
                if (Identity != null && Identity.IsAuthenticated && Identity.IsAdmin)
                {
                    IDataController ctl = Application.Current.Controller as IDataController;
                    if (ctl != null && string.Equals(ctl.DataProvider, PassportAuthentication.DataProvider))
                    {
                        if (M.Admin.HasRight(ctl.DataSource, (int)Identity.RoleId, right))
                            return true;
                    }
                    else
                    {
                        using (DataSource ds = new DataSource(PassportAuthentication.DataProvider))
                        {
                            if (M.Admin.HasRight(ds, (int)Identity.RoleId, right))
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        public ManagementPassport()
        {
        }

        protected internal override PassportPrincipal CreatePrincipal(PassportIdentity identity)
        {
            return new ManagementPrincipal(identity);
        }
    }
}
