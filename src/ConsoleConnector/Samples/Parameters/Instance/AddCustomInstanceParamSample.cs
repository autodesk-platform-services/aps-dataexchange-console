using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Custom instance params: you choose name and value.
    /// SDK: Element.CreateInstanceParameterAsync (custom).
    /// Console plumbing: ParameterSampleHelper.AddCustomInstanceParamInteractiveAsync.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(5, 1, 2)]
    public sealed class AddCustomInstanceParamSample : ISample
    {
        public string Name => "Add Custom Instance Param";
        public string Description => "Add a custom instance parameter (name + value)";

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

            if (await ParameterSampleHelper.AddCustomInstanceParamInteractiveAsync(element) == null)
                return;

            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
