using System;
using Cnaws.Data;
using Cnaws.Templates;

namespace Cnaws.Web.Modules
{
#if (DEBUG)
    [Serializable]
    public sealed class DataTestA : IdentityModule
    {
        public Guid Guid;
        [DataColumn(64)]
        public string String;
        public bool Boolean;
        public int Int32;
        public long Int64;
        public double Double;
        public DateTime DateTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        public Money Money;

        protected override void OnInstallBefor(DataSource ds)
        {
            DropIndex(ds, "Int32");
        }
        protected override void OnInstallAfter(DataSource ds)
        {
            CreateIndex(ds, "Int32", "Int32");
        }

        public static bool operator ==(DataTestA left, DataTestA right)
        {
            if (object.Equals(left, null) || object.Equals(right, null))
                return false;
            //if (left.Id != right.Id)
            //    return false;
            //if (left.Guid != right.Guid)
            //    return false;
            //if (left.String != right.String)
            //    return false;
            //if (left.Boolean != right.Boolean)
            //    return false;
            if (left.Int32 != right.Int32)
                return false;
            //if (left.Int64 != right.Int64)
            //    return false;
            //if (left.Double != right.Double)
            //    return false;
            //if (left.DateTime != right.DateTime)
            //    return false;
            return true;
        }
        public static bool operator !=(DataTestA left, DataTestA right)
        {
            return !(left == right);
        }
    }
    [Serializable]
    public sealed class DataTestB : NoIdentityModule
    {
        [DataColumn(true)]
        public int Id;
        public Guid Guid;
        public string String;
        public bool Boolean;
        public int Int32;
        public long Int64;
        public double Double;
        public DateTime DateTime = (DateTime)Types.GetDefaultValue(TType<DateTime>.Type);
        public Money Money;

        //public static bool operator ==(DataTestB left, DataTestB right)
        //{
        //    if (object.Equals(left, null) || object.Equals(right, null))
        //        return false;
        //    if (left.Id != right.Id)
        //        return false;
        //    if (left.Guid != right.Guid)
        //        return false;
        //    if (left.String != right.String)
        //        return false;
        //    if (left.Boolean != right.Boolean)
        //        return false;
        //    if (left.Int32 != right.Int32)
        //        return false;
        //    if (left.Int64 != right.Int64)
        //        return false;
        //    if (left.Double != right.Double)
        //        return false;
        //    if (left.DateTime != right.DateTime)
        //        return false;
        //    return true;
        //}
        //public static bool operator !=(DataTestB left, DataTestB right)
        //{
        //    return !(left == right);
        //}
    }
#endif
}
