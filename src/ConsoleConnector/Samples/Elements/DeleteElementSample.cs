using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Deleting an element removes its subtree; sync to persist the change.
    /// SDK: ElementDataModel.DeleteElementByUniqueId + SyncExchangeDataAsync.
    /// Console plumbing: ElementSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(3, 5)]
    public sealed class DeleteElementSample : ISample
    {
        public string Name => "Delete Element";
        public string Description => "Remove an element and its hierarchy, then sync";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ElementSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var model = session.Model;
            if (!model.Elements.Any())
            {
                TerminalUi.Warning("No elements to delete.");
                return;
            }

            TerminalUi.Chat("Pick an element to delete:");
            var element = ElementPicker.Pick(model, "Element to delete");
            if (element == null)
            {
                TerminalUi.Dim("Cancelled.");
                return;
            }

            var beforeCount = model.Elements.Count();
            TerminalUi.Chat($"Deleting {element.Name} ({element.SourceId})...");
            TerminalUi.Chat($"Elements before: {beforeCount}");
            var deleted = model.DeleteElementByUniqueId(element.UniqueId);
            if (!deleted)
            {
                TerminalUi.Chat("DeleteElementByUniqueId returned false.");
                return;
            }

            TerminalUi.Chat("Removed from in-memory model.");
            if (!await ElementSampleHelper.SyncAsync(ctx, session))
                return;
            TerminalUi.Chat($"Elements after: {model.Elements.Count()}");
        }
    }
}
