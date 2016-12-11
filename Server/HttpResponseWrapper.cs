using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using HttpServer.Common;
using log4net;

namespace HttpServer.Server
{
    public class HttpResponseWrapper
    {
        private readonly ILog log;
        private readonly HttpListenerContext context;

        private bool disposed;
        private volatile bool responseInitiated;

        public bool ResponseInitiated => responseInitiated;

        public HttpResponseWrapper(HttpListenerContext context, ILog log)
        {
            this.log = log;
            this.context = context;
        }

        public void Respond(HttpServerResponse response)
        {
            CheckResponseContent(response);
            responseInitiated = true;
            SetStatusCode(response);
            SetHeaders(response);
            WriteBody(response);
            CloseResponse();
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            try
            {
                context.Response.Close();
            }
            catch (Exception exception)
            {
                log.Error($"Error in closing response. Request = '{context.Request}'", exception);
                try
                {
                    context.Response.Abort();
                }
                catch (Exception anotherException)
                {
                    log.Error($"Error in aborting response. Request = '{context.Request}'", anotherException);
                }
            }
        }

        private void SetStatusCode(HttpServerResponse response)
        {
            context.Response.StatusCode = (int)response.Code;
        }

        private void SetHeaders(HttpServerResponse response)
        {
            if (!response.HasHeaders)
            {
                return;
            }
            context.Response.Headers = response.Headers;
        }

        private void WriteBody(HttpServerResponse response)
        {
            var body = response.Body;
            if (body != null)
            {
                context.Response.ContentLength64 = body.Length;
                var contentType = (body.ContentType ?? new ContentType()).ToString();
                if (body.Charset != null)
                {
                    contentType += "; charset=" + body.Charset.WebName;
                }
                context.Response.ContentType = contentType;
                if (body.ContentRange != null)
                {
                    context.Response.AddHeader(HttpHeaderNames.ContentRange, body.ContentRange.ToString());
                }
                try
                {
                    response.Body.CopyTo(context.Response.OutputStream);
                }
                catch (Exception exception)
                {
                    log.Error("Error in writing response body", exception);
                }
            }
            else context.Response.ContentLength64 = 0;
        }

        private void CloseResponse()
        {
            Dispose();
        }

        private void CheckResponseContent(HttpServerResponse response)
        {
            if (response.Body != null && context.Request.HttpMethod.Equals(HttpMethod.Head.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Attempt to respond with a PeekContent to request with {context.Request.HttpMethod} method.");
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (responseInitiated)
            {
                builder.AppendLine($"HTTP/{context.Response.ProtocolVersion} {context.Response.StatusCode} {context.Response.StatusDescription}");
                foreach (string header in context.Response.Headers)
                {
                    builder.AppendLine(header + ": " + context.Response.Headers[header]);
                }
            }
            else
            {
                builder.AppendLine("<Not initiated>");
            }
            return builder.ToString();
        }
    }
}