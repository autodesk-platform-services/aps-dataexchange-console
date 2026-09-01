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
    /// What you learn: How to download geometry assets for specific elements.
    /// SDK: GetElementGeometriesAsync.
    /// Console plumbing: GeometrySampleHelper.BeginAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 5, 8)]
    public sealed class DownloadElementGeometriesSample : ISample
    {
        public string Name => "Download Element Geometries";
        public string Description => "Retrieve geometries for picked elements via GetElementGeometriesAsync";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await GeometrySampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            if (!session.Model.Elements.Any())
            {
                TerminalUi.Warning("No elements in model.");
                return;
            }

            var element = ElementPicker.Pick(session.Model, "Element");
            if (element == null)
            {
                TerminalUi.Dim("Cancelled.");
                return;
            }

            Dictionary<IElement, IEnumerable<IElementGeometry>> result = default!;
            try
            {
                await TerminalUi.RunWithStatusAsync(
                    "Retrieving element geometries…",
                    async () => result = await session.Model.GetElementGeometriesAsync(
                        new[] { element },
                        CancellationToken.None).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                TerminalUi.Warning($"Could not retrieve geometries: {ex}");
                return;
            }
            TerminalUi.Chat($"Retrieved geometries for {result.Count} element(s).");
            foreach (var pair in result)
            {
                var count = pair.Value?.Count() ?? 0;
                TerminalUi.Chat($"  {pair.Key.Name} ({pair.Key.SourceId}): {count} geometry asset(s)");
        }
        }
    }
}
