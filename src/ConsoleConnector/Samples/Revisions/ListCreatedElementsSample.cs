using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Which elements were added since older revisions.
    /// SDK: ElementDataModel.GetCreatedElements.
    /// Console plumbing: DeltaSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(8, 1)]
    public sealed class ListCreatedElementsSample : ISample
    {
        public string Name => "List Created Elements";
        public string Description => "List elements created in newer revisions";

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
            DeltaSampleHelper.PrintElementList("Created", session.Model.GetCreatedElements(revisionIds));
        }
    }
}
