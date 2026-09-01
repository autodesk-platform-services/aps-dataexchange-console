using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;

namespace ConsoleConnector.Common
{
    internal static class DesignSampleHelper
    {
        internal static async Task<ElementSampleSession?> BeginAsync(SampleContext ctx) =>
            await ElementSampleHelper.BeginAsync(ctx);

        internal static IElement CreateDefinitionWithMesh(ElementDataModel model, string elementId, string name)
        {
            var def = model.AddElement(elementId, name);
            ElementSampleHelper.ClassifyGeneric(model, def);
            var mesh = GeometrySampleHelper.CreateSampleMesh();
            var geometry = ElementDataModel.CreateMeshGeometry(mesh, $"{name}_Mesh");
            model.AddElementGeometry(def, new List<IElementGeometry> { geometry });
            return def;
        }

        internal static IElement CreateInstance(ElementDataModel model, string elementId, string name)
        {
            var instance = model.AddElement(elementId, name);
            ElementSampleHelper.ClassifyGeneric(model, instance);
            return instance;
        }

        internal static void PrintDesignSummary(ElementDataModel model, IDesign design)
        {
            var instances = model.GetDesignInstances(design).ToList();
            TerminalUi.Chat($"  Design: {design.Name} (id: {design.SourceId})");
            TerminalUi.Chat($"  Instances: {instances.Count}");
            foreach (var instance in instances)
                TerminalUi.Chat($"    {instance.Name} ({instance.SourceId})");
        }

        internal static (string DesignName, string DesignId) PromptDesignIdentity(string? defaultName = null, string? defaultId = null)
        {
            var name = Prompt.AskString("Design name", defaultName ?? "Chair");
            var id = Prompt.AskString("Design id", defaultId ?? "chair-id");
            return (name, id);
        }

        /// <summary>Creates a design ref from a new or existing definition element. Used by 6.1 and scenarios that need one.</summary>
        internal static async Task<(ElementSampleSession Session, IDesign Design)?> CreateOrGetDesignRefInteractiveAsync(SampleContext ctx, bool syncAfter)
        {
            var session = await BeginAsync(ctx);
            if (session == null)
                return null;

            var (designName, designId) = PromptDesignIdentity();
            var existing = session.Model.GetDesignRefs()
                .FirstOrDefault(d => string.Equals(d.SourceId, designId, StringComparison.OrdinalIgnoreCase));
            IDesign design;
            if (existing != null)
            {
                PrintDesignSummary(session.Model, existing);
                design = existing;
            }
            else
            {
                var defId = Prompt.AskString("Definition element id", $"def_{Guid.NewGuid():N}"[..10]);
                var defName = Prompt.AskString("Definition element name", designName);
                var def = CreateDefinitionWithMesh(session.Model, defId, defName);
                design = session.Model.GetOrCreateDesignRef(def, designName, designId);
                PrintDesignSummary(session.Model, design);
            }

            if (syncAfter)
                await ElementSampleHelper.SyncAsync(ctx, session);

            return (session, design);
        }

        /// <summary>Creates an instance element from a design ref (new or existing). Used by 6.6 and scenarios that need one.</summary>
        internal static async Task<(ElementSampleSession Session, IDesign Design)?> InstantiateDesignInteractiveAsync(SampleContext ctx, bool syncAfter)
        {
            var session = await BeginAsync(ctx);
            if (session == null)
                return null;

            var (designName, designId) = PromptDesignIdentity();
            var defId = Prompt.AskString("Definition element id (existing or new)", $"def_{designId}");
            var def = session.Model.GetElementsBySourceId(defId).FirstOrDefault();
            IDesign design;

            var existingDesign = session.Model.GetDesignRefs()
                .FirstOrDefault(d => string.Equals(d.SourceId, designId, StringComparison.OrdinalIgnoreCase));
            if (existingDesign != null)
            {
                design = existingDesign;
            }
            else if (def == null)
            {
                def = CreateDefinitionWithMesh(session.Model, defId, designName);
                design = session.Model.GetOrCreateDesignRef(def, designName, designId);
            }
            else
            {
                design = session.Model.GetOrCreateDesignRef(def, designName, designId);
            }

            var instanceId = Prompt.AskString("Instance element id", $"inst_{Guid.NewGuid():N}"[..10]);
            var instanceName = Prompt.AskString("Instance element name", $"{designName}@Site");
            var instance = CreateInstance(session.Model, instanceId, instanceName);
            var useBySourceId = Prompt.AskString("Use InstantiateDesignBySourceId? [y/N]", "N");
            if (string.Equals(useBySourceId, "y", StringComparison.OrdinalIgnoreCase))
                session.Model.InstantiateDesignBySourceId(designId, instance);
            else
                session.Model.InstantiateDesign(design, instance);
            PrintDesignSummary(session.Model, design);

            if (syncAfter)
                await ElementSampleHelper.SyncAsync(ctx, session);

            return (session, design);
        }
    }
}
