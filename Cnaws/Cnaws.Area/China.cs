using System;
using System.IO;

namespace Cnaws.Area
{
    internal sealed class China : Country
    {
        protected override Stream GetDataStream()
        {
            return new MemoryStream(Properties.Resources.city);
        }
    }
}
