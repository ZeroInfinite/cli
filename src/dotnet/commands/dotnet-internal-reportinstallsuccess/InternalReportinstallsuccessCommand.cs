// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.DotNet.Configurer;
using Microsoft.DotNet.Cli.Telemetry;

namespace Microsoft.DotNet.Cli
{
    public class InternalReportinstallsuccess
    {
        internal const string TelemetrySessionIdEnvironmentVariableName = "DOTNET_CLI_TELEMETRY_SESSIONID";

        public static int Run(string[] args)
        {
            var telemetry = new ThreadBlockingTelemetry();
            ProcessInputAndSendTelemetry(args, telemetry);

            return 0;
        }

        public static void ProcessInputAndSendTelemetry(string[] args, ITelemetry telemetry)
        {
            var parser = Parser.Instance;
            var result = parser.ParseFrom("dotnet internal-reportinstallsuccess", args);

            var internalReportinstallsuccess = result["dotnet"]["internal-reportinstallsuccess"];

            var exeName = Path.GetFileName(internalReportinstallsuccess.Arguments.Single());
            telemetry.TrackEvent(
                "reportinstallsuccess",
                new Dictionary<string, string> { { "exeName", exeName } },
                new Dictionary<string, double>());
        }

        internal class ThreadBlockingTelemetry : ITelemetry
        {
            private Telemetry.Telemetry telemetry;

            internal ThreadBlockingTelemetry()
            {
                var sessionId =
                Environment.GetEnvironmentVariable(TelemetrySessionIdEnvironmentVariableName);
                telemetry = new Telemetry.Telemetry(new NoOpFirstTimeUseNoticeSentinel(), sessionId, blockThreadInitialization: true);
            }
            public bool Enabled => telemetry.Enabled;

            public void TrackEvent(string eventName, IDictionary<string, string> properties, IDictionary<string, double> measurements)
            {
                telemetry.ThreadBlockingTrackEvent(eventName, properties, measurements);
            }
        }
    }
}