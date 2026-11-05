using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.Interface;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Element wrappers compare by identity, so they work as dictionary keys; IElement.Design is null when there is no design.
    /// SDK: IElement Equals / GetHashCode, ElementDataModel.Elements + GetElementsBySourceId, IElement.Design.
    /// Console plumbing: ElementSampleHelper.BeginAsync, ElementPicker.Pick.
    /// Prerequisites: 2.3 Load Exchange (with at least one element).
    /// </summary>
    [SampleAddress(3, 10)]
    public sealed class CompareElementsSample : ISample
    {
        public string Name => "Compare Elements";
        public string Description => "Element equality across lookups, elements as dictionary keys, null Design";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ElementSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var model = session.Model;
            if (!model.Elements.Any())
            {
                TerminalUi.Warning("No elements in model.");
                return;
            }

            var picked = ElementPicker.Pick(model, "Element");
            if (picked == null)
            {
                TerminalUi.Dim("Cancelled.");
                return;
            }

            TerminalUi.Info("Each lookup can hand back a new wrapper object for the same element.");
            TerminalUi.Info("Equals and GetHashCode compare the underlying asset id, so two wrappers for one element are equal.");
            // SDK: fetch the same element through two different lookups
            var fromTraversal = model.Elements.First(e => e.UniqueId == picked.UniqueId);
            var fromSourceId = string.IsNullOrEmpty(picked.SourceId)
                ? null
                : model.GetElementsBySourceId(picked.SourceId).FirstOrDefault(e => e.UniqueId == picked.UniqueId);
            if (fromSourceId == null)
            {
                TerminalUi.Warning("The picked element has no SourceId lookup match, so there is no second wrapper to compare. Pick another element.");
                return;
            }
            TerminalUi.Detail("Same object reference", ReferenceEquals(fromTraversal, fromSourceId).ToString());
            TerminalUi.Detail("Equals", fromTraversal.Equals(fromSourceId).ToString());
            TerminalUi.Detail("Same hash code", (fromTraversal.GetHashCode() == fromSourceId.GetHashCode()).ToString());

            TerminalUi.Info("That makes elements safe to use as dictionary or HashSet keys across traversals.");
            var notes = new Dictionary<IElement, string> { [fromTraversal] = "noted during traversal" };
            TerminalUi.Detail("Dictionary hit via other lookup", notes.TryGetValue(fromSourceId, out var note) ? note : "miss");

            TerminalUi.Info("IElement.Design returns null when the element has no design, so check it before use.");
            // SDK: Design is null for elements without one
            var withDesign = model.Elements.Count(e => e.Design != null);
            var withoutDesign = model.Elements.Count() - withDesign;
            TerminalUi.Detail("Picked element design", picked.Design?.Name ?? "(null)");
            TerminalUi.Detail("Elements with a design", withDesign.ToString());
            TerminalUi.Detail("Elements without a design", withoutDesign.ToString());
        }
    }
}
