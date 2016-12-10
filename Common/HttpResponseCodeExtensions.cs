namespace HttpServer.Common
{
    internal static class HttpResponseCodeExtensions
    {
        public static bool IsInformational(this HttpResponseCode code) => CodeInRange(code, 100, 199);

        public static bool IsSuccessful(this HttpResponseCode code) => CodeInRange(code, 200, 299);

        public static bool IsRedirection(this HttpResponseCode code) => CodeInRange(code, 300, 399);

        public static bool IsClientError(this HttpResponseCode code) => CodeInRange(code, 400, 499);

        public static bool IsServerError(this HttpResponseCode code) => CodeInRange(code, 500, 599);

        private static bool CodeInRange(HttpResponseCode responseCode, int left, int right)
        {
            var code = (int) responseCode;
            return code >= left && code <= right;
        }
    }
}