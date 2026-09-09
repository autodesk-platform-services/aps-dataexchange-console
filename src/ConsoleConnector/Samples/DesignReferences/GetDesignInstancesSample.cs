using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to list instance elements that reference a design.
    /// SDK: ElementDataModel.GetDesignInstances.
    /// Console plumbing: DesignSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(6, 3)]
    public sealed class GetDesignInstancesSample : ISample
    {
        public string Name => "Get Design Instances";
        public string Description => "List instance elements for a design";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await DesignSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var designs = session.Model.GetDesignRefs().ToList();
            if (designs.Count == 0)
            {
                TerminalUi.Warning("No designs. Run 6.1 first.");
                return;
            }

            var design = ListPicker.PickOne(
                "Pick design",
                designs,
                d => ListPicker.FormatNameAndDetail(d.Name, d.SourceId));
            if (design == null)
            {
                TerminalUi.Dim("Cancelled.");
                return;
            }

            DesignSampleHelper.PrintDesignSummary(session.Model, design);
        }
    }
}
