using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using Autodesk.DataExchange.DataModels;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to detect instance vs design changes across revisions.
    /// SDK: Element.GetElementModifications with exchange revision ids.
    /// Console plumbing: ElementSampleHelper.BeginAsync, DeltaSampleHelper.GetRevisionIdsAsync.
    /// Prerequisites: 2.2 Load Exchange; multiple revisions.
    /// </summary>
    [SampleAddress(3, 7)]
    public sealed class ShowElementModificationsSample : ISample
    {
        public string Name => "Show Element Modifications";
        public string Description => "Show instance/design modifications across revisions";

        public async Task RunAsync(SampleContext ctx)
        {
            // Console plumbing (not SDK): pick loaded exchange and element
            var session = await ElementSampleHelper.BeginAsync(ctx);
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

            if (element is not Element concrete)
            {
                TerminalUi.Chat("Unexpected element type.");
                return;
            }

            // Console plumbing (not SDK): fetch revision ids
            var revisionIds = await DeltaSampleHelper.GetRevisionIdsAsync(ctx, session);
            if (revisionIds.Count == 0)
            {
                TerminalUi.Warning("No revisions found for this exchange.");
                return;
            }

            // SDK: inspect element modifications across revisions
            var modifications = concrete.GetElementModifications(revisionIds);
            TerminalUi.Chat($"Element: {element.Name} ({element.SourceId})");
            TerminalUi.Chat($"Revisions checked: {revisionIds.Count}");
            TerminalUi.Chat($"Modifications: {FormatModifications(modifications)}");
            if (modifications == ElementModifications.NoChange)
                TerminalUi.Warning("No instance or design changes detected in the listed revisions.");
        }

        private static string FormatModifications(ElementModifications modifications)
        {
            if (modifications == ElementModifications.NoChange)
                return "NoChange";

            var parts = new System.Collections.Generic.List<string>();
            if (modifications.HasFlag(ElementModifications.Instance))
                parts.Add("Instance");
            if (modifications.HasFlag(ElementModifications.Design))
                parts.Add("Design");
            return parts.Count == 0 ? modifications.ToString() : string.Join(", ", parts);
        }
    }
}
