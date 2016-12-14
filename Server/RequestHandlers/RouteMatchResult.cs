namespace HttpServer.Server
{
    internal enum RouteMatchResult
    {
        Matched = 0,
        UnrecognizedUri = 1,
        UnrecognizedMethod = 2
    }
}