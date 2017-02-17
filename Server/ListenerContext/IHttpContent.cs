using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Server
{
    public interface IHttpContent
    {
        long Length { get; }
        ContentType ContentType { get; set; }
        Encoding Charset { get; set; }
        ContentRange ContentRange { get; set; }
        Task CopyToAsync(Stream outputStream);
    }
}