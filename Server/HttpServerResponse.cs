﻿using System.Net;
using System.Text;
using HttpServer.Common;

namespace HttpServer.Server
{
    public sealed class HttpServerResponse
    {
        public readonly HttpStatusCode Code;
        private readonly HttpResponseHeaders headers;
        public readonly IHttpContent Body;

        public bool HasHeaders => headers != null && headers.Count > 0;
        public HttpResponseHeaders Headers => headers ?? new HttpResponseHeaders();


        public HttpServerResponse(HttpStatusCode code)
            : this(code, null, null) { }

        public HttpServerResponse(HttpStatusCode code, IHttpContent body)
            : this(code, null, body) { }

        public HttpServerResponse(HttpStatusCode code, HttpResponseHeaders headers)
            : this(code, headers, null) { }

        public HttpServerResponse(HttpStatusCode code, HttpResponseHeaders headers, IHttpContent body)
        {
            Code = code;
            this.headers = headers;
            Body = body;
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