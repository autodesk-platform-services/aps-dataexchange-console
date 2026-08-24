using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Minimal connector flow: create empty exchange and sync.
    /// SDK: CreateExchangeAsync + SyncExchangeDataAsync (empty exchange).
    /// Console plumbing: ExchangeSessionHelper.
    /// Prerequisites: folder in session.
    /// </summary>
    [SampleAddress(11, 1)]
    public sealed class SimpleExchangeScenarioSample : ISample
    {
        public string Name => "Simple Exchange";
        public string Description => "Create exchange and sync (empty)";

        public async Task RunAsync(SampleContext ctx)
        {
            TerminalUi.Chat("Scenario 11.1: Create -> empty model -> Sync");
            if (!await ExchangeSessionHelper.CreateAndPrepareEmptyAsync(ctx))
                return;

            try
            {
                await ExchangeSessionHelper.SyncDemoLineAsync(ctx, ctx.ScenarioExchangeTitle);
                TerminalUi.Success("Scenario complete.");
            }
            finally
            {
                ExchangeSessionHelper.EndScenario(ctx);
            }
        }
    }
}
