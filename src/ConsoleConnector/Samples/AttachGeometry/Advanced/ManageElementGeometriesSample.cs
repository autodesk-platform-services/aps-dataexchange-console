using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.Interface;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: The six element-geometry edit methods and when to use each one.
    /// SDK: ElementDataModel.AddElementGeometry, UpdateElementGeometry, DeleteElementGeometry,
    /// UpdateElementGeometries, DeleteElementGeometries, SetElementGeometry, IElementGeometry.UniqueId.
    /// Console plumbing: GeometrySampleHelper.BeginAsync, PickOrCreateElementAsync, CreateTaggedLineGeometry.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 5, 11)]
    public sealed class ManageElementGeometriesSample : ISample
    {
        public string Name => "Manage Element Geometries";
        public string Description => "Add, update, delete (single and batch) and replace an element's geometry";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await GeometrySampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var element = await GeometrySampleHelper.PickOrCreateElementAsync(ctx, session.Model);
            if (element == null)
                return;

            var model = session.Model;
            var prefix = $"edit-{Guid.NewGuid():N}"[..11];
            var first = GeometrySampleHelper.CreateTaggedLineGeometry($"{prefix}-1", 0);
            var second = GeometrySampleHelper.CreateTaggedLineGeometry($"{prefix}-2", 50);
            var third = GeometrySampleHelper.CreateTaggedLineGeometry($"{prefix}-3", 100);

            TerminalUi.Section("1. AddElementGeometry");
            TerminalUi.Info("Appends new geometry to the element. Geometry already on the element keeps its UniqueId.");
            // SDK: append two geometries, then a third; the first two are not touched by the second call
            model.AddElementGeometry(element, new List<IElementGeometry> { first, second });
            model.AddElementGeometry(element, new List<IElementGeometry> { third });
            PrintLive(model, element, first, second, third);

            TerminalUi.Section("2. UpdateElementGeometry");
            TerminalUi.Info("Replaces one geometry in place, matched by UniqueId. The UniqueId stays the same; returns false if not found.");
            // SDK: swap the payload of the first geometry; passing no sourceId keeps the existing SourceId
            var updated = model.UpdateElementGeometry(
                element,
                first.UniqueId,
                GeometrySampleHelper.CreateTaggedLineGeometry(null, 10));
            TerminalUi.Chat($"  UpdateElementGeometry({Short(first.UniqueId)}) -> {updated}");
            PrintLive(model, element, first);

            TerminalUi.Section("3. DeleteElementGeometry");
            TerminalUi.Info("Removes one geometry by UniqueId and leaves the others. Returns false when the id is not on the element.");
            // SDK: delete the second geometry, then try again to show the false result
            var deleted = model.DeleteElementGeometry(element, second.UniqueId);
            var deletedAgain = model.DeleteElementGeometry(element, second.UniqueId);
            TerminalUi.Chat($"  DeleteElementGeometry({Short(second.UniqueId)}) -> {deleted}, second call -> {deletedAgain}");
            PrintLive(model, element, first, second, third);

            TerminalUi.Section("4. UpdateElementGeometries (batch)");
            TerminalUi.Info("Updates many geometries in one call. Returns UniqueId -> updated; unknown ids map to false.");
            // SDK: batch update, including an id that does not exist
            var updateResult = model.UpdateElementGeometries(element, new Dictionary<string, IElementGeometry>
            {
                [first.UniqueId] = GeometrySampleHelper.CreateTaggedLineGeometry(null, 20),
                [third.UniqueId] = GeometrySampleHelper.CreateTaggedLineGeometry(null, 120),
                ["missing-geometry-id"] = GeometrySampleHelper.CreateTaggedLineGeometry(null, 0),
            });
            TerminalUi.Chat($"  {GeometrySampleHelper.FormatBatchResult(updateResult)}");

            TerminalUi.Section("5. DeleteElementGeometries (batch)");
            TerminalUi.Info("Deletes many geometries in one call. Returns UniqueId -> removed; unknown ids map to false.");
            // SDK: batch delete, including an id that does not exist
            var deleteResult = model.DeleteElementGeometries(element, new[] { third.UniqueId, "missing-geometry-id" });
            TerminalUi.Chat($"  {GeometrySampleHelper.FormatBatchResult(deleteResult)}");
            PrintLive(model, element, first, third);

            TerminalUi.Section("6. SetElementGeometry");
            TerminalUi.Info("Replaces ALL of the element's geometry with the list you pass. Use it for a full rewrite, not an edit.");
            var replacement = GeometrySampleHelper.CreateTaggedLineGeometry($"{prefix}-final", 0);
            // SDK: replace everything on the element with a single new geometry
            model.SetElementGeometry(element, new List<IElementGeometry> { replacement });
            PrintLive(model, element, first, replacement);

            TerminalUi.Success($"Geometry edits applied to {element.Name} ({element.SourceId}).");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }

        private static void PrintLive(IElementDataModel model, IElement element, params IElementGeometry[] geometries)
        {
            foreach (var geometry in geometries)
            {
                // SDK: look up live geometry by the SourceId it was created with (no download)
                var live = model.GetGeometryMetaDataById(geometry.SourceId, element).ToList();
                var state = live.Count == 0
                    ? "not on element"
                    : string.Join(", ", live.Select(m => $"UniqueId {Short(m.UniqueId)}"));
                TerminalUi.Chat($"  {geometry.SourceId}: {state}");
            }
        }

        private static string Short(string id) => string.IsNullOrEmpty(id) ? "(none)" : id.Length <= 12 ? id : id[..12] + "…";
    }
}
