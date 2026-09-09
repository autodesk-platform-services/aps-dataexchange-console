using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to resolve a hub and list its projects.
    /// SDK: IHostingProvider.GetHubAsync + GetProjectsAsync.
    /// Console plumbing: HostingSampleHelper, NavigationHelper.EnsureHub.
    /// Prerequisites: saved session folder (Hub Id from bootstrap).
    /// </summary>
    [SampleAddress(1, 2)]
    public sealed class ListProjectsSample : ISample
    {
        public string Name => "List Projects";
        public string Description => "List projects in the hub from your saved session";

        public async Task RunAsync(SampleContext ctx)
        {
            if (!NavigationHelper.EnsureHub(ctx))
                return;

            var hosting = HostingSampleHelper.GetProvider(ctx);
            var hubId = ctx.Folder!.HubId;

            // SDK: resolve hub and list projects
            TerminalUi.Info($"Resolving hub {hubId}...");
            var hub = await hosting.GetHubAsync(hubId);
            TerminalUi.Info($"Fetching projects in {hub.HubName}...");
            var projects = await hosting.GetProjectsAsync(hub);

            if (projects == null || projects.Count == 0)
            {
                TerminalUi.Warning("No projects found.");
                return;
            }

            TerminalUi.Chat($"Found {projects.Count} project(s):");
            foreach (var project in projects)
                TerminalUi.Chat($"  {project.ProjectName}  id={project.ProjectId}  type={project.ProjectType}");
        }
    }
}
