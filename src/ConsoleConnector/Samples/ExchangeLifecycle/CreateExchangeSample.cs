using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to create a new empty Data Exchange in a Forma folder.
    /// SDK: IClient.CreateExchangeAsync with ExchangeCreateRequestACC.
    /// Console plumbing: ExchangeSessionHelper.CreateNewExchangeAsync.
    /// Prerequisites: saved session folder (Hub Id, Project URN, Folder URN).
    /// </summary>
    [SampleAddress(2, 1)]
    public sealed class CreateExchangeSample : ISample
    {
        public string Name => "Create Exchange";
        public string Description => "Create a new empty Data Exchange in your saved folder";

        public Task RunAsync(SampleContext ctx) => ExchangeSessionHelper.CreateNewExchangeAsync(ctx);
    }
}
