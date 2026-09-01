using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Instantiate copies design geometry onto a new instance element.
    /// SDK: InstantiateDesign + InstantiateDesignBySourceId.
    /// Console plumbing: DesignSampleHelper.InstantiateDesignInteractiveAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(6, 6)]
    public sealed class InstantiateDesignSample : ISample
    {
        public string Name => "Instantiate Design";
        public string Description => "Create instance elements from a design reference";

        public Task RunAsync(SampleContext ctx) =>
            DesignSampleHelper.InstantiateDesignInteractiveAsync(ctx, syncAfter: true);
    }
}
