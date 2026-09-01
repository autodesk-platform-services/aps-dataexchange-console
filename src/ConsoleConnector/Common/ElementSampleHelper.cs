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
    /// Console plumbing for samples — loaded-exchange session, sync wrapper, element display.
    /// </summary>
    internal sealed record ElementSampleSession(
        ActiveExchange Active,
        ExchangeDetails Details,
        DataExchangeIdentifier Identifier,
        ElementDataModel Model);

    internal static class ElementSampleHelper
    {
        internal static bool EnsureFolder(SampleContext ctx) => NavigationHelper.EnsureFullFolder(ctx);

        internal static async Task<ElementSampleSession?> BeginAsync(SampleContext ctx)
        {
            if (!EnsureFolder(ctx))
                return null;

            var active = LoadedExchangePicker.Pick(ctx);
            if (active == null)
            {
                if (ctx.Exchanges.Count == 0)
                    TerminalUi.Warning("No loaded exchange. Run 2.3 Load Exchange first.");
                else
                    TerminalUi.Dim("Cancelled.");
                return null;
            }

            ExchangeDetails details;
            try
            {
                details = default!;
                await TerminalUi.RunWithStatusAsync(
                    "Resolving exchange details…",
                    async () =>
                    {
                        var response = await ctx.Client
                            .GetExchangeDetailsAsync(active.CollectionId, active.ExchangeFileUrn)
                            .ConfigureAwait(false);
                        if (response.IsFailed)
                            throw new InvalidOperationException(
                                response.Errors.FirstOrDefault()?.Message ?? "Failed to resolve exchange details.");
                        details = response.Value;
                    });
            }
            catch (Exception ex)
            {
                TerminalUi.Error($"Failed to resolve exchange: {ex}");
                return null;
            }

            var identifier = ExchangeSessionHelper.ToIdentifier(details, ctx.Folder!.HubId);
            return new ElementSampleSession(active, details, identifier, active.DataModel);
        }

        internal static void ClassifyGeneric(ElementDataModel model, IElement element)
        {
            var category = model.Classify(element, ClassificationSystem.Category, "Generics");
            var family = model.Classify(element, ClassificationSystem.Family, "Generic", parent: category);
            var type = model.DefineType("Type", "ConsoleConnector sample", parent: family);
            model.SetType(element, type);
        }

        internal static async Task<bool> SyncAsync(SampleContext ctx, ElementSampleSession session)
        {
            var title = session.Details.DisplayName ?? session.Active.ExchangeFileUrn;
            var success = false;
            await TerminalUi.RunWithStatusAsync(
                $"Syncing {title}…",
                async () =>
                {
                    var response = await ctx.Client.SyncExchangeDataAsync(
                        session.Identifier,
                        session.Model,
                        CancellationToken.None);
                    if (response.IsFailed)
                    {
                        var message = response.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                        TerminalUi.Error($"Sync failed: {message}");
                        return;
                    }

                    TerminalUi.Success("Sync complete.");
                    success = true;
                });

            if (success)
                await ExchangeSessionHelper.RefreshExchangeVersionAfterSyncAsync(ctx, session).ConfigureAwait(false);

            return success;
        }

        internal static void PrintElementSummary(IElement element, string prefix = "    ")
        {
            TerminalUi.Chat($"{prefix}{element.Name} ({element.SourceId})");
            TerminalUi.Chat($"{prefix}  Category: {element.Category ?? "(none)"}");
            TerminalUi.Chat($"{prefix}  Family:   {element.Family ?? "(none)"}");
            TerminalUi.Chat($"{prefix}  Type:     {element.Type?.Value ?? "(none)"}");
            TerminalUi.Chat($"{prefix}  HasGeometry: {element.HasGeometry}");
        }

        /// <summary>Adds a classified root element to an already-loaded model. Used by 3.1 Add Root Element and scenarios that need one.</summary>
        internal static IElement? AddRootElement(ElementDataModel model)
        {
            var beforeCount = model.Elements.Count();
            var elementId = Prompt.AskString("Element id", $"Root_{Guid.NewGuid():N}"[..12]);
            var name = Prompt.AskString("Element name", "Sample Root Element");
            if (string.IsNullOrWhiteSpace(elementId) || string.IsNullOrWhiteSpace(name))
            {
                TerminalUi.Chat("Element id and name are required.");
                return null;
            }

            if (model.GetElementsBySourceId(elementId).Any())
            {
                TerminalUi.Chat($"Element id already exists: {elementId}");
                return null;
            }

            TerminalUi.Chat($"Elements before: {beforeCount}");
            var element = model.AddElement(elementId, name);
            ClassifyGeneric(model, element);
            TerminalUi.Chat($"Added: {element.Name} ({element.SourceId})");
            return element;
        }

        /// <summary>Adds a classified child element under a picked parent. Used by 3.2 Add Child Element and scenarios that need one.</summary>
        internal static IElement? AddChildElement(SampleContext ctx, ElementSampleSession session)
        {
            var model = session.Model;
            if (!model.Elements.Any())
            {
                TerminalUi.Warning("No elements to use as parent. Run 3.1 Add Root Element first.");
                return null;
            }

            var parent = ctx.ScenarioExchangeTitle != null
                ? ElementPicker.PickTopLevel(model, "Pick a parent element")
                : ElementPicker.Pick(model, "Pick a parent element");
            if (parent == null)
            {
                TerminalUi.Dim("Cancelled.");
                return null;
            }

            var elementId = Prompt.AskString("Child element id", $"Child_{Guid.NewGuid():N}"[..12]);
            var name = Prompt.AskString("Child name", "Sample Child Element");
            if (string.IsNullOrWhiteSpace(elementId) || string.IsNullOrWhiteSpace(name))
            {
                TerminalUi.Chat("Element id and name are required.");
                return null;
            }

            if (model.GetElementsBySourceId(elementId).Any())
            {
                TerminalUi.Chat($"Element id already exists: {elementId}");
                return null;
            }

            TerminalUi.Info($"Adding child under {parent.Name} ({parent.SourceId})...");
            var child = model.AddElement(elementId, name, parent);
            ClassifyGeneric(model, child);
            TerminalUi.Chat($"Added: {child.Name} ({child.SourceId})");
            return child;
        }

        /// <summary>Adds a root element and attaches STEP geometry to it. Used by 11.2/11.7/11.10 scenarios.</summary>
        internal static async Task<bool> CreateRootElementWithStepGeometryAsync(SampleContext ctx, bool syncAfter)
        {
            var session = await BeginAsync(ctx);
            if (session == null)
                return false;

            if (AddRootElement(session.Model) == null)
                return false;

            if (!await GeometrySampleHelper.AttachFileGeometryAsync(
                    ctx, GeometryFormat.Step, "STEP path", ctx.Defaults.StepPath, syncAfter: false, "nist_ftc_09_asme1_rd.stp"))
                return false;

            if (!syncAfter)
                return true;

            return await SyncAsync(ctx, session);
        }
    }
}
