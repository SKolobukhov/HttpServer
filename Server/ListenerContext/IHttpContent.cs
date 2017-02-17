using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Server
{
    public interface IHttpContent
    {
        long Length { get; }
        Encoding Charset { get; }
        ContentType ContentType { get; }
        ContentRange ContentRange { get; }

        Task CopyToAsync(Stream outputStream);
    }
}