using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Server
{
    public class ByteArrayContent : IHttpContent
    {
        public static readonly ByteArrayContent Empty = new ByteArrayContent(new byte[0]);

        public byte[] Buffer => buffer;
        public int Offset => offset;
        public long Length => length;
        public Encoding Charset { get; protected set; }
        public ContentType ContentType { get; protected set; }
        public ContentRange ContentRange { get; protected set; }


        protected readonly byte[] buffer;
        protected readonly int offset;
        protected readonly int length;

        
        public ByteArrayContent(ArraySegment<byte> segment)
            : this(segment.Array, segment.Offset, segment.Count)
        { }

        public ByteArrayContent(byte[] buffer)
            : this(buffer, 0, buffer.Length)
        { }

        public ByteArrayContent(byte[] buffer, int offset)
            : this(buffer, offset, buffer.Length)
        { }

        public ByteArrayContent(byte[] buffer, int offset, int length)
        {
            Preconditions.EnsureNotNull(buffer, "buffer");
            Preconditions.EnsureArgumentRange(offset >= 0 && offset <= buffer.Length, "offset", "Incorrect offset = {0}. Buffer length = {1}.", offset, buffer.Length);
            Preconditions.EnsureArgumentRange(length >= 0 && offset + length <= buffer.Length, "length", "Incorrect length = {0}. Offset = {1}. Buffer length = {2}.", length, offset, buffer.Length);
            this.buffer = buffer;
            this.offset = offset;
            this.length = length;
        }


        public Task CopyToAsync(Stream outputStream)
        {
            return outputStream.WriteAsync(buffer, offset, length);
        }
    }
}