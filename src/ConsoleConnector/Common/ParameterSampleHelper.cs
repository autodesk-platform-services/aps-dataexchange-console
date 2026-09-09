using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using Autodesk.Parameters;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;

namespace ConsoleConnector.Common
{
    internal static class ParameterSampleHelper
    {
        /// <summary>
        /// Type name assigned by <see cref="ElementSampleHelper.ClassifyGeneric"/>.
        /// </summary>
        internal const string DefaultTypeName = "ConsoleConnector sample";
        internal const string DefaultBuiltInSchema = "autodesk.revit.parameter:curveElemLength-1.0.0";

        internal static async Task<ElementSampleSession?> BeginAsync(SampleContext ctx) =>
            await ElementSampleHelper.BeginAsync(ctx);

        internal static string DefaultBuiltInSchemaId(SampleContext ctx) =>
            string.IsNullOrWhiteSpace(ctx.Defaults.BuiltInSchemaId)
                ? DefaultBuiltInSchema
                : ctx.Defaults.BuiltInSchemaId!;

        internal static IElement? PickElement(ElementDataModel model, string prompt = "Pick element") =>
            ElementPicker.Pick(model, prompt);

        /// <summary>
        /// Lists type design assets in the model and prompts for one. Returns null when none exist.
        /// </summary>
        internal static string? PickTypeName(ElementDataModel model, string prompt = "Pick type")
        {
            var types = model.GetTypeParameters().Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToList();
            if (types.Count == 0)
            {
                var elementTypes = model.Elements
                    .Select(e => e.Type?.Value)
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            TerminalUi.Warning("No type design assets in this exchange.");
            if (elementTypes.Count > 0)
                {
                    TerminalUi.Chat("  Elements show these type labels (not registered for type params):");
            foreach (var label in elementTypes)
                        TerminalUi.Chat($"    - {label}");
        }

                TerminalUi.Info("Type params need a type design asset. Run 3.1 Add Root Element or attach geometry (4.x)");
            TerminalUi.Info("— both call ClassifyGeneric and create type 'ConsoleConnector sample' — then sync (2.2).");
            return null;
            }

            if (types.Count == 1)
            {
                TerminalUi.Info($"Using type '{types[0]}'.");
            return types[0];
            }

            var picked = ListPicker.PickOne(
                prompt,
                types,
                t => ListPicker.FormatNameAndDetail(t, null));
            if (picked != null)
                return picked;

            var typed = Prompt.AskString($"{prompt} (or type name, Enter to cancel)", null);
            if (string.IsNullOrWhiteSpace(typed))
                return null;

            var match = types.FirstOrDefault(t => string.Equals(t, typed.Trim(), StringComparison.OrdinalIgnoreCase));
            if (match != null)
                return match;

            TerminalUi.Warning($"Unknown type '{typed}'. Pick from the list or enter an exact name.");
            return null;
        }

        internal static Parameter CreateCustomParameter(string name, ParameterDataType value, string groupId)
        {
            var parameter = new Parameter(name, value)
            {
                IsCustomParameter = true,
                SampleText = string.Empty,
                Description = string.Empty,
                ReadOnly = false,
                GroupID = groupId,
            };
            return parameter;
        }

        internal static Parameter CreateBuiltInParameter(string schemaId, ParameterDataType value) =>
            new Parameter(new ParameterSchemaId(schemaId), value);

        internal static async Task<IParameter?> AddBuiltInInstanceParamAsync(IElement element, string schemaId, ParameterDataType value)
        {
            var parameter = CreateBuiltInParameter(schemaId, value);
            var added = await element.CreateInstanceParameterAsync(parameter);
            return added;
        }

        internal static async Task<IParameter?> AddCustomInstanceParamAsync(IElement element, string name, ParameterDataType value, string groupId)
        {
            var parameter = CreateCustomParameter(name, value, groupId);
            return await element.CreateInstanceParameterAsync(parameter);
        }

        /// <summary>
        /// Adds a custom instance param, or returns the existing one with a console hint (no throw on duplicate name).
        /// </summary>
        internal static async Task<IParameter?> EnsureCustomInstanceParamAsync(
            IElement element,
            string name,
            ParameterDataType createValue,
            string groupId)
        {
            var existing = element.FindInstanceParameter(name);
            if (existing != null)
            {
                TerminalUi.Info($"'{name}' already on this element — skipping add, will update.");
            return existing;
            }

            if (element is not Element concrete)
                return null;

            TerminalUi.Info($"Adding '{name}'...");
            return await AddCustomInstanceParamAsync(concrete, name, createValue, groupId);
        }

        internal static async Task<IParameter?> AddCustomTypeParamAsync(ElementDataModel model, string typeName, string name, ParameterDataType value, string groupId)
        {
            var existing = model.GetTypeParameters(typeName)
                .SelectMany(pair => pair.Value)
                .FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                TerminalUi.Info($"'{name}' already exists on type '{typeName}' — skipping add.");
                return existing;
            }

