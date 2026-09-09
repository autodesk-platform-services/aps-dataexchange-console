using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Element metadata via built-in and custom instance params.
    /// SDK: AddElement + instance parameters + SyncExchangeDataAsync.
    /// Console plumbing: ExchangeSessionHelper + ElementSampleHelper + ParameterSampleHelper.
    /// Prerequisites: folder in session.
    /// </summary>
    [SampleAddress(11, 5)]
    public sealed class ElementWithPropertiesScenarioSample : ISample
    {
        public string Name => "Element With Properties";
        public string Description => "Root element with built-in and custom params";

        public async Task RunAsync(SampleContext ctx)
        {
            TerminalUi.Chat("Scenario 11.5: Create -> empty model -> Root -> built-in + custom params -> Sync");
            if (!await ExchangeSessionHelper.CreateAndPrepareEmptyAsync(ctx))
                return;

            try
            {
                var session = await ElementSampleHelper.BeginAsync(ctx);
                if (session == null)
                    return;

                var element = ElementSampleHelper.AddRootElement(session.Model);
                if (element == null)
                    return;

                await ParameterSampleHelper.AddBuiltInInstanceParamInteractiveAsync(ctx, element);
                await ParameterSampleHelper.AddCustomInstanceParamInteractiveAsync(element);
                await ElementSampleHelper.SyncAsync(ctx, session);
                TerminalUi.Success("Scenario complete.");
            }
            finally
            {
                ExchangeSessionHelper.EndScenario(ctx);
            }
        }
    }
}
