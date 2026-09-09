using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to delete one instance parameter.
    /// SDK: Element.DeleteInstanceParameter.
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(5, 1, 4)]
    public sealed class DeleteInstanceParamSample : ISample
    {
        public string Name => "Delete Instance Param";
        public string Description => "Delete an instance parameter from an element";

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

            var parameters = element.InstanceParameters.ToList();
            if (parameters.Count == 0)
            {
                TerminalUi.Warning("No instance parameters on this element.");
                return;
            }

            ParameterSampleHelper.PrintParameters(parameters, "Instance parameters");
            var target = ParameterSampleHelper.PickNamedTypeParam(parameters);
            var paramName = Prompt.AskString("Parameter name to delete", target.Name);
            var param = string.IsNullOrWhiteSpace(paramName)
                ? target
                : element.FindInstanceParameter(paramName) ?? target;
            if (param == null)
            {
                TerminalUi.Error($"Parameter not found: {paramName}");
                return;
            }

            if (element is not Element concrete)
            {
                TerminalUi.Chat("Unexpected element type.");
                return;
            }

            var deleted = concrete.DeleteInstanceParameter(param.SchemaId);
            TerminalUi.Error(deleted ? $"  Deleted '{param.Name ?? param.SchemaId}'." : "  Delete failed.");
            if (deleted)
                await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
