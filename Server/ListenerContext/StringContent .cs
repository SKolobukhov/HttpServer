using System.Text;

namespace HttpServer.Server
{
    public class StringContent : ByteArrayContent
    {
        public readonly string Text;

        public StringContent(string content, Encoding charset, ContentType contentType)
            : base(charset.GetBytes(content))
        {
            Text = content;
            ContentType = contentType;
            Charset = charset;
        }

        public StringContent(string content, Encoding charset)
            : this(content, charset, ContentType.PlainText) { }

        public StringContent(string content, ContentType contentType)
            : this(content, Encoding.UTF8, contentType) { }

        public StringContent(string content)
            : this(content, ContentType.PlainText) { }

        public override string ToString()
        {
            return Text;
        }
    }
}