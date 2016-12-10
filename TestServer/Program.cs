using System;
using System.Net;
using log4net;
using log4net.Core;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new ConsoleLog();
            var server = new HttpServer.Server.HttpServer(1, null, AuthenticationSchemes.Anonymous, log);
            server.Start(789);
            Console.ReadKey();
            server.Stop();
            Console.ReadKey();
        }
    }

    public class ConsoleLog : ILog
    {
        public ConsoleLog()
        {
            IsInfoEnabled = true;
            IsWarnEnabled = true;
            IsDebugEnabled = true;
            IsErrorEnabled = true;
            IsFatalEnabled = true;
        }
        #region INFO
        public void Info(object message)
        {
            if (!IsInfoEnabled)
                return;
            CallsInfoCount++;
            Console.WriteLine(Format("INFO " + message));
        }

        public void Info(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Info(string message)
        {
            if (!IsInfoEnabled)
                return;
            CallsInfoCount++;
            Console.WriteLine(Format("INFO " + message));
        }
        public void Info(Exception exception)
        {
            if (!IsInfoEnabled)
                return;
            CallsInfoCount++;
            Console.WriteLine(Format("INFO {0}", exception));
        }
        public void Info(string message, Exception exception)
        {
            if (!IsInfoEnabled)
                return;
            CallsInfoCount++;
            Console.WriteLine(Format("INFO {0} {1}", message, exception));
        }
        public void InfoFormat(string format, object arg0)
        {
            if (!IsInfoEnabled)
                return;
            CallsInfoCount++;
            Console.WriteLine(Format("INFO " + format, arg0));
        }
        public void InfoFormat(string format, object arg0, object arg1)
        {
            if (!IsInfoEnabled)
                return;
            CallsInfoCount++;
            Console.WriteLine(Format("INFO " + format, arg0, arg1));
        }
        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            if (!IsInfoEnabled)
                return;
            CallsInfoCount++;
            Console.WriteLine(Format("INFO " + format, arg0, arg1, arg2));
        }
        public void InfoFormat(string format, params object[] parameters)
        {
            if (!IsInfoEnabled)
                return;
            CallsInfoCount++;
            Console.WriteLine(Format("INFO " + format, parameters));
        }
        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (!IsInfoEnabled)
                return;
            CallsInfoCount++;
            Console.WriteLine(Format(provider, "INFO " + format, args));
        }
        #endregion
        #region WARN
        public void Warn(object message)
        {
            if (!IsWarnEnabled)
                return;
            CallsWarnCount++;
            Console.WriteLine(Format("WARN " + message));
        }

        public void Warn(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Warn(string message)
        {
            if (!IsWarnEnabled)
                return;
            CallsWarnCount++;
            Console.WriteLine(Format("WARN " + message));
        }
        public void Warn(Exception exception)
        {
            if (!IsWarnEnabled)
                return;
            CallsWarnCount++;
            Console.WriteLine(Format("WARN {0}", exception));
        }
        public void Warn(string message, Exception exception)
        {
            if (!IsWarnEnabled)
                return;
            CallsWarnCount++;
            Console.WriteLine(Format("WARN {0} {1}", message, exception));
        }
        public void WarnFormat(string format, object arg0)
        {
            if (!IsWarnEnabled)
                return;
            CallsWarnCount++;
            Console.WriteLine(Format("WARN " + format, arg0));
        }
        public void WarnFormat(string format, object arg0, object arg1)
        {
            if (!IsWarnEnabled)
                return;
            CallsWarnCount++;
            Console.WriteLine(Format("WARN " + format, arg0, arg1));
        }
        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            if (!IsWarnEnabled)
                return;
            CallsWarnCount++;
            Console.WriteLine(Format("WARN " + format, arg0, arg1, arg2));
        }
        public void WarnFormat(string format, params object[] parameters)
        {
            if (!IsWarnEnabled)
                return;
            CallsWarnCount++;
            Console.WriteLine(Format("WARN " + format, parameters));
        }
        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (!IsWarnEnabled)
                return;
            CallsWarnCount++;
            Console.WriteLine(Format(provider, "WARN " + format, args));
        }
        #endregion

        #region ERROR
        public void Error(object message)
        {
            if (!IsErrorEnabled)
                return;
            CallsErrorCount++;
            Console.WriteLine(Format("ERROR " + message));
        }

        public void Error(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Error(string message)
        {
            if (!IsErrorEnabled)
                return;
            CallsErrorCount++;
            Console.WriteLine(Format("ERROR " + message));
        }
        public void Error(Exception exception)
        {
            if (!IsErrorEnabled)
                return;
            CallsErrorCount++;
            Console.WriteLine(Format("ERROR {0}", exception));
        }
        public void Error(string message, Exception exc)
        {
            if (!IsErrorEnabled)
                return;
            CallsErrorCount++;
            Console.WriteLine(Format("ERROR {0} {1}", message, exc));
        }
        public void ErrorFormat(string format, object arg0)
        {
            if (!IsErrorEnabled)
                return;
            CallsErrorCount++;
            Console.WriteLine(Format("ERROR " + format, arg0));
        }
        public void ErrorFormat(string format, object arg0, object arg1)
        {
            if (!IsErrorEnabled)
                return;
            CallsErrorCount++;
            Console.WriteLine(Format("ERROR " + format, arg0, arg1));
        }
        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            if (!IsErrorEnabled)
                return;
            CallsErrorCount++;
            Console.WriteLine(Format("ERROR " + format, arg0, arg1, arg2));
        }
        public void ErrorFormat(string format, params object[] parameters)
        {
            if (!IsErrorEnabled)
                return;
            CallsErrorCount++;
            Console.WriteLine(Format("ERROR " + format, parameters));
        }
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (!IsErrorEnabled)
                return;
            CallsErrorCount++;
            Console.WriteLine(Format(provider, "ERROR " + format, args));
        }
        #endregion
        #region DEBUG
        public void Debug(object message)
        {
            if (!IsDebugEnabled)
                return;
            CallsDebugCount++;
            Console.WriteLine(Format("DEBUG " + message));
        }

        public void Debug(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Debug(string message)
        {
            if (!IsDebugEnabled)
                return;
            CallsDebugCount++;
            Console.WriteLine(Format("DEBUG " + message));
        }
        public void Debug(Exception exception)
        {
            if (!IsDebugEnabled)
                return;
            CallsDebugCount++;
            Console.WriteLine(Format("DEBUG {0}", exception));
        }
        public void Debug(string message, Exception exception)
        {
            if (!IsDebugEnabled)
                return;
            CallsDebugCount++;
            Console.WriteLine(Format("DEBUG {0} {1}", message, exception));
        }

        public void DebugFormat(string format, object arg0)
        {
            if (!IsDebugEnabled)
                return;
            CallsDebugCount++;
            Console.WriteLine(Format("DEBUG " + format, arg0));
        }
        public void DebugFormat(string format, object arg0, object arg1)
        {
            if (!IsDebugEnabled)
                return;
            CallsDebugCount++;
            Console.WriteLine(Format("DEBUG " + format, arg0, arg1));
        }
        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            if (!IsDebugEnabled)
                return;

            CallsDebugCount++;
            Console.WriteLine(Format("DEBUG " + format, arg0, arg1, arg2));
        }
        public void DebugFormat(string format, params object[] parameters)
        {
            if (!IsDebugEnabled)
                return;
            CallsDebugCount++;
            Console.WriteLine(Format("DEBUG " + format, parameters));
        }
        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (!IsDebugEnabled)
                return;
            CallsDebugCount++;
            Console.WriteLine(Format(provider, "DEBUG " + format, args));
        }
        #endregion
        #region FATAL
        public void Fatal(object message)
        {
            if (!IsFatalEnabled)
                return;
            CallsFatalCount++;
            Console.WriteLine(Format("FATAL " + message));
        }

        public void Fatal(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Fatal(string message)
        {
            if (!IsFatalEnabled)
                return;
            CallsFatalCount++;
            Console.WriteLine(Format("FATAL " + message));
        }
        public void Fatal(Exception exception)
        {
            if (!IsFatalEnabled)
                return;
            CallsFatalCount++;
            Console.WriteLine(Format("FATAL {0}", exception));
        }
        public void Fatal(string message, Exception exception)
        {
            if (!IsFatalEnabled)
                return;
            CallsFatalCount++;
            Console.WriteLine(Format("FATAL {0} {1}", message, exception));
        }

        public void FatalFormat(string format, object arg0)
        {
            if (!IsFatalEnabled)
                return;
            CallsFatalCount++;
            Console.WriteLine(Format("FATAL " + format, arg0));
        }
        public void FatalFormat(string format, object arg0, object arg1)
        {
            if (!IsFatalEnabled)
                return;
            CallsFatalCount++;
            Console.WriteLine(Format("FATAL " + format, arg0, arg1));
        }
        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            if (!IsFatalEnabled)
                return;
            CallsFatalCount++;
            Console.WriteLine(Format("FATAL " + format, arg0, arg1, arg2));
        }
        public void FatalFormat(string format, params object[] parameters)
        {
            if (!IsFatalEnabled)
                return;
            CallsFatalCount++;
            Console.WriteLine(Format("FATAL " + format, parameters));
        }
        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (!IsFatalEnabled)
                return;
            CallsFatalCount++;
            Console.WriteLine(Format(provider, "FATAL " + format, args));
        }
        #endregion
        public void ResetCounters()
        {
            CallsInfoCount = 0;
            CallsWarnCount = 0;
            CallsDebugCount = 0;
            CallsErrorCount = 0;
            CallsFatalCount = 0;
        }
        #region FORMATS
        private string Format(string format, object arg0)
        {
            try
            {
                return string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff ") + format, arg0);
            }
            catch (Exception e)
            {
                return "Exception while rendering format [" + format + "]; Exception: " + e;
            }
        }
        private string Format(string format, object arg0, object arg1)
        {
            try
            {
                return string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff ") + format, arg0, arg1);
            }
            catch (Exception e)
            {
                return "Exception while rendering format [" + format + "]; Exception: " + e;
            }
        }
        private string Format(string format, object arg0, object arg1, object arg2)
        {
            try
            {
                return string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff ") + format, arg0, arg1, arg2);
            }
            catch (Exception e)
            {
                return "Exception while rendering format [" + format + "]; Exception: " + e;
            }
        }
        private string Format(string format, params object[] args)
        {
            try
            {
                return string.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff ") + format, args);
            }
            catch (Exception e)
            {
                return "Exception while rendering format [" + format + "]; Exception: " + e;
            }
        }
        private string Format(string format)
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff ") + format;
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
        #endregion
        public bool IsInfoEnabled { get; set; }
        public bool IsWarnEnabled { get; set; }
        public bool IsDebugEnabled { get; set; }
        public bool IsErrorEnabled { get; set; }
        public bool IsFatalEnabled { get; set; }
        public int CallsInfoCount { get; set; }
        public int CallsWarnCount { get; set; }
        public int CallsDebugCount { get; set; }
        public int CallsErrorCount { get; set; }
        public int CallsFatalCount { get; set; }
        public ILogger Logger { get; }
    }
}
