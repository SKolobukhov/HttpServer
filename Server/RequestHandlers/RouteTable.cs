using System.Collections.Generic;

namespace HttpServer.Server
{
    public class RouteTable
    {
        private readonly Dictionary<string, Dictionary<string, IRoutedHandler>> map;

        internal RouteTable(Dictionary<string, Dictionary<string, IRoutedHandler>> map)
        {
            this.map = map;
        }

        internal RouteMatchResult TryMatch(string method, string requestUri, out IRoutedHandler handler)
        {
            requestUri = requestUri.Trim('/');
            handler = null;
            if (!map.ContainsKey(requestUri))
            {
                return RouteMatchResult.UnrecognizedUri;
            }
            if (!map[requestUri].ContainsKey(method))
            {
                return RouteMatchResult.UnrecognizedMethod;
            }
            handler = map[requestUri][method];
            return RouteMatchResult.Matched;
        }
    }
}