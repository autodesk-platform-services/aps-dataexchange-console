using System;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Exchange-wide geometry totals.
    /// SDK: ElementDataModel.AllGeometryCounts.
    /// Console plumbing: GeometrySampleHelper.BeginAsync.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(4, 5, 7)]
    public sealed class ListAllGeometryCountsSample : ISample
    {
        public string Name => "List All Geometry Counts";
        public string Description => "Show exchange-wide geometry counts";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await GeometrySampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            TerminalUi.Chat("Exchange-wide geometry counts:");
            GeometrySampleHelper.PrintGeometryCounts(session.Model.AllGeometryCounts);
        }
    }
}
