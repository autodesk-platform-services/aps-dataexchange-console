using System.Threading;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Full story: create, geometry, param, sync, download.
    /// SDK: Full create + geometry + param + sync + STEP download.
    /// Console plumbing: ExchangeSessionHelper + ElementSampleHelper + ParameterSampleHelper + DownloadSampleHelper.
    /// Prerequisites: folder in session.
    /// </summary>
    [SampleAddress(11, 10)]
    public sealed class EndToEndCreateSyncDownloadScenarioSample : ISample
    {
        public string Name => "End To End Create Sync Download";
        public string Description => "Create, geometry, param, sync, download STEP";

        public async Task RunAsync(SampleContext ctx)
        {
            TerminalUi.Chat("Scenario 11.10: Single element workflow -> param -> sync -> STEP");
            try
            {
                if (!await ExchangeSessionHelper.CreateAndPrepareEmptyAsync(ctx))
                    return;
                if (!await ElementSampleHelper.CreateRootElementWithStepGeometryAsync(ctx, syncAfter: true))
                    return;

                var session = await ParameterSampleHelper.BeginAsync(ctx);
                var element = session == null ? null : ParameterSampleHelper.PickElement(session.Model);
                if (element != null)
                    await ParameterSampleHelper.AddBuiltInInstanceParamInteractiveAsync(ctx, element);

                await ExchangeSessionHelper.SyncDemoLineAsync(ctx, ctx.ScenarioExchangeTitle);

                var downloadSession = await DownloadSampleHelper.BeginAsync(ctx);
                if (downloadSession != null)
                {
                    var path = DownloadSampleHelper.ResolveOutputPath("STEP output path", "exchange.step");
                    await DownloadSampleHelper.RunExchangeDownloadAsync(
                        "STEP",
                        () => ctx.Client.DownloadCompleteExchangeAsSTEP(downloadSession.Identifier, path, CancellationToken.None));
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
