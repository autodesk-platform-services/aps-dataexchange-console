using System.Threading;
using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to export the entire exchange to a SAT file.
    /// SDK: IClient.DownloadCompleteExchangeAsSAT.
    /// Console plumbing: DownloadSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(7, 4)]
    public sealed class DownloadExchangeAsSatSample : ISample
    {
        public string Name => "Download Exchange As SAT";
        public string Description => "Download the whole exchange as SAT";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await DownloadSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var path = DownloadSampleHelper.ResolveOutputPath("SAT output path", "exchange.sat");
            await DownloadSampleHelper.RunExchangeDownloadAsync(
                "SAT",
                () => ctx.Client.DownloadCompleteExchangeAsSAT(session.Identifier, path, CancellationToken.None));
        }
    }
}
