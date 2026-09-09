using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to list designs and get one by definition element id.
    /// SDK: ElementDataModel.GetDesigns + GetDesign.
    /// Console plumbing: DesignSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(6, 2)]
    public sealed class GetDesignSample : ISample
    {
        public string Name => "Get Design";
        public string Description => "Get a design by definition element id";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await DesignSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var designs = session.Model.GetDesignRefs().ToList();
            if (designs.Count == 0)
            {
                TerminalUi.Warning("No designs in model. Run 6.1 first.");
                return;
            }

            foreach (var entry in designs)
                TerminalUi.Chat($"  {entry.Name} ({entry.SourceId})");
            var elementId = Prompt.AskString("Definition element id", null);
            if (string.IsNullOrWhiteSpace(elementId))
                return;
            var selectedDesign = session.Model.GetDesign(elementId);
            if (selectedDesign == null)
            {
                TerminalUi.Warning($"No design for element id: {elementId}");
                return;
            }

            DesignSampleHelper.PrintDesignSummary(session.Model, selectedDesign);
        }
    }
}
