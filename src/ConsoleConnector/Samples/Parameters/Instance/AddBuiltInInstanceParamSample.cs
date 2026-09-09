using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Built-in Revit schema params use a known schema id.
    /// SDK: Element.CreateInstanceParameterAsync (built-in schema).
    /// Console plumbing: ParameterSampleHelper.AddBuiltInInstanceParamInteractiveAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(5, 1, 1)]
    public sealed class AddBuiltInInstanceParamSample : ISample
    {
        public string Name => "Add Built-In Instance Param";
        public string Description => "Add a built-in schema instance parameter to an element";

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

            if (await ParameterSampleHelper.AddBuiltInInstanceParamInteractiveAsync(ctx, element) == null)
                return;

            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
