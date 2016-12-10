using System.Collections.Generic;

namespace HttpServer.Common
{
    public sealed class WritableHeadersCollection : HeadersCollection
    {
        public string this[string headerName]
        {
            get
            {
                if (!headers.ContainsKey(headerName))
                {
                    return null;
                }
                return headers[headerName];
            }
            set { headers[headerName] = value; }
        }

        public WritableHeadersCollection()
            : base(new Dictionary<string, string>())
        { }

        public WritableHeadersCollection(Dictionary<string, string> headers)
            : base(headers)
        { }

        public WritableHeadersCollection(HeadersCollection headersCollection)
            : base(headersCollection)
        { }
    }
}
