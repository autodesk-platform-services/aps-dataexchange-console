using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Incremental pull: load, refresh, see what changed.
    /// SDK: RetrieveLatestExchangeAsync + GetCreated/Modified/DeletedElements.
    /// Console plumbing: ExchangeSessionHelper + DeltaSampleHelper.
    /// Prerequisites: folder in session.
    /// </summary>
    [SampleAddress(11, 8)]
    public sealed class PullIncrementalRevisionScenarioSample : ISample
    {
        public string Name => "Pull Incremental Revision";
        public string Description => "Load exchange, pull latest, show delta";

        public async Task RunAsync(SampleContext ctx)
        {
            TerminalUi.Chat("Scenario 11.8: Load -> (optional external sync) -> pull latest -> delta");
            await ExchangeSessionHelper.LoadPickedExchangeAsync(ctx);
            Prompt.AskString("Press Enter after syncing changes elsewhere (or to continue with local state)", null);

            var session = await DeltaSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;

            await DeltaSampleHelper.PullLatestAsync(ctx, session.Model);
            var revisionIds = await DeltaSampleHelper.GetRevisionIdsAsync(ctx, session);
            if (revisionIds.Count < 2)
            {
                TerminalUi.Chat("Need 2+ revisions for meaningful delta. Sync changes with 2.3 first.");
                if (revisionIds.Count == 0)
                    return;
            }

            TerminalUi.Chat($"Delta across {revisionIds.Count} revision(s):");
            DeltaSampleHelper.PrintDeltaSummary(session.Model, revisionIds);
            TerminalUi.Success("Scenario complete.");
        }
    }
}
