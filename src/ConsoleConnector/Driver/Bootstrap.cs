using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Autodesk.DataExchange;
using Autodesk.DataExchange.Core.Interface;
using Autodesk.DataExchange.Core.Models;
using ConsoleConnector.Common;
using ConsoleConnector.Samples;
using Spectre.Console;
using AccFolderInfo = Autodesk.DataExchange.Core.Models.FolderInfo;
using SessionFolderInfo = ConsoleConnector.Samples.FolderInfo;

namespace ConsoleConnector.Driver
{
    internal static class Bootstrap
    {
        private const string ContinueChoice = "Continue with saved session";
        private const string ChangeChoice = "Change session";
        private const string QuitChoice = "Quit";
        public static async Task<SessionData> EnsureSessionAsync(SessionData session, SampleContext ctx)
        {
            SessionStore.Apply(session, ctx);

            if (!NavigationHelper.IsCompleteFolder(session.Folder))
                return await FirstRunAsync(session, ctx);

            return await SubsequentRunAsync(session, ctx);
        }

        private static async Task<SessionData> FirstRunAsync(SessionData session, SampleContext ctx)
        {
            TerminalUi.Section("First run", "Set the active Forma folder");
            TerminalUi.Info(
                "Paste a Forma or ACC docs URL, or press Enter to enter Hub / Project / Folder IDs manually.");
            TerminalUi.Info(
                "ACC URLs do not include a Hub Id — the app looks it up via API. That requires your Forge " +
                "Client ID to be added under ACC Hub Admin -> Custom Integrations for the target hub.");

            var url = Prompt.AskString("Forma URL", null);
            SessionFolderInfo? folder;
            if (string.IsNullOrWhiteSpace(url))
                folder = PromptFolderByIds(null);
            else
                folder = await ResolveFolderFromUrlAsync(url, null, ctx);

            if (folder == null)
            {
                TerminalUi.Goodbye();
                Environment.Exit(0);
            }

            session.Folder = folder;

            session.Folder = await EnrichFolderAsync(session.Folder, ctx);
            PrintFolder(session.Folder, "Session folder");

            if (Prompt.AskBool("Save session?", defaultValue: true))
                SessionStore.Save(session);

            return session;
        }

        private static async Task<SessionData> SubsequentRunAsync(SessionData session, SampleContext ctx)
        {
            if (BatchMode.Enabled)
            {
                if (NeedsNameEnrichment(session.Folder!))
                {
                    session.Folder = await EnrichFolderAsync(session.Folder!, ctx);
                    SessionStore.Save(session);
                }

                return session;
            }

            if (NeedsNameEnrichment(session.Folder!))
            {
                session.Folder = await EnrichFolderAsync(session.Folder!, ctx);
                SessionStore.Save(session);
            }

            WriteSavedSessionPanel(session);

            while (true)
            {
                var choice = TerminalUi.Pick(
                    "Session",
                    ContinueChoice,
                    ChangeChoice,
                    QuitChoice);

                switch (choice)
                {
                    case ContinueChoice:
                        return session;
                    case ChangeChoice:
                        var updated = await PromptNewFolderAsync(session.Folder!, ctx);
                        if (updated == null)
                            return session;

                        session.Folder = await EnrichFolderAsync(updated, ctx);
                        PrintFolder(session.Folder);
                        if (Prompt.AskBool("Save session?", defaultValue: true))
                            SessionStore.Save(session);
                        return session;
                    case QuitChoice:
                        TerminalUi.Goodbye();
                        Environment.Exit(0);
                        return session;
                }
            }
        }

        private static SessionFolderInfo PromptFolderByIds(SessionFolderInfo? current)
        {
            var hubId = Prompt.AskString("Hub Id", current?.HubId);
            var region = Prompt.AskString("Region", current?.Region ?? "US");
            var projectUrn = Prompt.AskString("Project URN", current?.ProjectUrn);
            var folderUrn = Prompt.AskString("Folder URN", current?.FolderUrn);
            return new SessionFolderInfo(hubId, projectUrn, folderUrn, region);
        }

        private static async Task<SessionFolderInfo?> PromptNewFolderAsync(SessionFolderInfo current, SampleContext ctx)
        {
            while (true)
            {
                var choice = TerminalUi.Pick(
                    "Update folder",
                    "From Forma URL",
                    "Hub Id",
                    "Region",
                    "Project URN",
                    "Folder URN",
                    "Cancel");

                switch (choice)
                {
                    case "From Forma URL":
                        return await PromptFolderFromUrlAsync(current, ctx);
                    case "Hub Id":
                        return current with { HubId = Prompt.AskString("Hub Id", current.HubId) };
                    case "Region":
                        return current with { Region = Prompt.AskString("Region", current.Region) };
                    case "Project URN":
                        return current with { ProjectUrn = Prompt.AskString("Project URN", current.ProjectUrn) };
                    case "Folder URN":
                        return current with { FolderUrn = Prompt.AskString("Folder URN", current.FolderUrn) };
                    case "Cancel":
                        return null;
                }
            }
        }

