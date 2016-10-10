using System;
using Cnaws.Web;
using Cnaws.Data;
using Cnaws.Templates;

namespace Cnaws.Passport.Modules
{
    [Serializable]
    public sealed class ExperienceRecord : LongIdentityModule
    {
        public long MemberId = 0L;
        [DataColumn(128)]
        public string Title = null;
        public int Type = 0;
        [DataColumn(64)]
        public string TargetId = null;
        public int Value = 0;
        public DateTime CreationDate = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "MemberId");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "MemberId", "MemberId");
        }

        protected override DataStatus OnDeleteBefor(DataSource ds, ref DataColumn[] columns)
        {
            return DataStatus.Failed;
        }
    }
}
