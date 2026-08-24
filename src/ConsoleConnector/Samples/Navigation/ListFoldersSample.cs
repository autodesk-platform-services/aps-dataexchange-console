using System.Collections.Generic;
using System.Threading.Tasks;
using AccFolderInfo = Autodesk.DataExchange.Core.Models.FolderInfo;
using ConsoleConnector.Driver;
using Spectre.Console;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to walk the Forma folder tree (top-level and children).
    /// SDK: IHostingProvider.GetProjectInformationAsync, GetProjectFoldersAsync, GetChildrenFoldersAsync.
    /// Console plumbing: HostingSampleHelper, NavigationHelper.EnsureProjectFolder.
    /// Prerequisites: saved session folder (Hub Id + Project URN).
    /// </summary>
    [SampleAddress(1, 3)]
    public sealed class ListFoldersSample : ISample
    {
        public string Name => "List Folders";
        public string Description => "List project folders and children of your saved folder";

        public async Task RunAsync(SampleContext ctx)
        {
            if (!NavigationHelper.EnsureProjectFolder(ctx))
                return;

            var hosting = HostingSampleHelper.GetProvider(ctx);
            var hubId = ctx.Folder!.HubId;
            var projectUrn = ctx.Folder.ProjectUrn;

            // SDK: resolve project and list folders
            TerminalUi.Info($"Resolving project {projectUrn}...");
            var project = await hosting.GetProjectInformationAsync(hubId, projectUrn);
            if (project == null)
            {
                TerminalUi.Error("Project not found.");
                return;
            }

            TerminalUi.Chat($"Top-level folders in {project.ProjectName}:");
            var topFolders = await hosting.GetProjectFoldersAsync(project);
            PrintFolders(topFolders);

            if (string.IsNullOrWhiteSpace(ctx.Folder.FolderUrn))
                return;

            AnsiConsole.WriteLine();
            TerminalUi.Chat("Subfolders under saved folder:");
            var parent = new AccFolderInfo
            {
                FolderId = ctx.Folder.FolderUrn,
                ProjectId = project.ProjectId,
                HubId = project.HubId,
                HubRegion = project.HubRegion,
            };

            var children = await hosting.GetChildrenFoldersAsync(parent);
            PrintFolders(children);
        }

        private static void PrintFolders(List<AccFolderInfo>? folders)
        {
            if (folders == null || folders.Count == 0)
            {
                TerminalUi.Chat("  (none)");
                return;
            }

            foreach (var folder in folders)
            {
                var childHint = folder.IsChildFolderPresent ? "has children" : "leaf";
                TerminalUi.Chat($"  {folder.FolderName}  id={folder.FolderId}  ({childHint})");
            }
        }
    }
}
