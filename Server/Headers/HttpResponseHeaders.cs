using System.Collections.Generic;

namespace HttpServer.Server.Headers
{
    public sealed class HttpResponseHeaders : HttpHeaders
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


        public HttpResponseHeaders()
            : base(new Dictionary<string, string>())
        { }

        public HttpResponseHeaders(Dictionary<string, string> headers)
            : base(headers)
        { }

        public HttpResponseHeaders(HttpHeaders httpHeaders)
            : base(httpHeaders)
        { }
    }
}