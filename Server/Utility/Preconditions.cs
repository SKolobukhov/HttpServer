using System;

namespace HttpServer.Server
{
    internal static class Preconditions
    {
        public static void EnsureCondition(bool argumentCondition, string argumentName, string format = null, params object[] args)
        {
            if (!argumentCondition)
            {
                throw new ArgumentException(string.Format(format ?? string.Empty, args), argumentName);
            }
        }

        public static void EnsureNotNull<T>(T argument, string argumentName, string format = null, params object[] args)
            where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName, string.Format(format ?? string.Empty, args));
            }
        }
    }
}