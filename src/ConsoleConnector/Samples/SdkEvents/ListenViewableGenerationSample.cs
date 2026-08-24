using System;
using System.Threading.Tasks;
using Autodesk.DataExchange.ProgressManager.Enums;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Sync progress includes viewable-generation steps.
    /// SDK: IProgressStepsManager during sync.
    /// Console plumbing: ElementSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(9, 3)]
    public sealed class ListenViewableGenerationSample : ISample
    {
        public string Name => "Listen Viewable Generation";
        public string Description => "Observe finalize/sync progress including viewable steps";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ElementSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var client = DiagnosticsSampleHelper.RequireClient(ctx);
            var listener = new ConsoleProgressListener();
            client.ProgressStepsManager.RegisterProgressUpdateListener(listener);
            client.ProgressStepsManager.SetActiveOperation(Operation.Update);
            TerminalUi.Info("Progress listener registered. Syncing exchange...");
            TerminalUi.Chat("(FinalizeExchange sub-steps include viewable creation during sync.)");
            await ElementSampleHelper.SyncAsync(ctx, session);
            TerminalUi.Chat("Sync finished. Review progress lines above for FinalizeExchange.");
        }
    }
}
