using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.DataExchange;
using Autodesk.DataExchange.Core.Enums;
using Autodesk.DataExchange.Core.Models;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using Autodesk.DataExchange.Models;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;

namespace ConsoleConnector.Common
{
    /// <summary>
    /// Console plumbing for samples — session state for loaded/created exchanges.
    /// </summary>
    internal static class ExchangeSessionHelper
    {
        internal static DataExchangeIdentifier ToIdentifier(ExchangeDetails details, string? hubId) =>
            new()
            {
                ExchangeId = details.ExchangeID,
                CollectionId = details.CollectionID,
                HubId = details.HubId ?? hubId,
            };

        internal static async Task<bool> LoadFromDetailsAsync(
            SampleContext ctx,
            ExchangeDetails details,
            bool useEmptyModelForNewExchange = false)
        {
            if (string.IsNullOrWhiteSpace(details.FileUrn))
            {
                TerminalUi.Error("Created exchange has no file URN.");
                return false;
            }

            var displayName = details.DisplayName ?? details.FileUrn;
            ElementDataModel model;

            if (useEmptyModelForNewExchange)
            {
                TerminalUi.Info(
                    $"Preparing empty in-memory model for {displayName} " +
                    "(new exchanges have no Forma snapshot until after the first sync).");
                model = ElementDataModel.Create(ctx.Client);
            }
            else
            {
                var identifier = ToIdentifier(details, ctx.Folder?.HubId);

                try
                {
                    var loaded = await TryLoadModelWithStatusAsync(ctx, identifier, displayName);
                    if (loaded == null)
                    {
                        TerminalUi.Error("Could not load exchange into memory.");
                        return false;
                    }

                    model = loaded;
                }
                catch (Exception ex)
                {
                    TerminalUi.Error($"Failed to load exchange: {ex}");
                    return false;
                }
            }

            RegisterLoaded(ctx, details, model);
            return true;
        }

        internal static async Task<bool> CreateAndPrepareEmptyAsync(SampleContext ctx)
        {
            ctx.LastCreatedExchange = null;
            if (!await CreateNewExchangeAsync(ctx))
                return false;

            if (!await LoadFromDetailsAsync(ctx, ctx.LastCreatedExchange!, useEmptyModelForNewExchange: true))
                return false;

            ctx.ScenarioExchangeTitle = ctx.LastExchangeTitle;
            return true;
        }

        internal static void EndScenario(SampleContext ctx) => ctx.ScenarioExchangeTitle = null;

        /// <summary>Creates a new empty exchange in the session folder. Used by 2.1 Create Exchange and by scenarios that need a fresh exchange.</summary>
        internal static async Task<bool> CreateNewExchangeAsync(SampleContext ctx)
        {
            var folder = NavigationHelper.LoadFolderFromSession(ctx);
            if (folder == null)
                return false;

            if (ctx.Client is not Client client || client.SDKOptions?.HostingProvider == null)
                throw new InvalidOperationException("SDK client is not configured.");

            var fileName = Prompt.AskString("Exchange name", $"New Exchange_{DateTime.Now:yyyyMMdd_HHmmss}");
            if (string.IsNullOrWhiteSpace(fileName))
            {
                TerminalUi.Dim("Cancelled.");
                return false;
            }

            var request = new ExchangeCreateRequestACC
            {
                Host = client.SDKOptions.HostingProvider,
                Contract = client.SDKOptions.ContractProvider,
                FileName = fileName.Trim(),
                ACCFolderURN = folder.FolderUrn,
                ProjectId = folder.ProjectUrn,
                HubId = folder.HubId,
                Region = folder.Region,
                ProjectType = ProjectType.ACC,
                Description = string.Empty,
            };

            TerminalUi.Chat("Creating exchange...");
            var response = await ctx.Client.CreateExchangeAsync(request);
            if (response.IsFailed)
            {
                var message = response.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                TerminalUi.Error($"Failed: {message}");
                return false;
            }

            RememberCreated(ctx, response.Value, fileName);
            PrintCreated(response.Value);
            return true;
        }

        /// <summary>Picks a file from the session folder, resolves its details, and loads it. Used by 2.3 Load Exchange and scenarios that need a loaded exchange.</summary>
        internal static async Task<bool> LoadPickedExchangeAsync(SampleContext ctx)
        {
            var fileUrn = await NavigationHelper.PickExchangeFileUrnAsync(ctx);
            if (fileUrn == null)
                return false;

            ExchangeDetails details;
            try
            {
                details = default!;
                await TerminalUi.RunWithStatusAsync(
                    "Resolving exchange details…",
                    async () =>
                    {
                        // A freshly picked exchange is known only by its file URN; the collection id is
                        // discoverable only from the details themselves, so the single-arg (obsolete)
                        // lookup is the only resolver available for this bootstrap path.
#pragma warning disable CS0618 // Type or member is obsolete
                        details = await ctx.Client.GetExchangeDetailsAsync(fileUrn).ConfigureAwait(false);
#pragma warning restore CS0618
                    });
            }
            catch (Exception ex)
            {
                TerminalUi.Error($"Failed to resolve exchange: {ex}");
                return false;
            }

            return await LoadFromDetailsAsync(ctx, details);
        }

