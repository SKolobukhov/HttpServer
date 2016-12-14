using System;
using log4net;
using log4net.Core;

namespace TestServer
{
    public class ConsoleLog : ILog
    {
        public ILogger Logger { get; }
        public bool IsDebugEnabled { get; }
        public bool IsInfoEnabled { get; }
        public bool IsWarnEnabled { get; }
        public bool IsErrorEnabled { get; }
        public bool IsFatalEnabled { get; }


        public ConsoleLog()
        {
            IsInfoEnabled = true;
            IsWarnEnabled = true;
            IsDebugEnabled = true;
            IsErrorEnabled = true;
            IsFatalEnabled = true;
        }

        public void Debug(object message) => DebugFormat(message.ToString());
        public void Debug(object message, Exception exception) => DebugFormat("{0},{1}", message, exception);
        public void Debug(string message) => DebugFormat(message);
        public void Debug(Exception exception) => DebugFormat("{0}", exception);
        public void Debug(string message, Exception exception) => DebugFormat("{0},{1}", message, exception);
        public void DebugFormat(string format, object arg0) => DebugFormat(format, new[] { arg0 });
        public void DebugFormat(string format, object arg0, object arg1) => DebugFormat(format, new[] { arg0, arg1 });
        public void DebugFormat(string format, object arg0, object arg1, object arg2) => DebugFormat(format, new[] { arg0, arg1, arg2 });
        public void DebugFormat(string format, params object[] parameters) => DebugFormat(null, format, parameters);
        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (!IsDebugEnabled) return;
            Console.WriteLine(Format(provider, "DEBUG " + format, args));
        }

        public void Info(object message) => InfoFormat(message.ToString());
        public void Info(object message, Exception exception) => InfoFormat("{0},{1}", message, exception);
        public void Info(string message) => InfoFormat(message);
        public void Info(Exception exception) => InfoFormat("{0}", exception);
        public void Info(string message, Exception exception) => InfoFormat("{0},{1}", message, exception);
        public void InfoFormat(string format, object arg0) => InfoFormat(format, new[] { arg0 });
        public void InfoFormat(string format, object arg0, object arg1) => InfoFormat(format, new[] { arg0, arg1 });
        public void InfoFormat(string format, object arg0, object arg1, object arg2) => InfoFormat(format, new[] { arg0, arg1, arg2 });
        public void InfoFormat(string format, params object[] parameters) => InfoFormat(null, format, parameters);
        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (!IsInfoEnabled) return;
            Console.WriteLine(Format(provider, "INFO " + format, args));
        }

        public void Warn(object message) => WarnFormat(message.ToString());
        public void Warn(object message, Exception exception) => WarnFormat("{0},{1}", message, exception);
        public void Warn(string message) => WarnFormat(message);
        public void Warn(Exception exception) => WarnFormat("{0}", exception);
        public void Warn(string message, Exception exception) => WarnFormat("{0},{1}", message, exception);
        public void WarnFormat(string format, object arg0) => WarnFormat(format, new[] { arg0 });
        public void WarnFormat(string format, object arg0, object arg1) => WarnFormat(format, new[] { arg0, arg1 });
        public void WarnFormat(string format, object arg0, object arg1, object arg2) => WarnFormat(format, new[] { arg0, arg1, arg2 });
        public void WarnFormat(string format, params object[] parameters) => WarnFormat(null, format, parameters);
        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (!IsWarnEnabled) return;
            Console.WriteLine(Format(provider, "WARN " + format, args));
        }

        public void Error(object message) => ErrorFormat(message.ToString());
        public void Error(object message, Exception exception) => ErrorFormat("{0},{1}", message, exception);
        public void Error(string message) => ErrorFormat(message);
        public void Error(Exception exception) => ErrorFormat("{0}", exception);
        public void Error(string message, Exception exception) => ErrorFormat("{0},{1}", message, exception);
        public void ErrorFormat(string format, object arg0) => ErrorFormat(format, new[] { arg0 });
        public void ErrorFormat(string format, object arg0, object arg1) => ErrorFormat(format, new[] { arg0, arg1 });
        public void ErrorFormat(string format, object arg0, object arg1, object arg2) => ErrorFormat(format, new[] { arg0, arg1, arg2 });
        public void ErrorFormat(string format, params object[] parameters) => ErrorFormat(null, format, parameters);
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (!IsErrorEnabled) return;
            Console.WriteLine(Format(provider, "ERROR " + format, args));
        }

        public void Fatal(object message) => FatalFormat(message.ToString());
        public void Fatal(object message, Exception exception) => FatalFormat("{0},{1}", message, exception);
        public void Fatal(string message) => FatalFormat(message);
        public void Fatal(Exception exception) => FatalFormat("{0}", exception);
        public void Fatal(string message, Exception exception) => FatalFormat("{0},{1}", message, exception);
        public void FatalFormat(string format, object arg0) => FatalFormat(format, new[] { arg0 });
        public void FatalFormat(string format, object arg0, object arg1) => FatalFormat(format, new[] { arg0, arg1 });
        public void FatalFormat(string format, object arg0, object arg1, object arg2) => FatalFormat(format, new[] { arg0, arg1, arg2 });
        public void FatalFormat(string format, params object[] parameters) => FatalFormat(null, format, parameters);
        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (!IsFatalEnabled) return;
            Console.WriteLine(Format(provider, "FATAL " + format, args));
        }

        private string Format(IFormatProvider prov, string format, params object[] args)
        {
            try
            {
                return string.Format(prov, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff ") + format, args);
            }
            catch (Exception e)
            {
                return "Exception while rendering format [" + format + "]; Exception: " + e;
            }
        }
    }
}