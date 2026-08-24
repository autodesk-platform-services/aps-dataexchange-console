using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Design-ref pattern: one definition, multiple instances.
    /// SDK: GetOrCreateDesignRef + InstantiateDesign + SyncExchangeDataAsync.
    /// Console plumbing: ExchangeSessionHelper + DesignSampleHelper.
    /// Prerequisites: folder in session.
    /// </summary>
    [SampleAddress(11, 6)]
    public sealed class ElementWithDesignRefScenarioSample : ISample
    {
        public string Name => "Element With Design Ref";
        public string Description => "Design ref with two instances";

        public async Task RunAsync(SampleContext ctx)
        {
            TerminalUi.Chat("Scenario 11.6: Create -> empty model -> Design Ref -> Instantiate x2 -> Sync");
            if (!await ExchangeSessionHelper.CreateAndPrepareEmptyAsync(ctx))
                return;

            try
            {
                if (await DesignSampleHelper.CreateOrGetDesignRefInteractiveAsync(ctx, syncAfter: false) == null)
                    return;

                await DesignSampleHelper.InstantiateDesignInteractiveAsync(ctx, syncAfter: false);
                await DesignSampleHelper.InstantiateDesignInteractiveAsync(ctx, syncAfter: false);
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
