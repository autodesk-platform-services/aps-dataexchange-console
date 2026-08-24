using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to add a root element and classify it (Category, Family, Type) before syncing.
    /// SDK: ElementDataModel.AddElement + Classify/DefineType/SetType + SyncExchangeDataAsync.
    /// Console plumbing: ElementSampleHelper.AddRootElement.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(3, 1)]
    public sealed class AddRootElementSample : ISample
    {
        public string Name => "Add Root Element";
        public string Description => "Add a top-level element (no parent) and sync";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ElementSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;

            var element = ElementSampleHelper.AddRootElement(session.Model);
            if (element == null)
                return;

            ElementSampleHelper.PrintElementSummary(element);
            if (await ElementSampleHelper.SyncAsync(ctx, session))
                TerminalUi.Chat($"Elements after: {session.Model.Elements.Count()}");
        }
    }
}
