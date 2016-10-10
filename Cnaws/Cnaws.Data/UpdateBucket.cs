using System;

namespace Cnaws.Data
{
    internal sealed class UpdateBucket
    {
        public string Sets;
        public string Wheres;
        public DataParameter[] Parameters;

        public UpdateBucket(string sets, string wheres, DataParameter[] parameters)
        {
            Sets = sets;
            Wheres = wheres;
            Parameters = parameters;
        }
    }
}
