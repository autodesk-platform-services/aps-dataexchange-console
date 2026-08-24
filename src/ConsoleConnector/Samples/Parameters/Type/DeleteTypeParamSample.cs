using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to delete a type parameter.
    /// SDK: ElementDataModel.DeleteTypeParameter.
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(5, 2, 4)]
    public sealed class DeleteTypeParamSample : ISample
    {
        public string Name => "Delete Type Param";
        public string Description => "Delete a type parameter";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ParameterSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var typeName = ParameterSampleHelper.PickTypeName(session.Model);
            if (typeName == null)
                return;
            if (!session.Model.GetTypeParameters(typeName).TryGetValue(typeName, out var parameters)
                || !parameters.Any())
            {
                TerminalUi.Warning($"No type parameters for '{typeName}'.");
                return;
            }

            var list = parameters.ToList();
            ParameterSampleHelper.PrintParameters(list, $"Type '{typeName}' parameters");
            var target = ParameterSampleHelper.PickNamedTypeParam(list);
            var paramName = Prompt.AskString("Parameter name to delete", target.Name);
            var param = string.IsNullOrWhiteSpace(paramName)
                ? target
                : session.Model.FindTypeParameter(typeName, paramName) ?? target;
            if (param == null)
            {
                TerminalUi.Error($"Parameter not found: {paramName}");
                return;
            }

            var deleted = session.Model.DeleteTypeParameter(typeName, param.SchemaId);
            TerminalUi.Error(deleted ? $"  Deleted '{paramName}'." : $"  Delete failed for '{paramName}'.");
            if (deleted)
                await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
