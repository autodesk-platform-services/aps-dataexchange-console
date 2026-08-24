using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to look up one element by its source id.
    /// SDK: ElementDataModel.GetElementsBySourceId.
    /// Console plumbing: ElementSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(3, 4)]
    public sealed class GetElementBySourceIdSample : ISample
    {
        public string Name => "Get Element By Source Id";
        public string Description => "Look up one element by its source id";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ElementSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var model = session.Model;
            var elementId = Prompt.AskString("Element source id");
            if (string.IsNullOrWhiteSpace(elementId))
            {
                TerminalUi.Chat("Element source id is required.");
                return;
            }

            var element = model.GetElementsBySourceId(elementId).FirstOrDefault();
            if (element == null)
            {
                TerminalUi.Warning($"No element found with source id: {elementId}");
                return;
            }

            TerminalUi.Chat("Found:");
            ElementSampleHelper.PrintElementSummary(element);
        }
    }
}