            var parameter = CreateCustomParameter(name, value, groupId);
            return await model.CreateTypeParameterAsync(typeName, parameter);
        }

        internal static string GeneralGroupId => Group.General.DisplayName();

        internal static string DimensionsGroupId => Group.Dimensions.DisplayName();

        internal static string SuggestUniqueCustomParamName(IElement element, string baseName = "DoorLength")
        {
            var existing = element.InstanceParameters
                .Select(p => p.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!existing.Contains(baseName))
                return baseName;

            for (var i = 2; i < 1000; i++)
            {
                var candidate = $"{baseName}{i}";
                if (!existing.Contains(candidate))
                    return candidate;
            }

            return $"{baseName}_{Guid.NewGuid():N}"[..16];
        }

        internal static string SuggestUniqueModelParamName(ElementDataModel model, string baseName)
        {
            var existing = model.Parameters
                .Select(p => p.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!existing.Contains(baseName))
                return baseName;

            for (var i = 2; i < 1000; i++)
            {
                var candidate = $"{baseName}{i}";
                if (!existing.Contains(candidate))
                    return candidate;
            }

            return $"{baseName}_{Guid.NewGuid():N}"[..16];
        }

        internal static IParameter PickUpdatableInstanceParam(IReadOnlyList<IParameter> parameters) =>
            parameters.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Name) && p.Value is double)
            ?? parameters.FirstOrDefault(p => p.Value is bool)
            ?? parameters.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Name))
            ?? parameters[0];

        internal static IParameter PickNamedTypeParam(IReadOnlyList<IParameter> parameters) =>
            parameters.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Name))
            ?? parameters[0];

        internal static ParameterDataType SuggestUpdatedValue(IParameter parameter) =>
            parameter.Value switch
            {
                double d => d + 1.0,
                bool b => !b,
                long l => l + 1L,
                int i => i + 1,
                string s => s + "-updated",
                _ => 9.6,
            };

        internal static string GraphicsGroupId => Group.Graphics.DisplayName();

        internal static async Task<IParameter?> AddModelLevelParamAsync(ElementDataModel model, string name, ParameterDataType value)
        {
            var parameter = CreateCustomParameter(name, value, GeneralGroupId);
            return await model.AddParameterAsync(parameter);
        }

        internal static async Task<ElementGeometry?> AttachMinimalGeometryAsync(ElementDataModel model, IElement element)
        {
            var mesh = GeometrySampleHelper.CreateSampleMesh();
            var geometry = ElementDataModel.CreateMeshGeometry(mesh, $"{element.Name}_Mesh");
            model.AddElementGeometry(element, new List<IElementGeometry> { geometry });
            return geometry;
        }

        internal static void PrintParameter(IParameter parameter, string prefix = "    ")
        {
            TerminalUi.Chat($"{prefix}{parameter.Name} = {parameter.Value}");
            TerminalUi.Chat($"{prefix}  SchemaId: {parameter.SchemaId}");
            if (!string.IsNullOrWhiteSpace(parameter.GroupID))
                TerminalUi.Chat($"{prefix}  Group: {parameter.GroupID}");
        }

        internal static void PrintParameters(IEnumerable<IParameter> parameters, string heading)
        {
            var list = parameters.ToList();
            TerminalUi.Chat($"  {heading}: {list.Count}");
            foreach (var parameter in list)
                PrintParameter(parameter);
        }

        internal static async Task<ElementGeometry?> ResolveGeometryAsync(
            SampleContext ctx,
            ElementDataModel model,
            IElement element)
        {
            if (!element.HasGeometry)
                return await AttachMinimalGeometryAsync(model, element);

            try
            {
                Dictionary<IElement, IEnumerable<IElementGeometry>> map = default!;
                await TerminalUi.RunWithStatusAsync(
                    "Retrieving element geometry…",
                    async () => map = await model.GetElementGeometriesAsync(new[] { element }, CancellationToken.None).ConfigureAwait(false));
                if (map.TryGetValue(element, out var geometries))
                    return geometries.FirstOrDefault() as ElementGeometry;
            }
            catch (Exception ex)
            {
                TerminalUi.Warning($"Could not retrieve geometry ({ex}); using sample mesh.");
            }

            return await AttachMinimalGeometryAsync(model, element);
        }

        internal static void PersistBuiltInSchemaId(SampleContext ctx, string schemaId)
        {
            if (!string.IsNullOrWhiteSpace(schemaId))
                ctx.Defaults.BuiltInSchemaId = schemaId;
        }

        internal static async Task<bool> DemoAllDataTypesAsync(IElement element)
        {
            if (element is not Element concrete)
                return false;

            var existingDemo = DemoParamNames.Count(n => concrete.FindInstanceParameter(n) != null);
            if (existingDemo == DemoParamNames.Length)
            {
                TerminalUi.Chat("All four demo params (TypeBool, TypeInt64, TypeFloat64, TypeString) are already on this element.");
                TerminalUi.Info("Will update their values only. Pick another element to see the add path.");
            }
            else if (existingDemo > 0)
            {
                TerminalUi.Info($"{existingDemo} of 4 demo params already present — missing names will be added, then all four updated.");
            }
            else
            {
                TerminalUi.Info("Will add TypeBool, TypeInt64, TypeFloat64, TypeString, then update each.");
            }

            var boolParam = await EnsureCustomInstanceParamAsync(concrete, "TypeBool", true, GeneralGroupId);
            var longParam = await EnsureCustomInstanceParamAsync(concrete, "TypeInt64", 123456789L, GeneralGroupId);
            var doubleParam = await EnsureCustomInstanceParamAsync(
                concrete, "TypeFloat64", 123.45, DimensionsGroupId);
            var stringParam = await EnsureCustomInstanceParamAsync(
                concrete, "TypeString", "initial", GraphicsGroupId);
            if (boolParam == null || longParam == null || doubleParam == null || stringParam == null)
            {
                TerminalUi.Chat("Could not resolve all four demo parameters.");
                return false;
            }

            TerminalUi.Chat("Updating values (bool→false, long→987654321, double→678.90, string→updated)...");
            concrete.UpdateInstanceParameter(boolParam.SchemaId, false);
            concrete.UpdateInstanceParameter(longParam.SchemaId, 987654321L);
            concrete.UpdateInstanceParameter(doubleParam.SchemaId, 678.90);
            concrete.UpdateInstanceParameter(stringParam.SchemaId, "updated");
            PrintParameters(concrete.InstanceParameters, "After updates");
            return true;
        }

        private static readonly string[] DemoParamNames = { "TypeBool", "TypeInt64", "TypeFloat64", "TypeString" };

        /// <summary>Prompts for a custom instance param name/value and adds it. Used by 5.1.2 and scenarios that need one.</summary>
        internal static async Task<IParameter?> AddCustomInstanceParamInteractiveAsync(IElement element)
        {
            var defaultName = SuggestUniqueCustomParamName(element);
            var name = Prompt.AskString("Parameter name", defaultName);
            var valueText = Prompt.AskString("Value (number)", "4.52");
            if (string.IsNullOrWhiteSpace(name) || !double.TryParse(valueText, out var value))
            {
                TerminalUi.Chat("Name and numeric value are required.");
                return null;
            }

            var added = await AddCustomInstanceParamAsync(element, name, value, DimensionsGroupId);
            if (added == null)
            {
                TerminalUi.Error("Failed to add parameter.");
                return null;
            }

            PrintParameter(added, "  ");
            return added;
        }

        /// <summary>Prompts for a built-in schema id/value and adds it. Used by 5.1.1 and scenarios that need one.</summary>
        internal static async Task<IParameter?> AddBuiltInInstanceParamInteractiveAsync(SampleContext ctx, IElement element)
        {
            var schemaId = Prompt.AskString("Built-in schema id", DefaultBuiltInSchemaId(ctx));
            var valueText = Prompt.AskString("Value (number)", "5.345");
            if (!double.TryParse(valueText, out var value))
            {
                TerminalUi.Error("Invalid numeric value.");
                return null;
            }

            var added = await AddBuiltInInstanceParamAsync(element, schemaId, value);
            if (added == null)
            {
                TerminalUi.Error("Failed to add parameter.");
                return null;
            }

            PersistBuiltInSchemaId(ctx, schemaId);
            PrintParameter(added, "  ");
            return added;
        }

        /// <summary>Picks an updatable instance param and applies a suggested new value. Used by 5.1.3 and scenarios that need one.</summary>
        internal static IParameter? UpdateInstanceParamInteractive(IElement element)
        {
            var parameters = element.InstanceParameters.ToList();
            if (parameters.Count == 0)
            {
                TerminalUi.Warning("No instance parameters on this element. Run 5.1.1 or 5.1.2 first.");
                return null;
            }

            PrintParameters(parameters, "Instance parameters");
            var target = PickUpdatableInstanceParam(parameters);
            var paramName = Prompt.AskString("Parameter name to update", target.Name);
            var param = string.IsNullOrWhiteSpace(paramName)
                ? target
                : element.FindInstanceParameter(paramName) ?? target;
            if (param == null)
            {
                TerminalUi.Error($"Parameter not found: {paramName}");
                return null;
            }

            if (element is not Element concrete)
            {
                TerminalUi.Chat("Unexpected element type.");
                return null;
            }

            var newValue = SuggestUpdatedValue(param);
            var updated = concrete.UpdateInstanceParameter(param.SchemaId, newValue);
            PrintParameter(updated, "  Updated: ");
            return updated;
        }
    }
}
