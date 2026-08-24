using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How parent/child hierarchy works when adding nested elements.
    /// SDK: ElementDataModel.AddElement (child) + SyncExchangeDataAsync.
    /// Console plumbing: ElementSampleHelper.AddChildElement.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(3, 2)]
    public sealed class AddChildElementSample : ISample
    {
        public string Name => "Add Child Element";
        public string Description => "Add a nested element under an existing parent";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ElementSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;

            var child = ElementSampleHelper.AddChildElement(ctx, session);
            if (child == null)
                return;

            if (await ElementSampleHelper.SyncAsync(ctx, session))
                TerminalUi.Chat($"Elements after: {session.Model.Elements.Count()}");
        }
    }
}
