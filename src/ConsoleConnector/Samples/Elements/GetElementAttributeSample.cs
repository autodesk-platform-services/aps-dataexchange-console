using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to read element attributes by name.
    /// SDK: IElement.Attributes + GetAttribute.
    /// Console plumbing: ElementSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(3, 9)]
    public sealed class GetElementAttributeSample : ISample
    {
        public string Name => "Get Element Attribute";
        public string Description => "Read a single attribute value by name";

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

            var attributes = element.Attributes;
            if (attributes == null || attributes.Count == 0)
            {
                TerminalUi.Warning($"{element.Name} ({element.SourceId}) has no attributes.");
            TerminalUi.Chat("Run 3.8 Add Element Attributes first.");
                return;
            }

            TerminalUi.Chat($"Attributes on {element.Name} ({element.SourceId}):");
            foreach (var kvp in attributes.OrderBy(a => a.Key))
                TerminalUi.Chat($"  {kvp.Key} = {kvp.Value}");
            var attributeName = Prompt.AskString("Attribute name to read", attributes.Keys.First());
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                TerminalUi.Chat("Attribute name is required.");
                return;
            }

            var value = element.GetAttribute(attributeName);
            if (value == null)
            {
                TerminalUi.Error($"Attribute not found: {attributeName}");
                return;
            }

            TerminalUi.Chat($"{attributeName} = {value}");
        }
    }
}
