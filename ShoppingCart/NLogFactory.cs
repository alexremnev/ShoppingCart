using System;
using NLog;
using NHibernate;

namespace ShoppingCart
{
    public class NLogFactory : ILoggerFactory
    {
        #region ILoggerFactory Members

        public IInternalLogger LoggerFor(Type type)
        {
            return new NLogLogger(LogManager.GetLogger(type.FullName));
        }

        public IInternalLogger LoggerFor(string keyName)
        {
            return new NLogLogger(LogManager.GetLogger(keyName));
        }

        #endregion
    }

    public class NLogLogger : IInternalLogger
    {
        private readonly Logger _logger;

        public NLogLogger(Logger logger)
        {
            _logger = logger;
        }

        #region Properties

        public bool IsDebugEnabled => _logger.IsDebugEnabled;

        public bool IsErrorEnabled => _logger.IsErrorEnabled;

        public bool IsFatalEnabled => _logger.IsFatalEnabled;

        public bool IsInfoEnabled => _logger.IsInfoEnabled;

        public bool IsWarnEnabled => _logger.IsWarnEnabled;

        #endregion

        #region IInternalLogger Methods

        public void Debug(object message, Exception exception)
        {
            _logger.Debug(exception, message.ToString());
        }

        public void Debug(object message)
        {
            _logger.Debug(message.ToString());
        }

        public void DebugFormat(string format, params object[] args)
        {
            _logger.Debug(format, args);
        }

        public void Error(object message, Exception exception)
        {
            _logger.Error(exception, message.ToString());
        }

        public void Error(object message)
        {
            _logger.Error(message.ToString());
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _logger.Error(format, args);
        }

        public void Fatal(object message, Exception exception)
        {
            _logger.Fatal(exception, message.ToString());
        }

        public void Fatal(object message)
        {
            _logger.Fatal(message.ToString());
        }

        public void Info(object message, Exception exception)
        {
            _logger.Info(exception, message.ToString());
        }

        public void Info(object message)
        {
            _logger.Info(message.ToString());
        }

        public void InfoFormat(string format, params object[] args)
        {
            _logger.Info(format, args);
        }

        public void Warn(object message, Exception exception)
        {
            _logger.Warn(exception, message.ToString());
        }

        public void Warn(object message)
        {
            _logger.Warn(message.ToString());
        }

        public void WarnFormat(string format, params object[] args)
        {
            _logger.Warn(format, args);
        }

        #endregion
    }
}