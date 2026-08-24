using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to export the entire exchange to IFC.
    /// SDK: IClient.DownloadCompleteExchangeAsIFC.
    /// Console plumbing: DownloadSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(7, 2)]
    public sealed class DownloadExchangeAsIfcSample : ISample
    {
        public string Name => "Download Exchange As IFC";
        public string Description => "Download the whole exchange as IFC";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await DownloadSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var path = DownloadSampleHelper.ResolveOutputPath("IFC output path", "exchange.ifc");
            await DownloadSampleHelper.RunExchangeDownloadAsync(
                "IFC",
                () => ctx.Client.DownloadCompleteExchangeAsIFC(session.Identifier, path, CancellationToken.None));
        }
    }
}
