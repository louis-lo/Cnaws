using System;

namespace Cnaws.Json
{
    public sealed class JsonException : Exception
    {
        private string _where;

        public JsonException()
        {
        }
        public JsonException(string message)
            : base(message)
        {
        }
        public JsonException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }
        public JsonException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public JsonException(string message, string where)
            : base(message)
        {
            _where = where;
        }

        public string Where
        {
            get { return _where; }
        }
    }
}
