using System;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Which elements were deleted across revisions.
    /// SDK: ElementDataModel.GetDeletedElements.
    /// Console plumbing: DeltaSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(8, 3)]
    public sealed class ListDeletedElementsSample : ISample
    {
        public string Name => "List Deleted Elements";
        public string Description => "List elements deleted in newer revisions";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await DeltaSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var revisionIds = await DeltaSampleHelper.GetRevisionIdsAsync(ctx, session);
            if (revisionIds.Count == 0)
            {
                TerminalUi.Warning("No revisions found.");
                return;
            }

            TerminalUi.Chat($"Checking {revisionIds.Count} revision(s)...");
            DeltaSampleHelper.PrintElementList("Deleted", session.Model.GetDeletedElements(revisionIds));
        }
    }
}
