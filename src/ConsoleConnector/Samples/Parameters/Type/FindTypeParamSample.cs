using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to find a type parameter by name.
    /// SDK: ElementDataModel.FindTypeParameter.
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(5, 2, 5)]
    public sealed class FindTypeParamSample : ISample
    {
        public string Name => "Find Type Param";
        public string Description => "Find a type parameter by name";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ParameterSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var typeName = ParameterSampleHelper.PickTypeName(session.Model);
            if (typeName == null)
                return;
            if (session.Model.GetTypeParameters(typeName).TryGetValue(typeName, out var parameters))
                ParameterSampleHelper.PrintParameters(parameters, $"Type '{typeName}' parameters");
            var paramName = Prompt.AskString("Parameter name to find", null);
            if (string.IsNullOrWhiteSpace(paramName))
                return;
            var found = session.Model.FindTypeParameter(typeName, paramName);
            if (found == null)
            {
                TerminalUi.Error($"Not found: {paramName} on type '{typeName}'");
                return;
            }

            TerminalUi.Chat("Found:");
            ParameterSampleHelper.PrintParameter(found, "    ");
        }
    }
}
