using System;
using System.Collections.Generic;
using HttpServer.Common;

namespace HttpServer.Server
{
    public class RouteTableBuilder
    {
        private readonly Dictionary<string, Dictionary<string, IRoutedHandler>> map;

        public RouteTableBuilder()
        {
            map = new Dictionary<string, Dictionary<string, IRoutedHandler>>(StringComparer.OrdinalIgnoreCase);
        }


        public RouteTableBuilder MapHandler(HttpMethod method, string uriTemplate, IRoutedHandler handler)
        {
            Dictionary<string, IRoutedHandler> innerMap;
            if (!map.TryGetValue(uriTemplate, out innerMap))
            {
                innerMap = new Dictionary<string, IRoutedHandler>(StringComparer.OrdinalIgnoreCase);
                map[uriTemplate] = innerMap;
            }
            innerMap[method.ToString()] = handler;
            return this;
        }

        public RouteTable Build()
        {
            return new RouteTable(map);
        }
    }
}