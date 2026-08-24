using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Models;
using Autodesk.DataExchange.Interface;
using Autodesk.DataExchange.Models;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;

namespace ConsoleConnector.Common
{
    internal static class DownloadSampleHelper
    {
        internal static async Task<ElementSampleSession?> BeginAsync(SampleContext ctx) =>
            await ElementSampleHelper.BeginAsync(ctx);

        internal static string ResolveOutputPath(string label, string fileName)
        {
            var defaultDir = Path.Combine(SessionStore.SessionDirectory, "downloads");
            Directory.CreateDirectory(defaultDir);
            var defaultPath = Path.Combine(defaultDir, fileName);
            var path = Prompt.AskString(label, defaultPath);
            if (string.IsNullOrWhiteSpace(path))
                return defaultPath;

            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(dir))
                Directory.CreateDirectory(dir);
            return path;
        }

        // The only SDK call in this project whose signature changed between SDK versions, so it is
        // isolated here instead of being duplicated across sample call sites. Define
        // DXSDK_LEGACY_OBJ_DOWNLOAD when building against a pre-7.7.0 package; that keeps this file
        // byte-identical across the SDK repo and the public sample repo.
        //   7.6.0-beta : DownloadCompleteExchangeAsOBJ(exchangeId, collectionId, objFolderPath, ct)
        //   7.7.0      : DownloadCompleteExchangeAsOBJ(DataExchangeIdentifier, objFolderPath, ct)
        internal static IResponse<string> DownloadObj(SampleContext ctx, DataExchangeIdentifier identifier, string objFolderPath) =>
#if DXSDK_LEGACY_OBJ_DOWNLOAD
            ctx.Client.DownloadCompleteExchangeAsOBJ(
                identifier.ExchangeId,
                identifier.CollectionId,
                objFolderPath,
                CancellationToken.None);
#else
            ctx.Client.DownloadCompleteExchangeAsOBJ(identifier, objFolderPath, CancellationToken.None);
#endif

        internal static async Task RunExchangeDownloadAsync(
            string format,
            Func<IResponse<string>> download)
        {
            IResponse<string> response = default!;
            await TerminalUi.RunWithStatusAsync(
                $"Downloading {format}…",
                async () => response = await Task.Run(download).ConfigureAwait(false));
            PrintDownloadResult(response, format);
        }

        internal static void PrintDownloadResult(IResponse<string> response, string format)
        {
            if (response.IsFailed)
            {
                var message = response.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                TerminalUi.Error($"{format} download failed: {message}");
                return;
            }

            TerminalUi.Success($"{format} saved to: {response.Value}");
        }
    }
}
