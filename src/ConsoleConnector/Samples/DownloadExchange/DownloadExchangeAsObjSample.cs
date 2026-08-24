using System;
using System.Threading;
using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to export the entire exchange to OBJ.
    /// SDK: IClient.DownloadCompleteExchangeAsOBJ.
    /// Console plumbing: DownloadSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(7, 3)]
    public sealed class DownloadExchangeAsObjSample : ISample
    {
        public string Name => "Download Exchange As OBJ";
        public string Description => "Download the whole exchange as OBJ";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await DownloadSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var path = DownloadSampleHelper.ResolveOutputPath("OBJ output folder", "exchange_obj");
            await DownloadSampleHelper.RunExchangeDownloadAsync(
                "OBJ",
                () => DownloadSampleHelper.DownloadObj(ctx, session.Identifier, path));
        }
    }
}
