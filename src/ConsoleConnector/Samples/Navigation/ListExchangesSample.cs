using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to find Data Exchanges in a folder with display names.
    /// SDK: IHostingProvider.GetAllExchangesInfoAsync + GetExchangeMetadataDescriptorAsync.
    /// Console plumbing: HostingSampleHelper, NavigationHelper, ExchangeFolderListing.
    /// Prerequisites: saved session folder (Hub Id, Project URN, Folder URN).
    /// </summary>
    [SampleAddress(1, 4)]
    public sealed class ListExchangesSample : ISample
    {
        public string Name => "List Exchanges";
        public string Description => "List Data Exchanges in your saved folder";

        public async Task RunAsync(SampleContext ctx)
        {
            if (!NavigationHelper.EnsureFullFolder(ctx))
                return;

            var project = await HostingSampleHelper.ResolveProjectAsync(ctx);
            if (project == null)
            {
                TerminalUi.Error("Project not found.");
                return;
            }

            var exchanges = await ExchangeFolderListing.ListInFolderAsync(
                HostingSampleHelper.GetProvider(ctx),
                project.ProjectId,
                ctx.Folder!.FolderUrn);

            if (exchanges.Count == 0)
            {
                TerminalUi.Warning("No exchanges found.");
                return;
            }

            TerminalUi.WriteTable(
                $"Found {exchanges.Count} exchange(s)",
                exchanges,
                ("Name", e => e.DisplayName),
                ("File URN", e => e.FileUrn),
                ("Version URN", e => e.VersionUrn));
        }
    }
}
