using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Core.Exceptions;

namespace Core.Utils
{
    public class LogUtil
    {
        private const string BCRM_Tag = "BCRM_Tag";
        private const string BCRM_Extra = "BCRM_Extra";

        public static void Log(ILogger logger, LogLevel logLevel, string tag, object extra, string message = "", params object[] args)
        {
            Dictionary<string, object> log_Extra = new Dictionary<string, object>();

            if (!String.IsNullOrWhiteSpace(tag)) log_Extra[BCRM_Tag] = tag;
            if (extra != null)
            {
                string extraJson = JsonConvert.SerializeObject(extra);
                log_Extra[BCRM_Extra] = extra;
            }

            using (logger.BeginScope(log_Extra))
            {
                logger.Log(logLevel, message, args);
            };
        }

        /// <summary>
        /// Log exception in error Level. 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        public static void LogException(ILogger logger, Exception ex, string message = "")
        {
            LogException(logger, LogLevel.Error, tag: null, ex, message);
        }

        public static void LogException(ILogger logger, string tag, Exception ex, string message = "")
        {
            LogException(logger, LogLevel.Error, tag: tag, ex, message);
        }

        /// <summary>
        /// Log exception in fatal Level.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        public static void LogFatalException(ILogger logger, Exception ex, string message = "")
        {
            LogException(logger, LogLevel.Critical, tag: null, ex, message);
        }

        public static void LogException(ILogger logger, LogLevel logLevel, Dictionary<string, object> list, Exception ex, string message = "")
        {
            StringBuilder sbMessageTemplate = new StringBuilder();
            string msgFatal = logLevel == LogLevel.Critical ? "Fatal " : "";

            if (!String.IsNullOrWhiteSpace(message))
                sbMessageTemplate.Append($"{message}, ");

            if (ex is CoreException)
                sbMessageTemplate.Append($"{msgFatal}{nameof(CoreException)} occurred = {{@Exception}}");
            else
                sbMessageTemplate.Append($"{msgFatal}Error occurred Exception = {{@Exception}}");

            if (list == null || list.Count == 0)
            {
                logger.Log(logLevel, sbMessageTemplate.ToString(), ex);
            }
            else
            {
                using (logger.BeginScope(list))
                {
                    logger.Log(logLevel, sbMessageTemplate.ToString(), ex);
                }
            }
        }

        public static void LogException(ILogger logger, LogLevel logLevel, string tag, Exception ex, string message = "")
        {
            StringBuilder sbMessageTemplate = new StringBuilder();
            string msgFatal = logLevel == LogLevel.Critical ? "Fatal " : "";

            if (!String.IsNullOrWhiteSpace(message))
                sbMessageTemplate.Append($"{message}, ");

            if (ex is CoreException)
                sbMessageTemplate.Append($"{msgFatal}{nameof(CoreException)} occurred = {{@Exception}}");
            else
                sbMessageTemplate.Append($"{msgFatal}Error occurred Exception = {{@Exception}}");

            if (String.IsNullOrWhiteSpace(tag))
            {
                logger.Log(logLevel, sbMessageTemplate.ToString(), ex);
            }
            else
            {
                using (logger.BeginScope(new Dictionary<string, object>
                {
                    [BCRM_Tag] = tag
                }))
                {
                    logger.Log(logLevel, sbMessageTemplate.ToString(), ex);
                }
            }
        }

        public static void SeriLogFatalException(Exception ex, string message = "")
        {
            SeriLogException(null, LogLevel.Critical, ex, message);
        }

        public static void SeriLogException(Exception ex, string message = "")
        {
            SeriLogException(null, LogLevel.Error, ex, message);
        }

        public static void SeriLogFatalException(Type sourceContext, Exception ex, string message = "")
        {
            SeriLogException(sourceContext, LogLevel.Critical, ex, message);
        }

        public static void SeriLogException(Type sourceContext, Exception ex, string message = "")
        {
            SeriLogException(sourceContext, LogLevel.Error, ex, message);
        }

        public static void SeriLogException(Type sourceContext, LogLevel logLevel, Exception ex, string message = "")
        {
            StringBuilder sbMessageTemplate = new StringBuilder();
            string msgFatal = logLevel == LogLevel.Critical ? "Fatal " : "";

            if (!string.IsNullOrWhiteSpace(message))
                sbMessageTemplate.Append($"{message}, ");

            if (ex is CoreException)
                sbMessageTemplate.Append($"{msgFatal}{nameof(CoreException)} occurred = {{@Exception}}");
            else
                sbMessageTemplate.Append($"{msgFatal}Error occurred Exception = {{@Exception}}");

            LogEventLevel logEventLevel;

            switch (logLevel)
            {
                case LogLevel.Trace:
                    logEventLevel = LogEventLevel.Verbose; 
                    break;
                case LogLevel.Debug:
                    logEventLevel = LogEventLevel.Debug; 
                    break;
                case LogLevel.Information:
                    logEventLevel = LogEventLevel.Information; 
                    break;
                case LogLevel.Warning:
                    logEventLevel = LogEventLevel.Warning; 
                    break;
                case LogLevel.Error:
                    logEventLevel = LogEventLevel.Error; 
                    break;
                case LogLevel.Critical:
                    logEventLevel = LogEventLevel.Fatal; 
                    break;
                case LogLevel.None:
                default:
                    logEventLevel = LogEventLevel.Information; 
                    break;                    
            }
            
            if (sourceContext == null)
                Serilog.Log.Write(logEventLevel, sbMessageTemplate.ToString(), ex);
            else
                Serilog.Log.ForContext(sourceContext).Write(logEventLevel, sbMessageTemplate.ToString(), ex);
        }
    }
}
