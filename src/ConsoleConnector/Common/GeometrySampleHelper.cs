using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.DataExchange;using Autodesk.DataExchange.Core.Enums;
using Autodesk.DataExchange.Core.Models;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using Autodesk.DataExchange.Models;
using Autodesk.DataExchange.SchemaObjects.Units;
using Autodesk.GeometryUtilities.MeshAPI;
using MeshApiMesh = Autodesk.GeometryUtilities.MeshAPI.Mesh;
using Autodesk.GeometryUtilities.PrimitivesAPI;
using Autodesk.GeometryUtilities.PrimitivesAPI.DX;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;

namespace ConsoleConnector.Common
{
    internal static class GeometrySampleHelper
    {
        internal static readonly RenderStyle DefaultRenderStyle =
            new("ConsoleConnector", new RGBA(0, 120, 215, 255), 1);

        internal static Autodesk.DataExchange.DataModels.Units DefaultUnits =>
            new(UnitFactory.Centimeter, UnitFactory.Centimeter, UnitFactory.Centimeter);

        internal static Client RequireClient(SampleContext ctx)
        {
            if (ctx.Client is Client client && client.SDKOptions != null)
                return client;

            throw new InvalidOperationException("SDK client is not configured.");
        }

        internal static GeometryConfiguration RequireGeometryConfiguration(SampleContext ctx) =>
            RequireClient(ctx).SDKOptions!.GeometryConfiguration;

        internal static async Task<ElementSampleSession?> BeginAsync(SampleContext ctx) =>
            await ElementSampleHelper.BeginAsync(ctx);

        internal static string ResolvePath(SampleContext ctx, string label, string? sessionDefault, params string[] assetFileNames)
        {
            var fileName = assetFileNames[^1];
            var fallback = sessionDefault ?? FindAssetFile(fileName);
            var path = Prompt.AskString(label, fallback);
            if (!string.IsNullOrWhiteSpace(path))
            {
                if (label.IndexOf("STEP", StringComparison.OrdinalIgnoreCase) >= 0)
                    ctx.Defaults.StepPath = path;
                else if (label.IndexOf("IFC", StringComparison.OrdinalIgnoreCase) >= 0)
                    ctx.Defaults.IfcPath = path;
                else if (label.IndexOf("OBJ", StringComparison.OrdinalIgnoreCase) >= 0)
                    ctx.Defaults.ObjPath = path;
            }

            return path;
        }

        internal static async Task<bool> AttachFileGeometryAsync(
            SampleContext ctx,
            GeometryFormat format,
            string pathLabel,
            string? sessionDefault,
            params string[] repoRelativeParts) =>
            await AttachFileGeometryAsync(ctx, format, pathLabel, sessionDefault, syncAfter: true, repoRelativeParts);

        internal static async Task<bool> AttachFileGeometryAsync(
            SampleContext ctx,
            GeometryFormat format,
            string pathLabel,
            string? sessionDefault,
            bool syncAfter,
            params string[] repoRelativeParts)
        {
            var session = await BeginAsync(ctx);
            if (session == null)
                return false;

            var path = ResolvePath(ctx, pathLabel, sessionDefault, repoRelativeParts);
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                TerminalUi.Warning($"File not found: {path}");
            return false;
            }

            var element = await PickOrCreateElementAsync(ctx, session.Model);
            if (element == null)
                return false;

            IElementGeometry geometry = default!;
            await TerminalUi.RunWithStatusAsync(
                $"Reading {format} file…",
                async () => geometry = await Task.Run(() =>
                    ElementDataModel.CreateFileGeometry(path, format, DefaultRenderStyle, DefaultUnits)).ConfigureAwait(false));
            session.Model.SetElementGeometry(element, new List<IElementGeometry> { geometry });
            TerminalUi.Success($"Attached {format} geometry from file to {element.Name} ({element.SourceId}).");
            if (!syncAfter)
                return true;

            return await ElementSampleHelper.SyncAsync(ctx, session);
        }

        internal static async Task<bool> AttachStreamGeometryAsync(
            SampleContext ctx,
            GeometryFormat format,
            string pathLabel,
            string? sessionDefault,
            params string[] repoRelativeParts)
        {
            var session = await BeginAsync(ctx);
            if (session == null)
                return false;

            var path = ResolvePath(ctx, pathLabel, sessionDefault, repoRelativeParts);
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                TerminalUi.Warning($"File not found: {path}");
            return false;
            }

            var element = await PickOrCreateElementAsync(ctx, session.Model);
            if (element == null)
                return false;

