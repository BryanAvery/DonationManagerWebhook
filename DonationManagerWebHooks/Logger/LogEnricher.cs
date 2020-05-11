using Serilog.Core;
using Serilog.Events;
using System;

namespace DonationManagerWebHooks.Logger
{
    public class LogEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent le, ILogEventPropertyFactory lepf)
        {
            // I definitely want to know machine name for each log entry:
            le.AddPropertyIfAbsent(lepf.CreateProperty("MachineName", Environment.MachineName));
            le.AddPropertyIfAbsent(lepf.CreateProperty("UserName", Environment.UserName));
            le.AddPropertyIfAbsent(lepf.CreateProperty("Assembly", $"{System.Reflection.Assembly.GetCallingAssembly()}"));
            le.AddPropertyIfAbsent(lepf.CreateProperty("Version", $"{GetType().Assembly.GetName().Version}"));
        }
    }
}
