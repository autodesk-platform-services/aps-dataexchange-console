using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.DataExchange;
using Autodesk.DataExchange.Interface;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to turn off the SDK's built-in download progress so your connector can report its own.
    /// SDK: GeometryOutputOptions.ReportDownloadProgress + ElementDataModel.GetElementGeometriesAsync.
    /// Console plumbing: GeometrySampleHelper.BeginAsync, TerminalUi.Progress.
    /// Prerequisites: 2.3 Load Exchange (with elements that have geometry).
    /// </summary>
    [SampleAddress(4, 5, 13)]
    public sealed class DownloadGeometriesWithOwnProgressSample : ISample
    {
        public string Name => "Download Geometries with Own Progress";
        public string Description => "Disable SDK download progress and report per-element progress yourself";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await GeometrySampleHelper.BeginAsync(ctx);
            if (session == null)
                return;

            var elements = session.Model.Elements.Where(e => e.HasGeometry).Take(Prompt.AskInt("Max elements", 5)).ToList();
            if (elements.Count == 0)
            {
                TerminalUi.Warning("No elements with geometry in model.");
                return;
            }

            TerminalUi.Info("ReportDownloadProgress = false stops the SDK from publishing its own download progress steps.");
            TerminalUi.Info("Use it when the host shows its own progress UI, so the user does not see two progress bars.");
            // SDK: opt out of SDK-reported download progress
            var options = new GeometryOutputOptions { ReportDownloadProgress = false };

            var stopwatch = Stopwatch.StartNew();
            var total = 0;
            for (var i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                TerminalUi.Progress($"[{i + 1}/{elements.Count}] {element.Name} ({element.SourceId})");
                try
                {
                    // SDK: download one element's geometry with the options above
                    var result = await session.Model.GetElementGeometriesAsync(
                        new[] { element },
                        CancellationToken.None,
                        options);
                    var count = result.Values.Sum(g => g?.Count() ?? 0);
                    total += count;
                    TerminalUi.Chat($"    {count} geometry asset(s)");
                }
                catch (Exception ex)
                {
                    TerminalUi.Warning($"    Could not retrieve geometry: {ex.Message}");
                }
            }

            TerminalUi.Success($"Downloaded {total} geometry asset(s) for {elements.Count} element(s) in {stopwatch.Elapsed.TotalSeconds:F1}s.");
        }
    }
}
