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
            var fileUrn = await NavigationHelper.PickExchangeFileUrnAsync(ctx);
            if (fileUrn == null)
                return;

            // SDK: resolve exchange metadata.
            // A picked exchange is known only by its file URN; the collection id needed by the
            // GetExchangeDetailsAsync(collectionId, urn) overload is discoverable only from the
            // details themselves, so the single-arg (obsolete) lookup is unavoidable here.
            ExchangeDetails details;
            try
            {
#pragma warning disable CS0618 // Type or member is obsolete
                details = await ctx.Client.GetExchangeDetailsAsync(fileUrn);
#pragma warning restore CS0618
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
                $"Revisions for {details.DisplayName ?? fileUrn}",
                revisions,
                ("Revision", r => r.Id),
                ("Modified", r => r.LastModifiedUTC.ToString("u")),
                ("Description", r => r.Description ?? string.Empty));
        }
    }
}
