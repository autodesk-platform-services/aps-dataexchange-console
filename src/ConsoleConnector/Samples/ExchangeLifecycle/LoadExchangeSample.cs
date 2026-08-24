using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: The two-step load: resolve metadata, then load ElementDataModel into memory.
    /// SDK: IClient.GetExchangeDetailsAsync + GetElementDataModelAsync.
    /// Console plumbing: ExchangeSessionHelper.LoadPickedExchangeAsync.
    /// Prerequisites: saved session folder with at least one exchange.
    /// </summary>
    [SampleAddress(2, 2)]
    public sealed class LoadExchangeSample : ISample
    {
        public string Name => "Load Exchange";
        public string Description => "Load a Data Exchange from your saved folder into memory";

        public Task RunAsync(SampleContext ctx) => ExchangeSessionHelper.LoadPickedExchangeAsync(ctx);
    }
}
