using System.Collections.Generic;

namespace HttpServer.Server
{
    public sealed class HttpRequestHeaders : HttpHeaders
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

        public HttpRequestHeaders()
            : base(new Dictionary<string, string>())
        { }

        public HttpRequestHeaders(Dictionary<string, string> headers)
            : base(headers)
        { }

        public HttpRequestHeaders(HttpHeaders httpHeaders)
            : base(httpHeaders)
        { }
    }
}
