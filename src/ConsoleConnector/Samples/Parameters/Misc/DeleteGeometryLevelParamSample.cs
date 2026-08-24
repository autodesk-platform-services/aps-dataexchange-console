using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to delete a geometry-level parameter.
    /// SDK: ElementGeometry.DeleteParameter.
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(5, 3, 6)]
    public sealed class DeleteGeometryLevelParamSample : ISample
    {
        public string Name => "Delete Geometry Level Param";
        public string Description => "Delete a geometry-level parameter";

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

            var geometry = await ParameterSampleHelper.ResolveGeometryAsync(ctx, session.Model, element);
            if (geometry == null)
            {
                TerminalUi.Warning("No geometry found.");
                return;
            }

            var parameters = geometry.Parameters.ToList();
            if (parameters.Count == 0)
            {
                TerminalUi.Warning("No geometry parameters to delete.");
                return;
            }

            var target = parameters[0];
            var deleted = geometry.DeleteParameter(target.SchemaId);
            TerminalUi.Error(deleted ? $"  Deleted '{target.Name}'." : "  Delete failed.");
            if (deleted)
                await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
