using log4net;
using log4net.Appender;
using log4net.Config;
using System;
using System.Text;

// This attribute tells log4net to use the settings in the config file for configuration
[assembly: XmlConfigurator(Watch = true)]
namespace LAMP.Utility
{
    /// <summary>
    /// LogUtil class
    /// </summary>
    public class LogUtil
    {
        private static ILog logger = LogManager.GetLogger("LAMP");

        /// <summary>
        /// Create Logger
        /// </summary>
        /// <param name="processName">Process Name</param>
        public static void CreateLogger(string processName)
        {
            if (logger == null)
            {
                logger = log4net.LogManager.GetLogger(processName);
            }
        }

        // FileAppender delegation
        /// <summary>
        ///GetFileAppender 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static Predicate<IAppender> GetFileAppender(long id)
        {
            return delegate(IAppender appender)
            {
                return appender is FileAppender ? true : false;
            };
        }
        /// <summary>
        /// LogUtil
        /// </summary>
        private LogUtil()
        {
        }

        /// <summary>
        /// Logs information
        /// </summary>
        /// <param name="message">Message</param>
        public static void Info(string message)
        {
            if (logger == null)
            {
                return;
            }
            logger.Info(message);
        }

        /// <summary>
        /// Logs debug info
        /// </summary>
        /// <param name="message">Message</param>
        public static void Debug(string message)
        {
            if (logger == null)
            {
                return;
            }
            if (logger.IsDebugEnabled)
                logger.Debug(message);
        }

        /// <summary>
        /// Logs warning message
        /// </summary>
        /// <param name="message">Message</param>
        public static void Warning(string message)
        {
            if (logger == null)
            {
                return;
            }
            logger.Warn(message);
        }

        /// <summary>
        /// Logs exception as warning
        /// </summary>
        /// <param name="ex">Exception</param>
        public static void Warning(Exception ex)
        {
            if (logger == null)
            {
                return;
            }
            logger.Warn(CreateExceptionMessage(ex));
        }

        /// <summary>
        /// Logs error message
        /// </summary>
        /// <param name="message">Message</param>
        public static void Error(string message)
        {
            if (logger == null)
            {
                return;
            }
            logger.Error(message);
        }

        /// <summary>
        /// Logs error exception
        /// </summary>
        /// <param name="ex">Exception</param>
        public static void Error(Exception ex)
        {
            if (logger == null)
            {
                return;
            }
            logger.Error(CreateExceptionMessage(ex));
        }

        /// <summary>
        /// Formats exception message
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Formatted message</returns>
        private static string CreateExceptionMessage(Exception ex)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(ex.Message).Append(Environment.NewLine);
            buffer.Append("----------").Append(Environment.NewLine);
            buffer.Append(ex.ToString()).Append(Environment.NewLine);
            buffer.Append("----------").Append(Environment.NewLine);
            return buffer.ToString();
        }

    }
}
