using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: The core write loop: mutate ElementDataModel locally, then push with SyncExchangeDataAsync.
    /// SDK: IClient.SyncExchangeDataAsync (with AddElement + CreatePrimitiveGeometry demo data).
    /// Console plumbing: ExchangeSessionHelper.SyncDemoLineAsync.
    /// Prerequisites: create an exchange (2.1) or pick one from your saved folder.
    /// </summary>
    [SampleAddress(2, 2)]
    public sealed class SyncExchangeSample : ISample
    {
        public string Name => "Sync Exchange";
        public string Description => "Add a sample line element and sync (works on new empty exchanges too)";

        public Task RunAsync(SampleContext ctx) => ExchangeSessionHelper.SyncDemoLineAsync(ctx, preferredTitle: null);
    }
}
