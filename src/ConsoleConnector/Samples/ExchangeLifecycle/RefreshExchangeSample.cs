using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Models;
using Autodesk.DataExchange.Models;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to pull the latest cloud state into your in-memory model.
    /// SDK: IClient.GetExchangeRevisionsAsync + RetrieveLatestExchangeAsync.
    /// Console plumbing: ElementSampleHelper.BeginAsync (pick loaded exchange).
    /// Prerequisites: load an exchange first (2.3).
    /// </summary>
    [SampleAddress(2, 4)]
    public sealed class RefreshExchangeSample : ISample
    {
        public string Name => "Refresh Exchange";
        public string Description => "Pull the latest exchange data into your loaded model";

        public async Task RunAsync(SampleContext ctx)
        {
            // Console plumbing (not SDK): pick loaded exchange
            var session = await ElementSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;

            var model = session.Model;
            var title = session.Details.DisplayName ?? session.Active.ExchangeFileUrn;
            var beforeCount = model.Elements.Count();
            TerminalUi.Chat($"Refreshing {title}...");
            TerminalUi.Chat($"Elements before: {beforeCount}");

            // SDK: list revisions
            var revisionsResponse = await ctx.Client.GetExchangeRevisionsAsync(session.Identifier);
            if (revisionsResponse.IsFailed)
            {
                var message = revisionsResponse.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                TerminalUi.Error($"Failed to list revisions: {message}");
                return;
            }

            var revisions = revisionsResponse.Value?.ToList() ?? new System.Collections.Generic.List<ExchangeRevision>();
            if (revisions.Count == 0)
                TerminalUi.Warning("No revisions found.");
            else
            {
                var latest = revisions.OrderByDescending(r => r.LastModifiedUTC).First();
                TerminalUi.Chat($"Revisions: {revisions.Count} (latest {latest.Id}, {latest.LastModifiedUTC:u})");
            }

            // SDK: pull latest cloud data into in-memory model
            var refreshResponse = await ctx.Client.RetrieveLatestExchangeAsync(model, CancellationToken.None);
            if (refreshResponse.IsFailed)
            {
                var message = refreshResponse.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                TerminalUi.Error($"Refresh failed: {message}");
                return;
            }

            var afterCount = model.Elements.Count();
            TerminalUi.Success("Refresh complete.");
            TerminalUi.Chat($"Elements after: {afterCount}");
            foreach (var element in model.Elements.Take(10))
                TerminalUi.Chat($"  {element.Name} ({element.SourceId})");
            if (afterCount > 10)
                TerminalUi.Chat($"  ... and {afterCount - 10} more");
        }
    }
}
