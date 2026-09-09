using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to traverse one level of the element hierarchy.
    /// SDK: IElement.GetChildElements.
    /// Console plumbing: ElementSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(3, 6)]
    public sealed class ListChildElementsSample : ISample
    {
        public string Name => "List Child Elements";
        public string Description => "List direct children of a picked parent element";

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

            var parent = ElementPicker.Pick(model, "Pick a parent element");
            if (parent == null)
            {
                TerminalUi.Dim("Cancelled.");
                return;
            }

            var children = parent.GetChildElements().ToList();
            TerminalUi.Chat($"Children of {parent.Name} ({parent.SourceId}): {children.Count}");
            if (children.Count == 0)
            {
                TerminalUi.Warning("  (none — run 3.2 Add Child Element to create one)");
                return;
            }

            ElementPicker.PrintList(children);
        }
    }
}
