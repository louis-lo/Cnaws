using System;
using Cnaws.Web;
using Cnaws.Data;

namespace Cnaws.Ad.Modules
{
    public enum AdType
    {
        Image = 0,
        Script = 1
    }

    [Serializable]
    public sealed class Ad : IdentityModule
    {
        [DataColumn(16)]
        public string Name = null;
        public AdType Type = AdType.Image;
        [DataColumn(4000)]
        public string Content = null;
        [DataColumn(256)]
        public string Url = null;
        [DataColumn(16)]
        public string Style = null;
        public int Width = 0;
        public int Height = 0;


    }
}
