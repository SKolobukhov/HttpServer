using System;

namespace HttpServer.Server.Utility
{
    internal static class Preconditions
    {
        public static void EnsureCondition(bool argumentCondition, string argumentName, string format = null, params object[] args)
        {
            if (!argumentCondition)
            {
                format = format ?? string.Empty;
                throw new ArgumentException(string.Format(format, args), argumentName);
            }
        }

        public static void EnsureNotNull<T>(T argument, string argumentName, string format = null, params object[] args)
            where T : class
        {
            if (argument == null)
            {
                format = format ?? string.Empty;
                throw new ArgumentNullException(argumentName, string.Format(format, args));
            }
        }
    }
}