        /// <summary>Adds a demo line element to an exchange and syncs. Used by 2.2 Sync Exchange and scenarios that need a quick sync.</summary>
        internal static async Task<bool> SyncDemoLineAsync(SampleContext ctx, string? preferredTitle)
        {
            if (!NavigationHelper.EnsureFullFolder(ctx))
                return false;

            var active = LoadedExchangePicker.Pick(ctx, preferredTitle ?? ctx.ScenarioExchangeTitle);
            ExchangeDetails details;
            ElementDataModel model;

            if (active != null)
            {
                var detailsResponse = await ctx.Client
                    .GetExchangeDetailsAsync(active.CollectionId, active.ExchangeFileUrn)
                    .ConfigureAwait(false);
                if (detailsResponse.IsFailed)
                {
                    var detailsError = detailsResponse.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                    TerminalUi.Error($"Failed to resolve exchange: {detailsError}");
                    return false;
                }

                details = detailsResponse.Value;
                model = active.DataModel;
            }
            else if (ctx.Exchanges.Count == 0)
            {
                details = await ResolveDetailsForSyncAsync(ctx, preferredTitle).ConfigureAwait(false);
                if (details == null)
                {
                    TerminalUi.Dim("Cancelled.");
                    return false;
                }

                model = await GetModelForSyncAsync(ctx, details).ConfigureAwait(false);
                if (model == null)
                    return false;
            }
            else
            {
                TerminalUi.Dim("Cancelled.");
                return false;
            }

            var identifier = ToIdentifier(details, ctx.Folder!.HubId);

            var beforeCount = model.Elements.Count();
            TerminalUi.Info($"Syncing to {details.DisplayName ?? details.FileUrn}...");
            TerminalUi.Chat($"Elements before: {beforeCount}");

            var element = SampleDataFactory.CreateDemoLine(model);
            TerminalUi.Success($"Added element: {element.Name} ({element.SourceId})");

            var response = await ctx.Client.SyncExchangeDataAsync(identifier, model, CancellationToken.None);
            if (response.IsFailed)
            {
                var message = response.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                TerminalUi.Error($"Sync failed: {message}");
                return false;
            }

            RegisterLoaded(ctx, details, model);
            TerminalUi.Success("Sync complete.");
            TerminalUi.Chat($"Elements after: {model.Elements.Count()}");
            return true;
        }

        private static async Task<ExchangeDetails?> ResolveDetailsForSyncAsync(
            SampleContext ctx,
            string? preferredTitle)
        {
            if (ctx.LastCreatedExchange != null)
            {
                var createdTitle = ctx.LastExchangeTitle ?? ctx.LastCreatedExchange.DisplayName;
                if (string.IsNullOrWhiteSpace(preferredTitle)
                    || string.Equals(createdTitle, preferredTitle, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(createdTitle, ctx.ScenarioExchangeTitle, StringComparison.OrdinalIgnoreCase))
                {
                    TerminalUi.Info($"Using recently created exchange: {createdTitle}");
                    return ctx.LastCreatedExchange;
                }
            }

            var fileUrn = await NavigationHelper.PickExchangeFileUrnAsync(ctx).ConfigureAwait(false);
            if (fileUrn == null)
                return null;

            try
            {
                ExchangeDetails details = default!;
                await TerminalUi.RunWithStatusAsync(
                    "Resolving exchange details…",
                    async () =>
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        details = await ctx.Client.GetExchangeDetailsAsync(fileUrn).ConfigureAwait(false);
#pragma warning restore CS0618
                    }).ConfigureAwait(false);
                return details;
            }
            catch (Exception ex)
            {
                TerminalUi.Error($"Failed to resolve exchange: {ex}");
                return null;
            }
        }

        private static async Task<ElementDataModel?> GetModelForSyncAsync(
            SampleContext ctx,
            ExchangeDetails details)
        {
            var displayName = details.DisplayName ?? details.FileUrn;
            var identifier = ToIdentifier(details, ctx.Folder?.HubId);
            var loaded = await TryLoadModelWithStatusAsync(ctx, identifier, displayName).ConfigureAwait(false);
            if (loaded != null)
                return loaded;

            TerminalUi.Info(
                $"Preparing empty in-memory model for {displayName} " +
                "(new exchanges have no Forma snapshot until after the first sync).");
            return ElementDataModel.Create(ctx.Client);
        }

