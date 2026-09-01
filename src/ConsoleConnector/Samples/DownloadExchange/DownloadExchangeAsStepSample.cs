using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to export the entire exchange to a STEP file.
    /// SDK: IClient.DownloadCompleteExchangeAsSTEP.
    /// Console plumbing: DownloadSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(7, 1)]
    public sealed class DownloadExchangeAsStepSample : ISample
    {
        public string Name => "Download Exchange As STEP";
        public string Description => "Download the whole exchange as STEP";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await DownloadSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var path = DownloadSampleHelper.ResolveOutputPath("STEP output path", "exchange.step");
            await DownloadSampleHelper.RunExchangeDownloadAsync(
                "STEP",
                () => ctx.Client.DownloadCompleteExchangeAsSTEP(session.Identifier, path, CancellationToken.None));
        }
    }
}
