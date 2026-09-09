using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to update a geometry-level parameter.
    /// SDK: ElementGeometry.UpdateParameter.
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(5, 3, 5)]
    public sealed class UpdateGeometryLevelParamSample : ISample
    {
        public string Name => "Update Geometry Level Param";
        public string Description => "Update a geometry-level parameter";

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
                TerminalUi.Warning("No geometry found. Run 5.3.4 first.");
                return;
            }

            var parameters = geometry.Parameters.ToList();
            if (parameters.Count == 0)
            {
                TerminalUi.Warning("No geometry parameters. Run 5.3.4 first.");
                return;
            }

            ParameterSampleHelper.PrintParameters(parameters, "Geometry parameters");
            var target = parameters[0];
            ParameterSampleHelper.PrintParameter(target, "  Before: ");
            var currentValue = target.Value?.ToString() ?? "v1";
            var suggested = currentValue;
            if (currentValue.Length > 1 && currentValue[0] == 'v')
            {
                var suffix = currentValue.Substring(1);
                if (int.TryParse(suffix, out int parsed))
                    suggested = "v" + (parsed + 1);
            }

            var newValue = Prompt.AskString("New value (string)", suggested);
            var updated = geometry.UpdateParameter(target.SchemaId, newValue);
            if (updated == null)
            {
                TerminalUi.Error($"Update failed for '{target.Name}'. New value type must match the parameter.");
                return;
            }

            ParameterSampleHelper.PrintParameter(updated, "  After:  ");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
