using System.Collections.Generic;

namespace HttpServer.Common
{
    public sealed class ReadonlyHeadersCollection : HeadersCollection
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
        }


        public ReadonlyHeadersCollection()
            : base(new Dictionary<string, string>())
        { }

        public ReadonlyHeadersCollection(Dictionary<string, string> headers)
            : base(headers)
        { }

        public ReadonlyHeadersCollection(HeadersCollection headersCollection)
            : base(headersCollection)
        { }
    }
}