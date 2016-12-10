using System.Collections.Generic;
using System.Linq;

namespace HttpServer.Common
{
    public abstract class HttpHeaders
    {
        protected readonly Dictionary<string, string> headers;

        public int Count => headers.Count;
        public string[] Keys => headers.Keys.ToArray();


        protected HttpHeaders(HttpHeaders httpHeaders)
            : this(new Dictionary<string, string>(httpHeaders?.headers ?? new Dictionary<string, string>()))
        { }

        protected HttpHeaders(Dictionary<string, string> headers)
        {
            this.headers = headers ?? new Dictionary<string, string>();
        }

        public override string ToString()
        {
            return headers.ToString();
        }
    }
}