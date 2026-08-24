using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: A design is a definition element plus GetOrCreateDesignRef.
    /// SDK: ElementDataModel.GetOrCreateDesignRef.
    /// Console plumbing: DesignSampleHelper.CreateOrGetDesignRefInteractiveAsync.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(6, 1)]
    public sealed class GetOrCreateDesignRefSample : ISample
    {
        public string Name => "Get Or Create Design Ref";
        public string Description => "Create a design reference from a definition element";

        public Task RunAsync(SampleContext ctx) =>
            DesignSampleHelper.CreateOrGetDesignRefInteractiveAsync(ctx, syncAfter: true);
    }
}
