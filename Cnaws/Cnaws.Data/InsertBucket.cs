using System;

namespace Cnaws.Data
{
    internal sealed class InsertBucket
    {
        public string Names;
        public string Values;
        public DataParameter[] Parameters;
        public string Id;

        public InsertBucket(string names, string values, DataParameter[] parameters, string id)
        {
            Names = names;
            Values = values;
            Parameters = parameters;
            Id = id;
        }
    }
}
