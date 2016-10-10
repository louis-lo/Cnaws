using System;

namespace Cnaws.Data
{
    internal sealed class DeleteBucket
    {
        public string Wheres;
        public DataParameter[] Parameters;

        public DeleteBucket(string wheres, DataParameter[] parameters)
        {
            Wheres = wheres;
            Parameters = parameters;
        }
    }
}
