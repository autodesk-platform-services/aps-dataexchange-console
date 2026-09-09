using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: First publish: create, add element, STEP geometry, sync.
    /// SDK: CreateExchangeAsync + AddElement + CreateFileGeometry (STEP) + SyncExchangeDataAsync.
    /// Console plumbing: ExchangeSessionHelper + ElementSampleHelper.
    /// Prerequisites: folder in session.
    /// </summary>
    [SampleAddress(11, 2)]
    public sealed class SingleElementWithGeometryScenarioSample : ISample
    {
        public string Name => "Single Element With Geometry";
        public string Description => "Create, add root element, BREP, sync";

        public Task RunAsync(SampleContext ctx) => RunAsync(ctx, clearScenarioBindingOnExit: true);

        internal async Task RunAsync(SampleContext ctx, bool clearScenarioBindingOnExit)
        {
            TerminalUi.Chat("Scenario 11.2: Create -> empty model -> Add Root -> BREP -> Sync");
            if (!await ExchangeSessionHelper.CreateAndPrepareEmptyAsync(ctx))
                return;

            try
            {
                if (!await ElementSampleHelper.CreateRootElementWithStepGeometryAsync(ctx, syncAfter: true))
                    return;

                TerminalUi.Success("Scenario complete.");
            }
            finally
            {
                if (clearScenarioBindingOnExit)
                    ExchangeSessionHelper.EndScenario(ctx);
            }
        }
    }
}
