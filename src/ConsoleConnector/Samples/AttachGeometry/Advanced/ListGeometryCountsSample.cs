using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Per-element geometry asset counts.
    /// SDK: ElementDataModel.GetElementGeometryCounts.
    /// Console plumbing: GeometrySampleHelper.BeginAsync.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(4, 5, 6)]
    public sealed class ListGeometryCountsSample : ISample
    {
        public string Name => "List Geometry Counts";
        public string Description => "Show geometry counts for a picked element";

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

            var counts = session.Model.GetElementGeometryCounts(new[] { element });
            TerminalUi.Chat($"Geometry counts for {element.Name} ({element.SourceId}):");
            GeometrySampleHelper.PrintGeometryCounts(counts);
        }
    }
}
