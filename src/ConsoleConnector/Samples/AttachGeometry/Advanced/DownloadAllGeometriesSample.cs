using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.DataExchange.Interface;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to download all geometry in the exchange.
    /// SDK: GetGeometriesAsync.
    /// Console plumbing: GeometrySampleHelper.BeginAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 5, 9)]
    public sealed class DownloadAllGeometriesSample : ISample
    {
        public string Name => "Download All Geometries";
        public string Description => "Retrieve all exchange geometries via GetGeometriesAsync";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await GeometrySampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            IEnumerable<IElementGeometry>? geometries = null;
            await TerminalUi.RunWithStatusAsync(
                "Retrieving exchange geometries…",
                async () => geometries = await session.Model.GetGeometriesAsync(cancellationToken: CancellationToken.None).ConfigureAwait(false));
            var count = geometries?.Count() ?? 0;
            TerminalUi.Chat($"Retrieved {count} geometry asset(s) from the exchange.");
        }
    }
}
