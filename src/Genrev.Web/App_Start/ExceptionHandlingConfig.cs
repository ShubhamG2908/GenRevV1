using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web
{
    public static class ExceptionHandlingConfig
    {
        public static void RegisterExceptionHandler() {

            var ts = new Dymeng.Exceptions.TelemetryLoggerSettings(
                "http://telemetry.dymeng.com/v1/Errors/Submit", 
                0, 
                0, 
                Environment.MachineName);

            Dymeng.Exceptions.Configuration.TelemetryLoggerSettings = ts;
            
        }

    }
}