        internal static void RegisterLoaded(SampleContext ctx, ExchangeDetails details, ElementDataModel model)
        {
            var title = details.DisplayName ?? details.FileUrn;
            ctx.Exchanges[title] = new ActiveExchange(details.FileUrn, details.CollectionID, model);
            RememberLoaded(
                ctx,
                title,
                details.FileUrn,
                details.ExchangeID,
                details.CollectionID,
                details.HubId ?? ctx.Folder?.HubId);

            var elementCount = model.Elements.Count();
            TerminalUi.WriteResultPanel(
                "Loaded",
                ("Name", title),
                ("Exchange", details.ExchangeID),
                ("File URN", details.FileUrn),
                ("Elements", elementCount.ToString()));
        }

        internal static void RememberCreated(SampleContext ctx, ExchangeDetails details, string? fallbackName = null)
        {
            ctx.LastCreatedExchange = details;
            ctx.LastExchangeTitle = details.DisplayName ?? fallbackName;
        }

        internal static void PrintCreated(ExchangeDetails details)
        {
            TerminalUi.Chat("Created:");
            TerminalUi.Chat($"  Name:       {details.DisplayName}");
            TerminalUi.Chat($"  Exchange:   {details.ExchangeID}");
            TerminalUi.Chat($"  Collection: {details.CollectionID}");
            TerminalUi.Chat($"  File URN:   {details.FileUrn}");
            TerminalUi.Chat($"  Version:    {details.FileVersionUrn}");
        }

        internal static void RemoveLoadedByFileUrn(SampleContext ctx, string fileUrn)
        {
            var loaded = ctx.Exchanges
                .Where(pair => string.Equals(pair.Value.ExchangeFileUrn, fileUrn, StringComparison.OrdinalIgnoreCase))
                .Select(pair => pair.Key)
                .ToList();
            foreach (var title in loaded)
                ctx.Exchanges.Remove(title);

            ClearLastIfMatches(ctx, fileUrn);
        }

        private static async Task<ElementDataModel?> TryLoadModelWithStatusAsync(
            SampleContext ctx,
            DataExchangeIdentifier identifier,
            string displayName)
        {
            IResponse<IElementDataModel> response = default!;
            await TerminalUi.RunWithStatusAsync(
                $"Loading {displayName}…",
                async () => response = await ctx.Client.GetElementDataModelAsync(identifier).ConfigureAwait(false));

            return ToElementDataModel(response);
        }

        private static ElementDataModel? ToElementDataModel(IResponse<IElementDataModel> response)
        {
            if (response.IsFailed)
                return null;

            return response.Value as ElementDataModel;
        }

        internal static void RememberLoaded(
            SampleContext ctx,
            string title,
            string fileUrn,
            string exchangeId,
            string collectionId,
            string? hubId)
        {
            ctx.LastExchange = new LoadedExchangeInfo(title, fileUrn, exchangeId, collectionId, hubId);
            ctx.LastExchangeTitle = title;
        }

        internal static void ClearLast(SampleContext ctx)
        {
            ctx.LastExchange = null;
            ctx.LastExchangeTitle = null;
        }

        internal static void ClearLastIfMatches(SampleContext ctx, string fileUrn)
        {
            if (ctx.LastExchange != null
                && string.Equals(ctx.LastExchange.FileUrn, fileUrn, StringComparison.OrdinalIgnoreCase))
            {
                ClearLast(ctx);
            }
        }

        internal static async Task<bool> TryRestoreAsync(SampleContext ctx)
        {
            var info = ctx.LastExchange;
            if (info == null || ctx.Exchanges.ContainsKey(info.Title))
                return false;

            var identifier = new DataExchangeIdentifier
            {
                ExchangeId = info.ExchangeId,
                CollectionId = info.CollectionId,
                HubId = info.HubId ?? ctx.Folder?.HubId,
            };

            IResponse<IElementDataModel> response;
            try
            {
                response = await ctx.Client.GetElementDataModelAsync(identifier).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                TerminalUi.Warning($"Could not restore '{info.Title}': {ex}");
                ClearLast(ctx);
                return false;
            }

            if (response.IsFailed)
            {
                var message = response.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                TerminalUi.Warning($"Could not restore '{info.Title}': {message}");
                ClearLast(ctx);
                return false;
            }

            if (response.Value is not ElementDataModel model)
            {
                TerminalUi.Warning($"Could not restore '{info.Title}': unexpected data model type.");
                ClearLast(ctx);
                return false;
            }

            ctx.Exchanges[info.Title] = new ActiveExchange(info.FileUrn, info.CollectionId, model);
            ctx.LastExchangeTitle = info.Title;
            return true;
        }
    }
}
