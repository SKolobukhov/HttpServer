using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;

namespace HttpServer.Server
{
    public class HttpRequestWrapper
    {
        public readonly IPrincipal User;
        public readonly HttpListenerRequest Request;

        public HttpRequestWrapper(IPrincipal user, HttpListenerRequest request)
        {
            User = user;
            Request = request;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{Request.HttpMethod} {Request.RawUrl} HTTP/{HttpVersion.Version11}");
            builder.AppendLine();
            foreach (string header in Request.Headers)
            {
                builder.AppendLine(header + ": " + Request.Headers[header]);
            }
            var buffer = new char[1024];
            using (var reader = new StreamReader(Request.InputStream))
            using (var writer = new StringWriter(builder))
            {
                if (!reader.EndOfStream)
                {
                    builder.AppendLine();
                }
                while (!reader.EndOfStream)
                {
                    reader.Read(buffer, 0, buffer.Length);
                    writer.Write(buffer);
                }
            }
            return builder.ToString();
        }
    }
}