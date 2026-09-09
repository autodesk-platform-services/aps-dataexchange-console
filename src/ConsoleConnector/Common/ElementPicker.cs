using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;

namespace ConsoleConnector.Common
{
    internal static class ElementPicker
    {
        internal static IElement? Pick(ElementDataModel model, string prompt = "Pick element")
        {
            var picked = PickFrom(model.Elements.ToList(), prompt);
            if (picked != null)
                return picked;

            var byId = Prompt.AskString("Or enter element id (Enter to cancel)", null);
            if (string.IsNullOrWhiteSpace(byId))
                return null;

            var found = model.GetElementsBySourceId(byId.Trim()).FirstOrDefault();
            if (found == null)
            {
                TerminalUi.Warning("Unknown element id.");
                return null;
            }

            TerminalUi.Chat($"Selected: {found.Name} ({found.SourceId})");
            return found;
        }

        /// <summary>Top-level elements only — used by scenario flows that add sibling children under one root.</summary>
        internal static IElement? PickTopLevel(ElementDataModel model, string prompt)
        {
            return PickFrom(model.TopLevelElements.ToList(), prompt);
        }

        private static IElement? PickFrom(IReadOnlyList<IElement> elements, string prompt)
        {
            var valid = elements.Where(e => !string.IsNullOrWhiteSpace(e.SourceId)).ToList();
            if (valid.Count == 0)
            {
                TerminalUi.Warning("No elements with usable ids in the loaded model.");
                return null;
            }

            if (valid.Count == 1)
            {
                var only = valid[0];
                TerminalUi.Chat($"{prompt}: {only.Name} ({only.SourceId})");
                return only;
            }

            if (BatchMode.Enabled)
            {
                var preferred = valid.LastOrDefault(IsSampleElement) ?? valid[^1];
                TerminalUi.Chat($"{prompt}: {preferred.Name} ({preferred.SourceId})");
                return preferred;
            }

            var picked = ListPicker.PickOne(
                prompt,
                valid,
                e => ListPicker.FormatNameAndDetail(e.Name, e.SourceId));

            if (picked != null)
            {
                TerminalUi.Chat($"Selected: {picked.Name} ({picked.SourceId})");
                return picked;
            }

            return null;
        }

        private static bool IsSampleElement(IElement element) =>
            element.Name?.StartsWith("Sample", StringComparison.OrdinalIgnoreCase) == true
            || element.SourceId.StartsWith("Line_", StringComparison.Ordinal)
            || element.SourceId.StartsWith("Root_", StringComparison.Ordinal)
            || element.SourceId.StartsWith("Child_", StringComparison.Ordinal)
            || element.SourceId.StartsWith("Geom_", StringComparison.Ordinal);

        internal static void PrintList(IReadOnlyList<IElement> elements)
        {
            var rows = elements
                .Select((e, i) => (Index: (i + 1).ToString(), Name: e.Name, Id: e.SourceId))
                .ToList();

            TerminalUi.WriteTable(
                null,
                rows,
                ("#", r => r.Index),
                ("Name", r => r.Name),
                ("Id", r => r.Id));
        }
    }
}
