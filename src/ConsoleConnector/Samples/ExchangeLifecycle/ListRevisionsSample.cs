using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Models;
using Autodesk.DataExchange.Models;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to inspect revision history without loading the full model.
    /// SDK: IClient.GetExchangeRevisionsAsync.
    /// Console plumbing: NavigationHelper (exchange picker).
    /// Prerequisites: saved session folder with at least one exchange.
    /// </summary>
    [SampleAddress(2, 5)]
    public sealed class ListRevisionsSample : ISample
    {
        public string Name => "List Revisions";
        public string Description => "List revisions for an exchange in your saved folder";

        public async Task RunAsync(SampleContext ctx)
        {
            // Console plumbing (not SDK): pick exchange from saved folder
            var picked = await NavigationHelper.PickExchangeAsync(ctx);
            if (picked == null)
                return;

            ExchangeDetails details;
            try
            {
                var collectionId = await ExchangeSessionHelper.ResolveCollectionIdAsync(ctx, picked.Project);
                var detailsResponse = await ctx.Client.GetExchangeDetailsAsync(collectionId, picked.FileUrn);
                if (detailsResponse.IsFailed)
                    throw new InvalidOperationException(
                        detailsResponse.Errors.FirstOrDefault()?.Message ?? "Failed to resolve exchange details.");
                details = detailsResponse.Value
                    ?? throw new InvalidOperationException(
                        $"Unable to resolve exchange details for {picked.FileUrn}.");
            }
            catch (Exception ex)
            {
                TerminalUi.Error($"Failed to resolve exchange: {ex}");
                return;
            }

            var identifier = ExchangeSessionHelper.ToIdentifier(details, ctx.Folder!.HubId);

            // SDK: list revisions
            var response = await ctx.Client.GetExchangeRevisionsAsync(identifier);
            if (response.IsFailed)
            {
                var message = response.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                TerminalUi.Error($"Failed: {message}");
                return;
            }

            var revisions = response.Value?
                .OrderByDescending(r => r.LastModifiedUTC)
                .ToList() ?? new System.Collections.Generic.List<ExchangeRevision>();

            if (revisions.Count == 0)
            {
                TerminalUi.Warning("No revisions.");
                return;
            }

            TerminalUi.WriteTable(
                $"Revisions for {details.DisplayName ?? picked.FileUrn}",
                revisions,
                ("Revision", r => r.Id),
                ("Modified", r => r.LastModifiedUTC.ToString("u")),
                ("Description", r => r.Description ?? string.Empty));
        }
    }
}
