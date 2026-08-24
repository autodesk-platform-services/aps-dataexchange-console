using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Versioning: modify, sync, reload old revision, diff.
    /// SDK: Sync + GetElementDataModelAsync(revisions) + delta APIs.
    /// Console plumbing: ExchangeSessionHelper + ElementSampleHelper + DeltaSampleHelper + ParameterSampleHelper.
    /// Prerequisites: folder in session.
    /// </summary>
    [SampleAddress(11, 7)]
    public sealed class TwoVersionRoundTripScenarioSample : ISample
    {
        public string Name => "Two Version Round Trip";
        public string Description => "Create v1, modify v2, reload v1, diff";

        public async Task RunAsync(SampleContext ctx)
        {
            TerminalUi.Chat("Scenario 11.7: Single element -> update param -> sync -> reload -> delta");
            try
            {
                if (!await ExchangeSessionHelper.CreateAndPrepareEmptyAsync(ctx))
                    return;
                if (!await ElementSampleHelper.CreateRootElementWithStepGeometryAsync(ctx, syncAfter: true))
                    return;

                var session = await DeltaSampleHelper.BeginAsync(ctx);
                if (session == null)
                    return;

                var revisionsBefore = await DeltaSampleHelper.GetRevisionIdsAsync(ctx, session);
                var v1Revision = revisionsBefore.LastOrDefault();

                var element = ParameterSampleHelper.PickElement(session.Model);
                if (element != null)
                {
                    await ParameterSampleHelper.AddCustomInstanceParamInteractiveAsync(element);
                    ParameterSampleHelper.UpdateInstanceParamInteractive(element);
                    await ElementSampleHelper.SyncAsync(ctx, session);
                }

                if (!string.IsNullOrWhiteSpace(v1Revision))
                {
                    TerminalUi.Info($"Reloading model at revision {v1Revision}...");
                    var reloaded = await DeltaSampleHelper.LoadAtRevisionAsync(ctx, session.Identifier, v1Revision, v1Revision);
                    if (reloaded != null)
                    {
                        var afterSession = await DeltaSampleHelper.BeginAsync(ctx);
                        if (afterSession != null)
                        {
                            var revisionIds = await DeltaSampleHelper.GetRevisionIdsAsync(ctx, afterSession);
                            DeltaSampleHelper.PrintDeltaSummary(afterSession.Model, revisionIds);
                        }
                    }
                }
                else
                {
                    TerminalUi.Info("Could not determine v1 revision id; skipping reload step.");
                }

                TerminalUi.Success("Scenario complete.");
            }
            finally
            {
                ExchangeSessionHelper.EndScenario(ctx);
            }
        }
    }
}
