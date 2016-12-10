using System.Collections.Generic;
using System.Linq;

namespace HttpServer.Common
{
    public abstract class HeadersCollection
    {
        protected readonly Dictionary<string, string> headers;

        public int Count => headers.Count;
        public string[] Keys => headers.Keys.ToArray();


        protected HeadersCollection(HeadersCollection headersCollection)
            : this(new Dictionary<string, string>(headersCollection?.headers ?? new Dictionary<string, string>()))
        { }

        protected HeadersCollection(Dictionary<string, string> headers)
        {
            this.headers = headers ?? new Dictionary<string, string>();
        }

        public override string ToString()
        {
            return headers.ToString();
        }
    }
}