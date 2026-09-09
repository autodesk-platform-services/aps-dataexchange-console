using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using Autodesk.DataExchange.Models.Components;
using Autodesk.DataExchange.Models.Math;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Set element transform before attaching geometry.
    /// SDK: IElement.Transformation + SetElementGeometry.
    /// Console plumbing: GeometrySampleHelper.BeginAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 5, 2)]
    public sealed class SetElementTransformSample : ISample
    {
        public string Name => "Set Element Transform";
        public string Description => "Set element transformation before attaching geometry";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await GeometrySampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var element = await GeometrySampleHelper.PickOrCreateElementAsync(ctx, session.Model);
            if (element == null)
                return;
            element.Transformation = new Transformation
            {
                Matrix = new Matrix4d(new double[]
                {
                    1, 0, 0, 10,
                    0, 1, 0, 20,
                    0, 0, 1, 30,
                    0, 0, 0, 1,
                }),
            };

            var geometry = GeometrySampleHelper.CreateLineGeometry();
            session.Model.SetElementGeometry(element, new List<IElementGeometry> { geometry });
            TerminalUi.Success($"Set transform and attached geometry on {element.Name} ({element.SourceId}).");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