        private static async Task<SessionFolderInfo?> PromptFolderFromUrlAsync(SessionFolderInfo? current, SampleContext ctx)
        {
            TerminalUi.Info("Paste a Forma or ACC docs URL, or press Enter to cancel.");
            var url = Prompt.AskString("Forma URL", null);
            if (string.IsNullOrWhiteSpace(url))
                return null;

            return await ResolveFolderFromUrlAsync(url, current, ctx);
        }

        private static async Task<SessionFolderInfo?> ResolveFolderFromUrlAsync(string url, SessionFolderInfo? defaults, SampleContext ctx)
        {
            var parsed = TryParseFormaUrl(url);
            if (parsed == null)
                return null;

            parsed = await FillMissingHubAsync(parsed, defaults, ctx);

            if (string.IsNullOrEmpty(parsed.HubId))
                return await PromptAfterHubLookupFailedAsync(parsed, defaults, ctx);

            return parsed;
        }

        private static async Task<SessionFolderInfo?> PromptAfterHubLookupFailedAsync(
            SessionFolderInfo parsed,
            SessionFolderInfo? defaults,
            SampleContext ctx)
        {
            var folder = parsed;
            ExplainAccCustomIntegrationRequired(parsed.ProjectUrn);

            while (true)
            {
                var choice = TerminalUi.Pick(
                    "Hub lookup failed",
                    "Retry (added Custom Integration?)",
                    "Paste a different URL",
                    "Enter Hub / Project / Folder manually",
                    "Exit");

                switch (choice)
                {
                    case "Retry (added Custom Integration?)":
                        folder = await FillMissingHubAsync(folder, defaults, ctx);
                        if (!string.IsNullOrEmpty(folder.HubId))
                            return folder;

                        TerminalUi.Warning(
                            "Hub lookup still failed. Confirm Custom Integration is approved and your account has hub access.");
                        break;
                    case "Paste a different URL":
                        TerminalUi.Info("Paste a Forma or ACC docs URL, or press Enter to go back.");
                        var url = Prompt.AskString("Forma URL", null);
                        if (string.IsNullOrWhiteSpace(url))
                            break;

                        var retried = await ResolveFolderFromUrlAsync(url, defaults, ctx);
                        if (retried != null)
                            return retried;
                        break;
                    case "Enter Hub / Project / Folder manually":
                        return PromptFolderByIds(MergeFolderDefaults(parsed, defaults));
                    case "Exit":
                        TerminalUi.Goodbye();
                        Environment.Exit(0);
                        return null;
                }
            }
        }

        private static SessionFolderInfo MergeFolderDefaults(SessionFolderInfo parsed, SessionFolderInfo? defaults)
        {
            if (defaults == null)
                return parsed;

            return parsed with
            {
                HubId = string.IsNullOrEmpty(parsed.HubId) ? defaults.HubId : parsed.HubId,
                Region = string.IsNullOrEmpty(parsed.Region) ? defaults.Region : parsed.Region,
            };
        }

        private static async Task<SessionFolderInfo> FillMissingHubAsync(SessionFolderInfo parsed, SessionFolderInfo? defaults, SampleContext ctx)
        {
            if (!string.IsNullOrEmpty(parsed.HubId))
                return parsed;

            try
            {
                var hosting = GetHostingProvider(ctx);
                var result = parsed;
                await TerminalUi.RunWithStatusAsync(
                    $"Looking up hub for project {parsed.ProjectUrn}…",
                    async () => result = await LookupHubAsync(parsed, hosting).ConfigureAwait(false))
                    .ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                TerminalUi.Warning($"Hub lookup failed: {ex}");
                return parsed;
            }
        }

        private static void ExplainAccCustomIntegrationRequired(string? projectUrn)
        {
            var clientId = GetConfiguredClientId();
            var clientLine = string.IsNullOrWhiteSpace(clientId)
                ? "[dim]Use the Client ID from your Forge app at aps.autodesk.com/myapps[/]"
                : $"[bold]Client ID[/]  {Markup.Escape(clientId)}";

            var projectLine = string.IsNullOrWhiteSpace(projectUrn)
                ? string.Empty
                : $"\n[dim]Project[/] {Markup.Escape(projectUrn)} — your account does not have access to a hub that contains this project.";

            TerminalUi.WriteMarkupPanel(
                "ACC Custom Integration required",
                "[dim]A hub admin must authorize this Forge app before hub/project lookup works:[/]\n"
                + "1. ACC -> [bold]Hub Admin[/] -> [bold]Custom Integrations[/]\n"
                + "2. [bold]Add Custom Integration[/] -> paste your Forge Client ID\n"
                + "3. Approve the integration\n"
                + "4. Your Autodesk account must have access to the hub\n"
                + "5. Retry folder setup — pick [bold]Retry (added Custom Integration?)[/], or use menu [bold]1.1 List Hubs[/] / [bold]1.2 List Projects[/] to verify\n\n"
                + clientLine
                + projectLine);
        }

