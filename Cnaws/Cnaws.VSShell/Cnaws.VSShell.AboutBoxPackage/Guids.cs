// Guids.cs
// MUST match guids.h
using System;

namespace Cnaws.VSShell.AboutBoxPackage
{
    static class GuidList
    {
        public const string guidAboutBoxPackagePkgString = "0076642c-6cf2-48ff-b3b5-11982b6e5cc2";
        public const string guidAboutBoxPackageCmdSetString = "c3422d99-e132-4f8a-bdd9-a23f567c9e78";

        public static readonly Guid guidAboutBoxPackageCmdSet = new Guid(guidAboutBoxPackageCmdSetString);
    };
}