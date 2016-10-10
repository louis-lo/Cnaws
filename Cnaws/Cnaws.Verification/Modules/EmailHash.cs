using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;
using Cnaws.ExtensionMethods;

namespace Cnaws.Verification.Modules
{
    [Serializable]
    public sealed class EmailHash : NoIdentityModule
    {
        [DataColumn(true, 256)]
        public string Email = null;
        [DataColumn(true)]
        public int Type = 0;
        public Guid Hash = Guid.Empty;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        protected override DataStatus OnInsertBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (!Utility.EmailRegularExpression.IsMatch(Email))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnUpdateBefor(DataSource ds, ColumnMode mode, ref DataColumn[] columns)
        {
            if (!Utility.EmailRegularExpression.IsMatch(Email))
                return DataStatus.Failed;
            return DataStatus.Success;
        }
        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }

        public static DataStatus Create(DataSource ds, string email, int type)
        {
            EmailHash hash = new EmailHash() { Email = email.ToLower(), Type = type, Hash = Guid.NewGuid(), CreationDate = DateTime.Now };
            if (ExecuteCount<EmailHash>(ds, P("Type", hash.Type) & P("Email", hash.Email)) > 0)
                return hash.Update(ds);
            return hash.Insert(ds);
        }
        public static bool Equals(DataSource ds, string email, int type, Guid hash)
        {
            if (string.IsNullOrEmpty(email) || hash == Guid.Empty)
                return false;
            EmailHash eh = ExecuteSingleRow<EmailHash>(ds, P("Type", type) & P("Email", email.ToLower()));
            if (eh == null)
                return false;
            if (hash != eh.Hash)
                return false;
            //if (eh.CreationDate.AddMinutes(1) < DateTime.Now)
            //    return false;
            eh.Hash = Guid.Empty;
            eh.CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
            eh.Update(ds);
            return true;
        }
    }
}
