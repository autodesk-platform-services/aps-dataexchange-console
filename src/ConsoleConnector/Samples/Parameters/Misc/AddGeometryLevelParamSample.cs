using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Parameters can live on geometry, not just elements.
    /// SDK: ElementGeometry.CreateParameter.
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(5, 3, 4)]
    public sealed class AddGeometryLevelParamSample : ISample
    {
        public string Name => "Add Geometry Level Param";
        public string Description => "Add a parameter on element geometry";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ParameterSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var element = ParameterSampleHelper.PickElement(session.Model);
            if (element == null)
            {
                TerminalUi.Dim("Cancelled.");
                return;
            }

            ElementGeometry? geometry;
            if (!element.HasGeometry)
            {
                TerminalUi.Warning("Element has no geometry; attaching sample mesh...");
            geometry = await ParameterSampleHelper.AttachMinimalGeometryAsync(session.Model, element);
        }
            else
            {
                geometry = await ParameterSampleHelper.ResolveGeometryAsync(ctx, session.Model, element);
        }

            if (geometry == null)
            {
                TerminalUi.Warning("No geometry available.");
                return;
            }

            var added = await geometry.CreateParameter(new Parameter("GeomTag", "v1"));
            ParameterSampleHelper.PrintParameter(added, "  ");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
