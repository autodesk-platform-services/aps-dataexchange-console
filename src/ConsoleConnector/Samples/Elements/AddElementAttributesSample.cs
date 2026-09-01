using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: System-level key/value metadata on elements (not Revit parameters).
    /// SDK: IElement.AddAttributes + SyncExchangeDataAsync.
    /// Console plumbing: ElementSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(3, 8)]
    public sealed class AddElementAttributesSample : ISample
    {
        public string Name => "Add Element Attributes";
        public string Description => "Attach system-level attributes to an element and sync";

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

            TerminalUi.Chat("Pick an element:");
            var element = ElementPicker.Pick(model, "Element");
            if (element == null)
            {
                TerminalUi.Dim("Cancelled.");
                return;
            }

            var attributeName = Prompt.AskString("Attribute name", "ConsoleConnector.Tag");
            var attributeValue = Prompt.AskString("Attribute value", "sample-value");
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                TerminalUi.Chat("Attribute name is required.");
                return;
            }

            element.AddAttributes(new Dictionary<string, string>
            {
                [attributeName] = attributeValue ?? string.Empty,
            });
            TerminalUi.Success($"Set attribute on {element.Name} ({element.SourceId}):");
            TerminalUi.Chat($"  {attributeName} = {attributeValue}");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
