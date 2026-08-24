using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Interface;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;

namespace ConsoleConnector.Common
{
    internal static class ExchangeFolderListing
    {
        internal sealed record Entry(string FileUrn, string VersionUrn, string DisplayName);

        internal static async Task<List<Entry>> ListInFolderAsync(
            IHostingProvider hosting,
            string projectId,
            string folderId,
            bool subFolder = false)
        {
            var raw = await FetchFileUrnsAsync(hosting, projectId, folderId, subFolder);
            if (raw.Count == 0)
                return new List<Entry>();

            List<Entry> entries = new();
            await TerminalUi.RunWithStatusAsync(
                $"Fetching names for {raw.Count} exchange(s)…",
                async () =>
                {
                    using var gate = new SemaphoreSlim(10);
                    var tasks = new List<Task<Entry>>(raw.Count);

                    foreach (var (fileUrn, versionUrn) in raw)
                        tasks.Add(EnrichWithNameAsync(hosting, projectId, fileUrn, versionUrn, gate));

                    entries = new List<Entry>(await Task.WhenAll(tasks));
                });

            return entries;
        }

        private static async Task<Entry> EnrichWithNameAsync(
            IHostingProvider hosting,
            string projectId,
            string fileUrn,
            string versionUrn,
            SemaphoreSlim gate)
        {
            await gate.WaitAsync().ConfigureAwait(false);
            try
            {
                var name = await TryGetDisplayNameAsync(hosting, projectId, fileUrn).ConfigureAwait(false);
                var displayName = string.IsNullOrWhiteSpace(name) ? ShortLabel(fileUrn) : name.Trim();
                return new Entry(fileUrn, versionUrn, displayName);
            }
            finally
            {
                gate.Release();
            }
        }

        private static async Task<string?> TryGetDisplayNameAsync(
            IHostingProvider hosting,
            string projectId,
            string fileUrn)
        {
            try
            {
                var descriptor = await hosting.GetExchangeMetadataDescriptorAsync(projectId, fileUrn)
                    .ConfigureAwait(false);

                if (descriptor.StatusCode != 200 || descriptor.Metadata?.Data?.Attributes == null)
                    return null;

                var attrs = descriptor.Metadata.Data.Attributes;
                if (!string.IsNullOrWhiteSpace(attrs.DisplayName))
                    return attrs.DisplayName.Trim();

                return null;
            }
            catch (Exception ex)
            {
                TerminalUi.Dim($"Could not fetch display name for {ShortLabel(fileUrn)}: {ex}");
                return null;
            }
        }

        private static async Task<List<(string FileUrn, string VersionUrn)>> FetchFileUrnsAsync(
            IHostingProvider hosting,
            string projectId,
            string folderId,
            bool subFolder)
        {
            var all = new List<(string FileUrn, string VersionUrn)>();
            var page = 0;

            while (true)
            {
                var result = await hosting.GetAllExchangesInfoAsync(projectId, folderId, page, subFolder)
                    .ConfigureAwait(false);

                foreach (var pair in result.ExchangeIds)
                    all.Add((pair.Item1, pair.Item2));

                if (!result.Next)
                    break;

                page++;
            }

            return all;
        }

        private static string ShortLabel(string fileUrn)
        {
            var lastColon = fileUrn.LastIndexOf(':');
            return lastColon >= 0 ? fileUrn[(lastColon + 1)..] : fileUrn;
        }
    }
}
