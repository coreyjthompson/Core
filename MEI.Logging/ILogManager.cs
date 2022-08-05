using Microsoft.Extensions.Configuration;

namespace MEI.Logging
{
    public interface ILogManager
    {
        void Shutdown();

        void ConfigureVariables();
    }

    public class NLogManager
        : ILogManager
    {
        private readonly IConfiguration _config;
        private readonly ICorrelationProvider _correlationProvider;

        public NLogManager(IConfiguration config)
        {
            _config = config;
        }

        public NLogManager(IConfiguration config, ICorrelationProvider correlationProvider)
            : this(config)
        {
            _config = config;
            _correlationProvider = correlationProvider;
        }

        public void Shutdown()
        {
            NLog.LogManager.Shutdown();
        }

        public void ConfigureVariables()
        {
            string environment = null;

            if (_config["ApplicationOptions:Environment"] != null)
            {
                environment = _config["ApplicationOptions:Environment"];
            }

            if (environment == null && _config["Environment"] != null)
            {
                environment = _config["Environment"];
            }

            if (environment != null)
            {
                AddVariable("environment", environment);
            }

            string appName = null;

            if (_config["ApplicationOptions:AppName"] != null)
            {
                appName = _config["ApplicationOptions:AppName"];
            }

            if (appName == null && _config["AppName"] != null)
            {
                appName = _config["AppName"];
            }

            if (appName != null)
            {
                AddVariable("appName", appName);
            }

            if (_correlationProvider != null)
            {
                AddVariable("correlationId", _correlationProvider.GetCorrelationId());
            }

            /*Target dbTarget = LogManager.Configuration.ConfiguredNamedTargets.FirstOrDefault(x => x.Name == "logDatabase");
            if (dbTarget == null)
            {
                throw new Exception("Unable to find NLog database target with name: logDatabase.");
            }

            void SetConnectionString(DatabaseTarget target)
            {
                if (config["ConnectionStrings:Default"] == null)
                {
                    throw new Exception("Log database connection string not found at: ConnectionStrings:Log");
                }

                target.ConnectionString = config["ConnectionStrings:Log"];

                LogManager.ReconfigExistingLoggers();
            }

            switch (dbTarget)
            {
                case AsyncTargetWrapper targetWrapper:
                {
                    if (targetWrapper.WrappedTarget is DatabaseTarget target)
                    {
                        SetConnectionString(target);
                    }

                    break;
                }
                case DatabaseTarget target:
                    SetConnectionString(target);
                    break;
            }

            Target emailTarget = LogManager.Configuration.ConfiguredNamedTargets.FirstOrDefault(x => x.Name == "logFatalMail");
            if (emailTarget == null)
            {
                throw new Exception("Unable to find NLog email target with name: logFatalMail");
            }

            void SetFatalEmailAddress(MailTarget target)
            {
                if (config[""] == null)
                {
                    throw new Exception("Log fatal email address not found at: ");
                }

                target.To = config[""];
            }

            switch (emailTarget)
            {
                case AsyncTargetWrapper targetWrapper:
                {
                    if (targetWrapper.WrappedTarget is MailTarget target)
                    {
                        SetFatalEmailAddress(target);
                    }

                    break;
                    }
                case MailTarget target:
                    SetFatalEmailAddress(target);
                    break;
            }*/
        }

        private void AddVariable(string name, string value)
        {
            NLog.LogManager.Configuration.Variables[name] = value;
        }
    }
}
