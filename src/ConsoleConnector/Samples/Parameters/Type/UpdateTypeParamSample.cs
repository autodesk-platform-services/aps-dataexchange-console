using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to update a type parameter value.
    /// SDK: ElementDataModel.UpdateTypeParameter.
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(5, 2, 3)]
    public sealed class UpdateTypeParamSample : ISample
    {
        public string Name => "Update Type Param";
        public string Description => "Update a type parameter value";

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
                TerminalUi.Warning($"No type parameters for '{typeName}'. Run 5.2.1 or 5.2.2 first.");
                return;
            }

            var list = parameters.ToList();
            ParameterSampleHelper.PrintParameters(list, $"Type '{typeName}' parameters");
            var target = ParameterSampleHelper.PickNamedTypeParam(list);
            var paramName = Prompt.AskString("Parameter name to update", target.Name);
            var param = string.IsNullOrWhiteSpace(paramName)
                ? target
                : session.Model.FindTypeParameter(typeName, paramName) ?? target;
            if (param == null)
            {
                TerminalUi.Error($"Parameter not found: {paramName}");
                return;
            }

            if (param.Value is not double)
            {
                TerminalUi.Error("Sample only updates numeric type parameters.");
                return;
            }

            var updated = session.Model.UpdateTypeParameter(
                typeName,
                param.SchemaId,
                ParameterSampleHelper.SuggestUpdatedValue(param));
            ParameterSampleHelper.PrintParameter(updated, "  Updated: ");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
