using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to browse and find instance parameters by name.
    /// SDK: Element.FindInstanceParameter + InstanceParameters.
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(5, 1, 5)]
    public sealed class FindInstanceParamSample : ISample
    {
        public string Name => "Find Instance Param";
        public string Description => "List and find instance parameters by name";

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

            ParameterSampleHelper.PrintParameters(element.InstanceParameters, "All instance parameters");
            var name = Prompt.AskString("Parameter name to find", null);
            if (string.IsNullOrWhiteSpace(name))
                return;
            var found = element.FindInstanceParameter(name);
            if (found == null)
            {
                TerminalUi.Error($"Not found: {name}");
                return;
            }

            TerminalUi.Chat("Found:");
            ParameterSampleHelper.PrintParameter(found, "    ");
        }
    }
}
