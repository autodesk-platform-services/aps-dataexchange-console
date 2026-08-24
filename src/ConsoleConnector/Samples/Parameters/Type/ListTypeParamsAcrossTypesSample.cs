using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using Spectre.Console;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to list type params for one type or all types.
    /// SDK: ElementDataModel.GetTypeParameters.
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(5, 2, 6)]
    public sealed class ListTypeParamsAcrossTypesSample : ISample
    {
        public string Name => "List Type Params Across Types";
        public string Description => "List type parameters for one or all types";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ParameterSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var allTypes = session.Model.GetTypeParameters();
            TerminalUi.Chat($"Types with parameters: {allTypes.Count}");
            foreach (var entry in allTypes)
            {
                var list = entry.Value.ToList();
            TerminalUi.Chat($"  Type '{entry.Key}': {list.Count} parameter(s)");
            foreach (var parameter in list)
                    ParameterSampleHelper.PrintParameter(parameter, "      ");
        }

            var filterType = ParameterSampleHelper.DefaultTypeName;
            if (allTypes.TryGetValue(filterType, out var filtered))
            {
                AnsiConsole.WriteLine();
            TerminalUi.Chat($"Filtered lookup for '{filterType}':");
            ParameterSampleHelper.PrintParameters(filtered, "Filtered");
        }
        }
    }
}