        private static string? GetConfiguredClientId()
        {
            var fromEnv = Environment.GetEnvironmentVariable("DXSDK_CLIENT_ID");
            if (!string.IsNullOrWhiteSpace(fromEnv))
                return fromEnv.Trim();

            var fromConfig = ConfigurationManager.AppSettings["AuthClientId"];
            return string.IsNullOrWhiteSpace(fromConfig) ? null : fromConfig.Trim();
        }

        private static async Task<SessionFolderInfo> LookupHubAsync(SessionFolderInfo parsed, IHostingProvider hosting)
        {
            var hubId = await hosting.GetHubIdAsync(parsed.ProjectUrn).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(hubId))
                return parsed;

            var region = await hosting.GetRegionAsync(hubId).ConfigureAwait(false);
            return parsed with
            {
                HubId = hubId,
                Region = string.IsNullOrWhiteSpace(region) ? parsed.Region : region,
            };
        }

        private static IHostingProvider GetHostingProvider(SampleContext ctx)
        {
            if (ctx.Client is Client client && client.SDKOptions?.HostingProvider != null)
                return client.SDKOptions.HostingProvider;

            throw new InvalidOperationException("HostingProvider is not available — sign in before setting a folder.");
        }

        private static SessionFolderInfo? TryParseFormaUrl(string url)
        {
            if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri))
                return null;

            var query = uri.Query;
            var folderUrn = GetQueryParam(query, "folderUrn") ?? ExtractFolderUrn(url);
            if (string.IsNullOrEmpty(folderUrn))
                return null;

            var hubId = GetQueryParam(query, "hubId") ?? ExtractToken(url, @"\b(b\.[a-f0-9-]+)\b");
            var projectUrn = GetQueryParam(query, "projectId") ?? ExtractToken(url, @"projects/([^/?&#]+)");
            if (string.IsNullOrEmpty(projectUrn))
                return null;

            projectUrn = NormalizeProjectId(projectUrn);
            var region = InferRegion(uri.Host);

            return new SessionFolderInfo(hubId ?? string.Empty, projectUrn, folderUrn, region);
        }

        private static string NormalizeProjectId(string projectId)
        {
            projectId = projectId.Trim();
            if (projectId.StartsWith("b.", StringComparison.OrdinalIgnoreCase))
                return projectId;

            return "b." + projectId;
        }

        private static string? ExtractFolderUrn(string url)
        {
            var decoded = Uri.UnescapeDataString(url);
            var match = System.Text.RegularExpressions.Regex.Match(
                decoded,
                @"urn:adsk\.[^/?&#\s]+:fs\.folder:[^/?&#\s]+",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return match.Success ? match.Value : null;
        }

        private static string? ExtractToken(string text, string pattern)
        {
            var match = System.Text.RegularExpressions.Regex.Match(text, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : null;
        }

        private static string? GetQueryParam(string query, string key)
        {
            if (string.IsNullOrEmpty(query))
                return null;

            foreach (var part in query.TrimStart('?').Split('&'))
            {
                var pair = part.Split(new[] { '=' }, 2);
                if (pair.Length == 2
                    && string.Equals(Uri.UnescapeDataString(pair[0]), key, StringComparison.OrdinalIgnoreCase))
                {
                    return Uri.UnescapeDataString(pair[1]);
                }
            }

            return null;
        }

        private static string InferRegion(string host)
        {
            if (host.IndexOf("eu", StringComparison.OrdinalIgnoreCase) >= 0)
                return "EMEA";
            if (host.IndexOf("australasia", StringComparison.OrdinalIgnoreCase) >= 0)
                return "AUS";
            return "US";
        }

        private static void PrintFolder(SessionFolderInfo folder, string panelTitle = "Saved session")
        {
            WriteSavedSessionPanel(new SessionData { Folder = folder }, panelTitle);
        }

        private static void WriteSavedSessionPanel(SessionData session, string panelTitle = "Saved session")
        {
            var folder = session.Folder!;
            var folderLabel = DisplayName(folder.FolderName, ShortUrn(folder.FolderUrn));
            var projectLabel = DisplayName(folder.ProjectName, ShortUrn(folder.ProjectUrn));
            var hubLabel = DisplayName(folder.HubName, folder.HubId);

            var lines = new List<string>
            {
                $"[bold white]Folder[/]   {Markup.Escape(folderLabel)}",
                $"[dim]Project[/] {Markup.Escape(projectLabel)}",
                $"[dim]Hub[/]     {Markup.Escape(hubLabel)}",
            };

            if (session.LastExchange != null)
                lines.Add($"[dim]Exchange[/] {Markup.Escape(session.LastExchange.Title)}");

            lines.Add($"[dim]Region[/]  {Markup.Escape(folder.Region)}");

            TerminalUi.WriteMarkupPanel(panelTitle, string.Join("\n", lines));
        }

        private static bool NeedsNameEnrichment(SessionFolderInfo folder) =>
            string.IsNullOrWhiteSpace(folder.FolderName)
            || string.IsNullOrWhiteSpace(folder.ProjectName)
            || string.IsNullOrWhiteSpace(folder.HubName);

        private static async Task<SessionFolderInfo> EnrichFolderAsync(SessionFolderInfo folder, SampleContext ctx)
        {
            if (!NeedsNameEnrichment(folder))
                return folder;

            try
            {
                var hosting = GetHostingProvider(ctx);
                SessionFolderInfo enriched = folder;

                await TerminalUi.RunWithStatusAsync(
                    "Resolving saved session…",
                    async () =>
                    {
                        string? hubName = folder.HubName;
                        string? projectName = folder.ProjectName;
                        string? folderName = folder.FolderName;

                        if (!string.IsNullOrWhiteSpace(folder.HubId)
                            && string.IsNullOrWhiteSpace(hubName))
                        {
                            var hub = await hosting.GetHubAsync(folder.HubId).ConfigureAwait(false);
                            hubName = hub?.HubName;
                        }

                        ProjectInfo? project = null;
                        if (!string.IsNullOrWhiteSpace(folder.HubId)
                            && !string.IsNullOrWhiteSpace(folder.ProjectUrn))
                        {
                            project = await hosting.GetProjectInformationAsync(folder.HubId, folder.ProjectUrn)
                                .ConfigureAwait(false);
                            if (string.IsNullOrWhiteSpace(projectName))
                                projectName = project?.ProjectName;
                        }

                        if (project != null
                            && !string.IsNullOrWhiteSpace(folder.FolderUrn)
                            && string.IsNullOrWhiteSpace(folderName))
                        {
                            folderName = await ResolveFolderNameAsync(hosting, project, folder.FolderUrn)
                                .ConfigureAwait(false);
                        }

                        enriched = folder with
                        {
                            HubName = hubName,
                            ProjectName = projectName,
                            FolderName = folderName,
                        };
                    });

                return enriched;
            }
            catch (Exception ex)
            {
                TerminalUi.Warning($"Could not resolve session names: {ex}");
                return folder;
            }
        }

        private static async Task<string?> ResolveFolderNameAsync(
            IHostingProvider hosting,
            ProjectInfo project,
            string folderUrn)
        {
            var queue = new Queue<AccFolderInfo>();
            foreach (var top in await hosting.GetProjectFoldersAsync(project).ConfigureAwait(false) ?? [])
            {
                if (FolderIdsMatch(top.FolderId, folderUrn))
                    return top.FolderName;

                if (top.IsChildFolderPresent)
                    queue.Enqueue(top);
            }

            while (queue.Count > 0)
            {
                var parent = queue.Dequeue();
                foreach (var child in await hosting.GetChildrenFoldersAsync(parent).ConfigureAwait(false) ?? [])
                {
                    if (FolderIdsMatch(child.FolderId, folderUrn))
                        return child.FolderName;

                    if (child.IsChildFolderPresent)
                        queue.Enqueue(child);
                }
            }

            return null;
        }

        private static bool FolderIdsMatch(string? left, string? right) =>
            !string.IsNullOrWhiteSpace(left)
            && !string.IsNullOrWhiteSpace(right)
            && string.Equals(left.Trim(), right.Trim(), StringComparison.OrdinalIgnoreCase);

        private static string DisplayName(string? name, string fallback) =>
            string.IsNullOrWhiteSpace(name) ? fallback : name.Trim();

        private static string ShortUrn(string urn)
        {
            if (string.IsNullOrWhiteSpace(urn))
                return "(unknown)";

            var tail = urn.Trim();
            var colon = tail.LastIndexOf(':');
            if (colon >= 0 && colon < tail.Length - 1)
                tail = tail[(colon + 1)..];

            return tail.Length > 24 ? tail[..24] + "…" : tail;
        }
    }
}
