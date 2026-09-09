using System;
using System.Threading.Tasks;
using Autodesk.DataExchange.Insights;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to start an ADP metrics tracking session.
    /// SDK: Metrics.EnableMetrics + Metrics.StartTracking.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(10, 4)]
    public sealed class StartTelemetrySessionSample : ISample
    {
        public string Name => "Start Telemetry Session";
        public string Description => "Enable metrics and start a tracking session";

        public Task RunAsync(SampleContext ctx)
        {
            Metrics.EnableMetrics(true);
            Metrics.StartTracking(DiagnosticsSampleHelper.TrackingId);
            TerminalUi.Success($"Metrics enabled. Tracking '{DiagnosticsSampleHelper.TrackingId}' started.");
            TerminalUi.Chat("Run 10.5 Stop Telemetry Session when finished.");
            return Task.CompletedTask;
        }
    }
}
