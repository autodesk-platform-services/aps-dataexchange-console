using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: The two-step load: resolve metadata, then load ElementDataModel into memory.
    /// SDK: IClient.GetExchangeDetailsAsync + GetElementDataModelAsync.
    /// Console plumbing: ExchangeSessionHelper.LoadPickedExchangeAsync.
    /// Prerequisites: saved session folder with at least one synced exchange (run 2.2 first for new exchanges).
    /// </summary>
    [SampleAddress(2, 3)]
    public sealed class LoadExchangeSample : ISample
    {
        public string Name => "Load Exchange";
        public string Description => "Load a Data Exchange from your saved folder into memory";

        public Task RunAsync(SampleContext ctx) => ExchangeSessionHelper.LoadPickedExchangeAsync(ctx);
    }
}
