using System;
using System.Threading.Tasks;
using Autodesk.DataExchange.Insights;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Always pair metrics start with stop to get elapsed time.
    /// SDK: Metrics.StopTracking.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(10, 5)]
    public sealed class StopTelemetrySessionSample : ISample
    {
        public string Name => "Stop Telemetry Session";
        public string Description => "Stop tracking and print elapsed time";

        public Task RunAsync(SampleContext ctx)
        {
            Metrics.EnableMetrics(true);
            var elapsed = Metrics.StopTracking(DiagnosticsSampleHelper.TrackingId);
            TerminalUi.Success($"Stopped '{DiagnosticsSampleHelper.TrackingId}': {elapsed} ms");
            return Task.CompletedTask;
        }
    }
}
