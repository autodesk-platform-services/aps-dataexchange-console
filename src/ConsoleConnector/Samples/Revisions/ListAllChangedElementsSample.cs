using System;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Full delta: created, modified, and deleted together.
    /// SDK: GetCreated/Modified/DeletedElements combined.
    /// Console plumbing: DeltaSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(8, 4)]
    public sealed class ListAllChangedElementsSample : ISample
    {
        public string Name => "List All Changed Elements";
        public string Description => "Summarize created, modified, and deleted elements";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await DeltaSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var revisionIds = await DeltaSampleHelper.GetRevisionIdsAsync(ctx, session);
            if (revisionIds.Count < 2)
            {
                TerminalUi.Chat("Need 2+ revisions for meaningful delta. Sync changes with 2.2 first.");
            if (revisionIds.Count == 0)
                    return;
            }

            TerminalUi.Chat($"Delta across {revisionIds.Count} revision(s):");
            DeltaSampleHelper.PrintDeltaSummary(session.Model, revisionIds);
        }
    }
}
