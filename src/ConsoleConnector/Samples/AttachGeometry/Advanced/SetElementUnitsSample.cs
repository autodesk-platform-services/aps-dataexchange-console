using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using Autodesk.DataExchange.SchemaObjects.Units;
using Autodesk.GeometryUtilities.PrimitivesAPI;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Element length/display units affect geometry interpretation.
    /// SDK: IElement.LengthUnit + CreatePrimitiveGeometry.
    /// Console plumbing: GeometrySampleHelper.BeginAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 5, 3)]
    public sealed class SetElementUnitsSample : ISample
    {
        public string Name => "Set Element Units";
        public string Description => "Set element length/display units before attaching geometry";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await GeometrySampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var element = await GeometrySampleHelper.PickOrCreateElementAsync(ctx, session.Model);
            if (element == null)
                return;
            element.LengthUnit = UnitFactory.Meter;
            element.DisplayLengthUnit = UnitFactory.Meter;

            var units = new Autodesk.DataExchange.DataModels.Units(UnitFactory.Meter, UnitFactory.Meter, UnitFactory.Meter);
            var geometry = ElementDataModel.CreatePrimitiveGeometry(
                GeometrySampleHelper.CreateLineGeometry().Geometry,
                GeometrySampleHelper.DefaultRenderStyle,
                units);
            session.Model.AddElementGeometry(element, new System.Collections.Generic.List<IElementGeometry> { geometry });
            TerminalUi.Success($"Set meter units and attached geometry on {element.Name} ({element.SourceId}).");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
