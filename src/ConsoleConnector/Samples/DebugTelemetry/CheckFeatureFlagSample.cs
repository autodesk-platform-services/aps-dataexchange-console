using System;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Feature flags are region-aware and queryable at runtime.
    /// SDK: IFeatureFlagController.GetFlagStatus.
    /// Console plumbing: DiagnosticsSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(10, 2)]
    public sealed class CheckFeatureFlagSample : ISample
    {
        public string Name => "Check Feature Flag";
        public string Description => "Query feature flag status for your region";

        public Task RunAsync(SampleContext ctx)
        {
            if (ctx.Folder == null || string.IsNullOrWhiteSpace(ctx.Folder.Region))
            {
                TerminalUi.Warning("No folder region in session.");
            return Task.CompletedTask;
            }

            var client = DiagnosticsSampleHelper.RequireClient(ctx);
            if (client.FeatureFlagController == null)
            {
                TerminalUi.Chat("Feature flag controller is not available.");
            return Task.CompletedTask;
            }

            var flagName = Prompt.AskString("Feature flag name", "DXSDK_EnableNewGeometryPipeline");
            var enabled = client.FeatureFlagController.GetFlagStatus(flagName, ctx.Folder.Region);
            TerminalUi.Success($"Flag '{flagName}' in {ctx.Folder.Region}: {(enabled ? "ON" : "OFF")}");
            return Task.CompletedTask;
        }
    }
}
