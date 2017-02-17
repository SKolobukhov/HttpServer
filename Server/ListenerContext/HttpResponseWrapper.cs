using System;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
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

        public async Task RespondAsync(HttpServerResponse response)
        {
            CheckResponseContent(response);
            responseInitiated = true;
            context.Response.StatusCode = (int)response.Code;
            if (response.HasHeaders)
            {
                context.Response.Headers = response.Headers;
            }
            await WriteBodyAsync(response).ConfigureAwait(false);
            await Task.Run(() => CloseResponse()).ConfigureAwait(false);
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
                log.Error($"Error in closing response. Request: '{context.Request}'", exception);
                try
                {
                    context.Response.Abort();
                }
                catch (Exception anotherException)
                {
                    log.Error($"Error in aborting response. Request: '{context.Request}'", anotherException);
                }
            }
        }
        
        private async Task WriteBodyAsync(HttpServerResponse response)
        {
            var body = response.Body;
            if (body != null)
            {
                context.Response.ContentLength64 = body.Length;
                var contentType = (body.ContentType ?? ContentType.OctetStream).Type;
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
                    await response.Body.CopyToAsync(context.Response.OutputStream).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    log.Error("Error in writing response body", exception);
                }
            }
            else
            {
                context.Response.ContentLength64 = 0;
            }
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