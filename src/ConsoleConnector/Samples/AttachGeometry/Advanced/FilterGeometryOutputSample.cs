using System;
using System.Collections.Generic;
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
    /// What you learn: How to filter geometry on download with GeometryFilters.
    /// SDK: GetGeometriesAsync with GeometryFilters.
    /// Console plumbing: GeometrySampleHelper.BeginAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 5, 10)]
    public sealed class FilterGeometryOutputSample : ISample
    {
        public string Name => "Filter Geometry Output";
        public string Description => "Download geometries filtered by GeometryFilters";

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

            var options = new GeometryOutputOptions { GeometryFilters = filter };
            IEnumerable<IElementGeometry>? geometries = null;
            await TerminalUi.RunWithStatusAsync(
                $"Retrieving {filter} geometries…",
                async () => geometries = await session.Model.GetGeometriesAsync(options, CancellationToken.None).ConfigureAwait(false));
            var count = geometries?.Count() ?? 0;
            TerminalUi.Success($"Retrieved {count} geometry asset(s) with filter {filter}.");
        }
    }
}
