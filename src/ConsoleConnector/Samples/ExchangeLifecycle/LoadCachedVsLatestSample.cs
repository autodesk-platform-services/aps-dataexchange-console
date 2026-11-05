using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Models;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: The difference between loading the cached model and loading the latest revision in one call.
    /// SDK: IClient.GetElementDataModelAsync(identifier, loadLatest).
    /// Console plumbing: ElementSampleHelper.BeginAsync, ExchangeSessionHelper.RegisterLoaded.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(2, 7)]
    public sealed class LoadCachedVsLatestSample : ISample
    {
        public string Name => "Load Cached vs Latest";
        public string Description => "Compare GetElementDataModelAsync with loadLatest false and true";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ElementSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;

            TerminalUi.Info("loadLatest = false (default): returns the cached model as-is, no delta sync. A cache miss still loads from the server.");
            // SDK: cached load
            var cached = await LoadAsync(ctx, session.Identifier, loadLatest: false);
            if (cached == null)
                return;

            TerminalUi.Info("loadLatest = true: if the exchange is cached, first syncs it to the latest server revision, then returns it.");
            TerminalUi.Info("Use it for 'Update to latest' instead of a separate GetElementDataModelAsync + RetrieveLatestExchangeAsync.");
            // SDK: load and bring up to the latest revision
            var latest = await LoadAsync(ctx, session.Identifier, loadLatest: true);
            if (latest == null)
                return;

            if (latest is ElementDataModel model)
            {
                // Console plumbing (not SDK): later samples use the latest model
                ExchangeSessionHelper.RegisterLoaded(ctx, session.Details, model);
                TerminalUi.Success("Loaded exchange now points at the latest model.");
            }
            else
            {
                TerminalUi.Warning($"Latest model is a {latest.GetType().Name}, not an ElementDataModel, so the loaded exchange was not updated.");
            }
        }

        private static async Task<IElementDataModel?> LoadAsync(SampleContext ctx, DataExchangeIdentifier identifier, bool loadLatest)
        {
            IElementDataModel? model = null;
            var stopwatch = Stopwatch.StartNew();
            await TerminalUi.RunWithStatusAsync(
                $"GetElementDataModelAsync(loadLatest: {loadLatest})…",
                async () =>
                {
                    var response = await ctx.Client.GetElementDataModelAsync(identifier, loadLatest).ConfigureAwait(false);
                    if (response.IsFailed)
                        TerminalUi.Error($"Load failed: {response.Errors.FirstOrDefault()?.Message ?? "Unknown error"}");
                    else
                        model = response.Value;
                });
            if (model == null)
                return null;

            TerminalUi.WriteResultPanel(
                $"loadLatest: {loadLatest}",
                ("Elements", model.Elements.Count().ToString()),
                ("Elapsed", $"{stopwatch.ElapsedMilliseconds} ms"));
            return model;
        }
    }
}
