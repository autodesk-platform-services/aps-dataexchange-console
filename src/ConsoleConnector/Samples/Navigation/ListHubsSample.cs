using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to discover which Forma hubs your token can access.
    /// SDK: IHostingProvider.GetHubsAsync.
    /// Console plumbing: HostingSampleHelper.GetProvider.
    /// Prerequisites: none.
    /// </summary>
    [SampleAddress(1, 1)]
    public sealed class ListHubsSample : ISample
    {
        public string Name => "List Hubs";
        public string Description => "List Forma hubs you have access to";

        public async Task RunAsync(SampleContext ctx)
        {
            var hosting = HostingSampleHelper.GetProvider(ctx);
            TerminalUi.Info("Fetching hubs…");

            // SDK: list accessible hubs
            var hubs = await hosting.GetHubsAsync();

            if (hubs == null || hubs.Count == 0)
            {
                TerminalUi.Warning("No hubs found.");
                return;
            }

            TerminalUi.WriteTable(
                $"Found {hubs.Count} hub(s)",
                hubs,
                ("Name", h => h.HubName),
                ("Hub Id", h => h.HubId),
                ("Region", h => h.HubRegion));
        }
    }
}
