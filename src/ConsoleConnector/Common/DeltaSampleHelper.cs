using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Models;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Models;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;

namespace ConsoleConnector.Common
{
    internal static class DeltaSampleHelper
    {
        internal static async Task<ElementSampleSession?> BeginAsync(SampleContext ctx) =>
            await ElementSampleHelper.BeginAsync(ctx);

        internal static async Task<List<string>> GetRevisionIdsAsync(SampleContext ctx, ElementSampleSession session)
        {
            var response = await ctx.Client.GetExchangeRevisionsAsync(session.Identifier);
            if (response.IsFailed)
                return new List<string>();
            return response.Value?
                .Select(r => r.Id)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToList() ?? new List<string>();
        }

        internal static async Task<string?> PullLatestAsync(SampleContext ctx, ElementDataModel model)
        {
            var response = await ctx.Client.RetrieveLatestExchangeAsync(model, CancellationToken.None);
            if (response.IsFailed)
            {
                var message = response.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                TerminalUi.Error($"Pull latest failed: {message}");
            return null;
            }

            if (string.IsNullOrWhiteSpace(response.Value))
                TerminalUi.Info("Already at latest revision.");
            else
                TerminalUi.Success($"Applied revision: {response.Value}");
            return response.Value;
        }

        internal static void PrintElementList(string heading, IEnumerable<Autodesk.DataExchange.Interface.IElement> elements)
        {
            var list = elements.ToList();
            TerminalUi.Chat($"  {heading}: {list.Count}");
            foreach (var element in list)
                TerminalUi.Chat($"    {element.Name} ({element.SourceId})");
        }

        internal static void PrintDeltaSummary(ElementDataModel model, IEnumerable<string> revisionIds)
        {
            var ids = revisionIds.ToList();
            PrintElementList("Created", model.GetCreatedElements(ids));
            PrintElementList("Modified", model.GetModifiedElements(ids));
            PrintElementList("Deleted", model.GetDeletedElements(ids));
        }

        internal static async Task<ElementDataModel?> LoadAtRevisionAsync(
            SampleContext ctx,
            DataExchangeIdentifier identifier,
            string? fromRevision,
            string? toRevision)
        {
            var response = await ctx.Client.GetElementDataModelAsync(identifier, fromRevision, toRevision);
            if (response.IsFailed)
            {
                var message = response.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                TerminalUi.Error($"Load at revision failed: {message}");
            return null;
            }

            return response.Value as ElementDataModel;
        }
    }
}
