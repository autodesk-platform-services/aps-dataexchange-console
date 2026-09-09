using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Find instance param by name, update by schema id.
    /// SDK: Element.FindInstanceParameter + UpdateInstanceParameter.
    /// Console plumbing: ParameterSampleHelper.UpdateInstanceParamInteractive.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(5, 1, 3)]
    public sealed class UpdateInstanceParamSample : ISample
    {
        public string Name => "Update Instance Param";
        public string Description => "Find and update an instance parameter value";

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

            if (ParameterSampleHelper.UpdateInstanceParamInteractive(element) == null)
                return;

            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
