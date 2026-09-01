using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Models;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;
using SessionFolderInfo = ConsoleConnector.Samples.FolderInfo;

namespace ConsoleConnector.Common
{
    /// <summary>
    /// Console plumbing for samples — folder guards, Forma project resolve, exchange pickers.
    /// </summary>
    internal static class NavigationHelper
    {
        internal sealed record PickedExchange(ProjectInfo Project, string FileUrn);

        internal static bool EnsureHub(SampleContext ctx)
        {
            if (ctx.Folder != null && !string.IsNullOrWhiteSpace(ctx.Folder.HubId))
                return true;

            TerminalUi.Warning("No hub in session. Set a folder at startup first.");
            return false;
        }

        internal static bool EnsureProjectFolder(SampleContext ctx)
        {
            if (ctx.Folder != null
                && !string.IsNullOrWhiteSpace(ctx.Folder.HubId)
                && !string.IsNullOrWhiteSpace(ctx.Folder.ProjectUrn))
                return true;

            TerminalUi.Warning("No project in session. Set a folder at startup first.");
            return false;
        }

        internal static bool IsCompleteFolder(SessionFolderInfo? folder) =>
            folder != null
            && !string.IsNullOrWhiteSpace(folder.HubId)
            && !string.IsNullOrWhiteSpace(folder.ProjectUrn)
            && !string.IsNullOrWhiteSpace(folder.FolderUrn);

        internal static bool EnsureFullFolder(SampleContext ctx)
        {
            if (IsCompleteFolder(ctx.Folder))
                return true;

            TerminalUi.Warning("No folder in session. Set a folder at startup first.");
            return false;
        }

        internal static SessionFolderInfo? LoadFolderFromSession(SampleContext ctx)
        {
            return EnsureFullFolder(ctx) ? ctx.Folder : null;
        }

        internal static async Task<string?> PickExchangeFileUrnAsync(SampleContext ctx)
        {
            var picked = await PickExchangeAsync(ctx);
            return picked?.FileUrn;
        }

        internal static async Task<PickedExchange?> PickExchangeAsync(SampleContext ctx)
        {
            if (!EnsureFullFolder(ctx))
                return null;

            var hosting = HostingSampleHelper.GetProvider(ctx);
            TerminalUi.Info($"Resolving project {ctx.Folder!.ProjectUrn}…");
            var project = await hosting.GetProjectInformationAsync(ctx.Folder.HubId, ctx.Folder.ProjectUrn);
            if (project == null)
            {
                TerminalUi.Error("Project not found.");
                return null;
            }

            var fileUrn = await ExchangeFolderPicker.PickFileUrnAsync(
                hosting,
                project.ProjectId,
                ctx.Folder.FolderUrn,
                ctx);
            if (fileUrn == null)
            {
                TerminalUi.Dim("Cancelled.");
                return null;
            }

            return new PickedExchange(project, fileUrn);
        }
    }
}
