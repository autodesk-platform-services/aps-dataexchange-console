using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: The core write loop: mutate ElementDataModel locally, then push with SyncExchangeDataAsync.
    /// SDK: IClient.SyncExchangeDataAsync (with AddElement + CreatePrimitiveGeometry demo data).
    /// Console plumbing: ExchangeSessionHelper.SyncDemoLineAsync.
    /// Prerequisites: load an exchange first (2.2).
    /// </summary>
    [SampleAddress(2, 3)]
    public sealed class SyncExchangeSample : ISample
    {
        public string Name => "Sync Exchange";
        public string Description => "Add a sample line element and sync to a loaded exchange";

        public Task RunAsync(SampleContext ctx) => ExchangeSessionHelper.SyncDemoLineAsync(ctx, preferredTitle: null);
    }
}
