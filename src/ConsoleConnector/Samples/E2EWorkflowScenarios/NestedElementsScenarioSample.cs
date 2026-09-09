using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Hierarchy in one shot: root plus children, then sync.
    /// SDK: AddElement (root + children) + SyncExchangeDataAsync.
    /// Console plumbing: ExchangeSessionHelper + ElementSampleHelper.
    /// Prerequisites: folder in session.
    /// </summary>
    [SampleAddress(11, 3)]
    public sealed class NestedElementsScenarioSample : ISample
    {
        public string Name => "Nested Elements";
        public string Description => "Create exchange with root and two child elements";

        public async Task RunAsync(SampleContext ctx)
        {
            TerminalUi.Chat("Scenario 11.3: Create -> empty model -> Root -> Child x2 -> Sync");
            if (!await ExchangeSessionHelper.CreateAndPrepareEmptyAsync(ctx))
                return;

            try
            {
                var session = await ElementSampleHelper.BeginAsync(ctx);
                if (session == null)
                    return;

                if (ElementSampleHelper.AddRootElement(session.Model) == null)
                    return;

                ElementSampleHelper.AddChildElement(ctx, session);
                ElementSampleHelper.AddChildElement(ctx, session);

                TerminalUi.Info($"Syncing {session.Details.DisplayName ?? session.Active.ExchangeFileUrn}...");
                TerminalUi.Chat($"Elements before sync: {session.Model.Elements.Count()}");
                if (!await ElementSampleHelper.SyncAsync(ctx, session))
                    return;

                TerminalUi.Chat($"Elements after sync: {session.Model.Elements.Count()}");
                TerminalUi.Success("Scenario complete.");
            }
            finally
            {
                ExchangeSessionHelper.EndScenario(ctx);
            }
        }
    }
}
