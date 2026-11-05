using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.DataExchange;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to filter geometry on download with GeometryFilters, and read BREP back as STEP or SAT.
    /// SDK: GetGeometriesAsync with GeometryOutputOptions.GeometryFilters / BrepOutputFormat.
    /// Console plumbing: GeometrySampleHelper.BeginAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 5, 10)]
    public sealed class FilterGeometryOutputSample : ISample
    {
        public string Name => "Filter Geometry Output";
        public string Description => "Download geometries filtered by GeometryFilters, BREP as STEP or SAT";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await GeometrySampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var filters = new[] { "Brep", "Mesh", "Primitive", "All" };
            var picked = ListPicker.PickOne(
                "Geometry filter",
                filters,
                f => ListPicker.FormatNameAndDetail(f, null));
            var filter = picked switch
            {
                "Brep" => GeometryFilters.Brep,
                "Mesh" => GeometryFilters.Mesh,
                "Primitive" => GeometryFilters.Primitive,
                _ => GeometryFilters.All,
            };

            var brepChoice = ListPicker.PickBinary("BREP output format", "STEP (default)", "SAT");
            var brepFormat = brepChoice == 2 ? BrepOutputFormat.SAT : BrepOutputFormat.STEP;

            var options = new GeometryOutputOptions { GeometryFilters = filter, BrepOutputFormat = brepFormat };
            IEnumerable<IElementGeometry>? geometries = null;
            await TerminalUi.RunWithStatusAsync(
                $"Retrieving {filter} geometries (BREP as {brepFormat})…",
                async () => geometries = await session.Model.GetGeometriesAsync(options, CancellationToken.None).ConfigureAwait(false));
            var list = geometries?.ToList() ?? new List<IElementGeometry>();
            TerminalUi.Success($"Retrieved {list.Count} geometry asset(s) with filter {filter}, BREP output {brepFormat}.");

            foreach (var geometry in list.OfType<FileGeometry>())
                TerminalUi.Chat($"  {geometry.SourceId ?? geometry.UniqueId}: {geometry.Format} -> {geometry.FilePath}");
        }
    }
}
