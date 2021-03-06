﻿using System.Net;
using System.Text;

namespace HttpServer.Server
{
    public sealed class HttpServerResponse
    {
        public readonly IHttpContent Body;
        public readonly HttpStatusCode Code;
        public readonly HttpResponseHeaders Headers;

        public bool HasHeaders => Headers != null && Headers.Count > 0;


        public HttpServerResponse(HttpStatusCode code)
            : this(code, null, null) { }

        public HttpServerResponse(HttpStatusCode code, IHttpContent body)
            : this(code, null, body) { }

        public HttpServerResponse(HttpStatusCode code, HttpResponseHeaders headers)
            : this(code, headers, null) { }

        public HttpServerResponse(HttpStatusCode code, HttpResponseHeaders headers, IHttpContent body)
        {
            Body = body;
            Code = code;
            Headers = headers;
        }
        
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"HTTP/{HttpVersion.Version11} {(int) Code} {Code}");
            if (Body != null)
            {
                builder.Append(Body);
            }
            return builder.ToString();
        }
    }
}