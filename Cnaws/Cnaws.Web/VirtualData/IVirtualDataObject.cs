using System;

namespace Cnaws.Web.VirtualData
{
    public interface IVirtualDataObject
    {
        object this[int index] { get; }
        object this[string name] { get; }
    }
}
