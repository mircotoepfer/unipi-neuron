using System;
using System.Threading.Tasks;

namespace adcoso.iot.devicedrivers.neuron.driver.commons
{

    public delegate void LogInformation(DateTime timeStamp, string instance, string logMessage, LogLevel loggingLevel);

    internal class DriverLogger
    {
        #region Private Members
        private LogLevel _logLevel;
        #endregion Private Members

        #region Constructor
        internal DriverLogger(LogLevel logLevel)
        {
            _logLevel = logLevel;
        }
        #endregion Constructor

        #region Public Events
        internal event LogInformation OnLogInformation;
        #endregion

        #region Internal Methods

        internal void Log(object caller, Exception exception) => Log(caller, exception.Message + Environment.NewLine + exception.StackTrace, LogLevel.Error);

        internal void LogInformation(object caller, string logMessage) => Log(caller, logMessage, LogLevel.Information);

        internal void LogDebug(object caller, string logMessage) => Log(caller, logMessage, LogLevel.Debug);

        internal void LogMonitor(object caller, string logMessage) => Log(caller, logMessage, LogLevel.Monitor);

        internal void LogError(object caller, string logMessage) => Log(caller, logMessage, LogLevel.Error);

        public void LogInstantiating(object caller) => Log(caller, "creating new instance of " + caller?.GetType(), LogLevel.Debug);

        internal void SetLogLevel(LogLevel logLevel) => _logLevel = logLevel;

        public void LogException(object caller, Exception exception) => Log(caller, exception.ToString(), LogLevel.Exception);

        #endregion Internal Methods

        #region Private Methods
        private void Log(object caller, string logmessage, LogLevel loggingLevel)
        {

            if (loggingLevel > _logLevel)
            {
                return;
            }

            if (caller == null)
            {
                caller = new object();
            }


            if (OnLogInformation == null)
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    OnLogInformation.Invoke(DateTime.Now, caller.GetType()?.Name, logmessage, loggingLevel);
                }
                catch (Exception)
                {
                    // ignored
                }
            });
        }
        #endregion


    }
}

