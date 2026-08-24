using System;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to enable HTTP and SDK debug logging.
    /// SDK: ILogger.SetDebugLogLevel.
    /// Console plumbing: DiagnosticsSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(10, 1)]
    public sealed class ToggleHttpDebugLoggingSample : ISample
    {
        public string Name => "Toggle HTTP Debug Logging";
        public string Description => "Enable SDK debug and HTTP request logging";

        public Task RunAsync(SampleContext ctx)
        {
            var client = DiagnosticsSampleHelper.RequireClient(ctx);
            var enable = Prompt.AskString("Enable debug logging? [Y/n]", "Y");
            if (string.Equals(enable, "n", StringComparison.OrdinalIgnoreCase))
            {
                TerminalUi.Chat("Left logging unchanged.");
            return Task.CompletedTask;
            }

            DiagnosticsSampleHelper.EnableHttpDebugLogging(DiagnosticsSampleHelper.GetLogger(client));
            return Task.CompletedTask;
        }
    }
}