            await TerminalUi.RunWithStatusAsync(
                $"Reading {format} file into memory…",
                async () =>
                {
                    using var stream = File.OpenRead(path);
                    using var memory = new MemoryStream();
                    await stream.CopyToAsync(memory).ConfigureAwait(false);
                    memory.Position = 0;

                    var geometry = ElementDataModel.CreateFileGeometry(memory, format, DefaultRenderStyle, DefaultUnits);

                    // CreateFileGeometry(MemoryStream) persists the bytes to an extension-less temp file.
                    // GUSDK ConvertSources (run during SyncExchangeDataAsync) detects format from the
                    // file path extension, not GeometryFormat, so sync fails with
                    // "File conversion from Unknown to SMB are not supported".
                    // The stream bytes are unchanged; point FilePath at the source file so GUSDK sees .stp/.ifc/.obj.
                    if (geometry is FileGeometry fileGeometry)
                        fileGeometry.FilePath = path;

                    session.Model.SetElementGeometry(element, new List<IElementGeometry> { geometry });
                });
            TerminalUi.Success($"Attached {format} geometry from stream to {element.Name} ({element.SourceId}).");
            return await ElementSampleHelper.SyncAsync(ctx, session);
        }

        internal static async Task<bool> AttachPrimitiveAsync(
            SampleContext ctx,
            Func<PrimitiveGeometry> geometryFactory,
            string geometryLabel,
            bool syncAfter = true)
        {
            var session = await BeginAsync(ctx);
            if (session == null)
                return false;

            var element = await PickOrCreateElementAsync(ctx, session.Model);
            if (element == null)
                return false;

            var geometry = geometryFactory();
            session.Model.SetElementGeometry(element, new List<IElementGeometry> { geometry });
            TerminalUi.Success($"Attached {geometryLabel} to {element.Name} ({element.SourceId}).");
            if (!syncAfter)
                return true;

            return await ElementSampleHelper.SyncAsync(ctx, session);
        }

        internal static async Task<bool> AttachMeshAsync(SampleContext ctx, MeshApiMesh mesh, string meshName)
        {
            var session = await BeginAsync(ctx);
            if (session == null)
                return false;

            var element = await PickOrCreateElementAsync(ctx, session.Model);
            if (element == null)
                return false;

            var geometry = ElementDataModel.CreateMeshGeometry(mesh, meshName, DefaultUnits);
            session.Model.SetElementGeometry(element, new List<IElementGeometry> { geometry });
            TerminalUi.Success($"Attached mesh '{meshName}' to {element.Name} ({element.SourceId}).");
            return await ElementSampleHelper.SyncAsync(ctx, session);
        }

        internal static Task<IElement?> PickOrCreateElementAsync(SampleContext ctx, ElementDataModel model)
        {
            var topLevel = model.TopLevelElements.ToList();
            if (ctx.ScenarioExchangeTitle != null && topLevel.Count == 1)
            {
                var only = topLevel[0];
                TerminalUi.Chat($"Element: {only.Name} ({only.SourceId})");
                return Task.FromResult<IElement?>(only);
            }

            if (model.Elements.Any())
            {
                var choice = ListPicker.PickBinary(
                    "Target element",
                    "Pick existing element",
                    "Create new root element");
                if (choice == 0)
                    return Task.FromResult<IElement?>(null);
                if (choice == 1)
                {
                    var picked = ElementPicker.Pick(model, "Element");
                    if (picked != null)
                        return Task.FromResult<IElement?>(picked);
                }
            }

            var elementId = Prompt.AskString("New element id", $"Geom_{Guid.NewGuid():N}"[..12]);
            var name = Prompt.AskString("New element name", "Geometry Sample Element");
            if (string.IsNullOrWhiteSpace(elementId) || string.IsNullOrWhiteSpace(name))
            {
                TerminalUi.Warning("Element id and name are required.");
            return Task.FromResult<IElement?>(null);
        }

            if (model.GetElementsBySourceId(elementId).Any())
            {
                TerminalUi.Warning($"Element id already exists: {elementId}");
            return Task.FromResult<IElement?>(null);
        }

            var element = model.AddElement(elementId, name);
            ElementSampleHelper.ClassifyGeneric(model, element);
            return Task.FromResult<IElement?>(element);
        }

        internal static PrimitiveGeometry CreateLineGeometry()
        {
            var container = new GeometryContainer();
            var line = new Line(
                new Point3d { X = 200, Y = 200, Z = 200 },
                new Vector3d { X = 100, Y = 400, Z = 300 });
            line.Range = new ParamRange
            {
                High = 3.5,
                Low = 0,
                Type = ParamRange.RangeType.Finite,
            };
            container.Curves.Add(line);
            return ElementDataModel.CreatePrimitiveGeometry(container, DefaultRenderStyle, DefaultUnits);
        }

        internal static PrimitiveGeometry CreatePointGeometry()
        {
            var point = new DesignPoint(10.0, 10.0, 10.0);
            return ElementDataModel.CreatePrimitiveGeometry(point, DefaultRenderStyle, DefaultUnits);
        }

        internal static PrimitiveGeometry CreateSingleCurveGeometry<T>(Func<T> factory)
            where T : Curve
        {
            var container = new GeometryContainer();
            container.Curves.Add(factory());
            return ElementDataModel.CreatePrimitiveGeometry(container, DefaultRenderStyle, DefaultUnits);
        }

        internal static PrimitiveGeometry CreateCombinedPrimitiveGeometry()
        {
            var container = new GeometryContainer();
            var line = new Line(
                new Point3d { X = 200, Y = 200, Z = 200 },
                new Vector3d { X = 100, Y = 400, Z = 300 });
            line.Range = new ParamRange
            {
                High = 3.5,
                Low = 0,
                Type = ParamRange.RangeType.Finite,
            };
            container.Curves.Add(line);
            container.Points.Add(new Point { Position = new Point3d(0.1, 1.1, 1.0) });
            return ElementDataModel.CreatePrimitiveGeometry(container, DefaultRenderStyle, DefaultUnits);
        }

        internal static MeshApiMesh CreateSampleMesh() =>
            new()
            {
                Vertices = new List<Vertex> { new(0.0, 0.0, 0.0), new(1.0, 0.0, 0.0), new(0.0, 1.0, 0.0) },
                Faces = new List<Face>
                {
                    new()
                    {
                        Corners = new List<int> { 0, 1, 2 },
                        Normals = new List<Normal> { new(0, 0, 1) },
                    },
                },
            };

        internal static void PrintGeometryCounts(Dictionary<GeometryFilters, long> counts)
        {
            foreach (var pair in counts.OrderBy(p => p.Key.ToString()))
                TerminalUi.Chat($"    {pair.Key}: {pair.Value}");
        }

        internal static string? FindAssetFile(string fileName)
        {
            var bundled = Path.Combine(AppContext.BaseDirectory, "Assets", fileName);
            return File.Exists(bundled) ? bundled : null;
        }
    }
}
