using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace HttpServer.Server
{
    public abstract class HttpHeaders: IEnumerable<KeyValuePair<string, string>>
    {
        protected readonly Dictionary<string, string> headers;

        public int Count => headers.Count;
        public string[] Keys => headers.Keys.ToArray();


        protected HttpHeaders(HttpHeaders httpHeaders)
            : this(new Dictionary<string, string>(httpHeaders.headers))
        { }

        protected HttpHeaders(Dictionary<string, string> headers)
        {
            this.headers = headers ?? new Dictionary<string, string>();
        }

        public static implicit operator Dictionary<string, string>(HttpHeaders headers)
        {
            return headers.headers;
        }

        public static implicit operator WebHeaderCollection(HttpHeaders headers)
        {
            var headerCollection = new WebHeaderCollection();
            foreach (var header in headers)
            {
                headerCollection.Add(header.Key, header.Value);   
            }
            return headerCollection;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return headers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return headers.GetEnumerator();
        }

        public override string ToString()
        {
            return headers.ToString();
        }
    }
}