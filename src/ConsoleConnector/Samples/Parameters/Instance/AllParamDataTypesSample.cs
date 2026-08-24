using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Params support bool, long, double, and string types.
    /// SDK: Element.CreateInstanceParameterAsync + UpdateInstanceParameter (bool, long, double, string).
    /// Console plumbing: ParameterSampleHelper.BeginAsync, ParameterSampleHelper.DemoAllDataTypesAsync.
    /// Prerequisites: 2.2 Load Exchange; 3.1 Add Root Element.
    /// </summary>
    [SampleAddress(5, 1, 9)]
    public sealed class AllParamDataTypesSample : ISample
    {
        public string Name => "All Param Data Types";
        public string Description => "Add and update bool, long, double, and string params";

        public async Task RunAsync(SampleContext ctx)
        {
            // Console plumbing (not SDK): pick loaded exchange and element
            var session = await ParameterSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;

            var element = ParameterSampleHelper.PickElement(session.Model);
            if (element == null)
            {
                TerminalUi.Dim("Cancelled.");
                return;
            }

            // SDK: add/update all four parameter data types
            if (!await ParameterSampleHelper.DemoAllDataTypesAsync(element))
                return;

            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
