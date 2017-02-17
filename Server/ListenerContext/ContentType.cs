namespace HttpServer.Server
{
    public class ContentType
    {
        public static readonly ContentType OctetStream = new ContentType("application/octet-stream");
        public static readonly ContentType PlainText = new ContentType("text/plain");
        public static readonly ContentType Xml = new ContentType("text/xml");
        public static readonly ContentType ApplicationXml = new ContentType("application/xml");
        public static readonly ContentType Html = new ContentType("text/html");
        public static readonly ContentType Json = new ContentType("application/json");
        public static readonly ContentType Csv = new ContentType("text/csv");
        public static readonly ContentType Pdf = new ContentType("application/pdf");
        public static readonly ContentType Zip = new ContentType("application/zip");
        public static readonly ContentType Gzip = new ContentType("application/gzip");

        public static ContentType Parse(string contentTypeHeader)
        {
            var separatorIndex = contentTypeHeader.IndexOf(';');
            return separatorIndex < 0
                ? new ContentType(contentTypeHeader)
                : new ContentType(contentTypeHeader.Substring(0, separatorIndex));
        }

        public readonly string Type;


        public ContentType(string type)
        {
            Preconditions.EnsureNotNull(type, "type");
            Preconditions.EnsureCondition(!type.Contains(";"), "type", "Types with charset are not allowed. Use Charset property of IHttpContent.");
            Type = type;
        }
        
        public override bool Equals(object obj)
        {
            var contentType = obj as ContentType;
            if (contentType == null)
            {
                return false;
            }
            return contentType.Type.Equals(Type);
        }

        public override int GetHashCode()
        {
            return Type?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Type;
        }
    }
}