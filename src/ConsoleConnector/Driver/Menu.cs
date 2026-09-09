using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConsoleConnector.Samples;
using Spectre.Console;

namespace ConsoleConnector.Driver
{
    internal static class Menu
    {
        private const string ShowAllChoice = "?  Show all samples";
        private const string JumpChoice = "›  Jump to sample key";
        private const string QuitChoice = "Q  Quit";
        private const string BackChoice = "←  Back";

        private static readonly (int Id, string Title)[] Categories =
        {
            (1,  "Navigation (Hubs, Projects, Folders)"),
            (2,  "Exchange Lifecycle (Create, Load, Sync)"),
            (3,  "Elements (Add, List, Edit, Delete)"),
            (4,  "Attach Geometry to Elements"),
            (5,  "Parameters on Elements & Exchanges"),
            (6,  "Create Design References & Instances"),
            (7,  "Download Whole Exchange (STEP / IFC / OBJ)"),
            (8,  "Revisions & What Changed in Versions"),
            (9,  "SDK Events & Viewables"),
            (10, "Debug & Telemetry"),
            (11, "E2E Workflow Scenarios"),
        };

        private static readonly Dictionary<int, (int Id, string Title)[]> CategorySubgroups =
            new()
            {
                [4] = new[]
                {
                    (1, "BREP / STEP"),
                    (2, "IFC"),
                    (3, "Mesh"),
                    (4, "Primitives"),
                    (5, "Advanced"),
                },
                [5] = new[]
                {
                    (1, "Instance"),
                    (2, "Type"),
                    (3, "Misc"),
                },
            };

        private static readonly Regex SampleKeyPattern = new(
            @"^(?<cat>\d+)\.(?<sample>\d+)(?:\.(?<sub>\d+))?$",
            RegexOptions.Compiled);

        public static async Task RunAsync(SampleContext ctx, SessionData session)
        {
            var samples = SampleDiscovery.DiscoverSamples();

            while (true)
            {
                var choice = PickMainMenu();
                switch (choice)
                {
                    case ShowAllChoice:
                        PrintAll(samples);
                        TerminalUi.WaitForEnter();
                        continue;
                    case JumpChoice:
                        await TryJumpAsync(samples, ctx, session);
                        continue;
                    case QuitChoice:
                        TerminalUi.Goodbye();
                        return;
                    default:
                        if (!TryParseCategoryChoice(choice, out var categoryId))
                            continue;
                        if (!await RunCategoryAsync(categoryId, samples, ctx, session))
                            return;
                        break;
                }
            }
        }

        private static string PickMainMenu()
        {
            var choices = Categories
                .Select(c => $"{c.Id,2}  {c.Title}")
                .Concat(new[] { ShowAllChoice, JumpChoice, QuitChoice })
                .ToArray();

            return TerminalUi.Pick("Categories", choices, FormatMainMenuChoice);
        }

        private static string FormatMainMenuChoice(string choice)
        {
            if (choice == ShowAllChoice)
                return "[grey50]?[/]  [grey50]Show all samples[/]";
            if (choice == JumpChoice)
                return "[yellow]›[/]  [yellow]Jump to sample key[/]";
            if (choice == QuitChoice)
                return "[red]Q[/]  [red]Quit[/]";
            if (TryParseCategoryChoice(choice, out var id))
            {
                var title = Categories.First(c => c.Id == id).Title;
                return $"[cyan]{id,2}[/]  [white]{Markup.Escape(title)}[/]";
            }

            return Markup.Escape(choice);
        }

        private static async Task TryJumpAsync(IReadOnlyList<SampleRef> samples, SampleContext ctx, SessionData session)
        {
            var key = Prompt.AskJumpKey("Sample key (e.g. 4.1, 4.4.2, 5.2.1)");
            if (TryResolveSample(key, samples, out var jumpSample))
                await RunSampleAsync(jumpSample, ctx, session);
            else
                TerminalUi.Warning("Unknown sample key. Use ? to browse all samples.");
        }

        private static async Task<bool> RunCategoryAsync(int categoryId, IReadOnlyList<SampleRef> samples, SampleContext ctx, SessionData session)
        {
            var title = Categories.First(c => c.Id == categoryId).Title;

            if (CategorySubgroups.TryGetValue(categoryId, out var subgroups))
                return await RunCategoryWithSubgroupsAsync(categoryId, title, subgroups, samples, ctx, session);

            return await RunSampleListAsync(
                title,
                samples.Where(s => s.CategoryId == categoryId),
                samples,
                ctx,
                session,
                backLabel: "Back to categories");
        }

        private static async Task<bool> RunCategoryWithSubgroupsAsync(
            int categoryId,
            string title,
            (int Id, string Title)[] subgroups,
            IReadOnlyList<SampleRef> samples,
            SampleContext ctx,
            SessionData session)
        {
            while (true)
            {
                var choice = PickSubgroupMenu(title, categoryId, subgroups, samples);
                switch (choice)
                {
                    case QuitChoice:
                        TerminalUi.Goodbye();
                        return false;
                    case JumpChoice:
                        await TryJumpAsync(samples, ctx, session);
                        continue;
                    default:
                        if (IsBack(choice))
                            return true;
                        if (!TryParseSubgroupChoice(choice, out var pick))
                            continue;

                        var (_, subgroupTitle) = subgroups.First(g => g.Id == pick);
                        var subgroupSamples = samples.Where(s =>
                            s.CategoryId == categoryId && s.SampleId == pick && s.SubSampleId > 0);

                        var backLabel = categoryId == 5 ? "Back to parameter groups" : "Back to geometry types";
                        if (!await RunSampleListAsync(
                                $"{title} — {subgroupTitle}",
                                subgroupSamples,
                                samples,
                                ctx,
                                session,
                                backLabel: backLabel))
                        {
                            return false;
                        }

                        break;
                }
            }
        }

        private static string PickSubgroupMenu(
            string title,
            int categoryId,
            (int Id, string Title)[] subgroups,
            IReadOnlyList<SampleRef> samples)
        {
            var choices = subgroups
                .Select(g =>
                {
                    var count = samples.Count(s =>
                        s.CategoryId == categoryId && s.SampleId == g.Id && s.SubSampleId > 0);
                    return $"{g.Id,2}  {g.Title} ({count})";
                })
                .Concat(new[] { JumpChoice, BackChoice, QuitChoice })
                .ToArray();

            var (_, choice) = TerminalUi.TryPick(
                title,
                choices,
                choice => FormatSubgroupChoice(choice, subgroups),
                BackChoice);
            return choice;
        }

        private static string FormatSubgroupChoice(string choice, (int Id, string Title)[] subgroups)
        {
            if (choice == JumpChoice)
                return "[yellow]›[/]  [yellow]Jump to sample key[/]";
            if (IsBack(choice))
                return $"[grey50]{Markup.Escape(choice)}[/]";
            if (choice == QuitChoice)
                return "[red]Q[/]  [red]Quit[/]";
            if (TryParseSubgroupChoice(choice, out var id))
            {
                var (_, groupTitle) = subgroups.First(g => g.Id == id);
                var countStart = choice.LastIndexOf('(');
                var count = countStart >= 0
                    ? choice.Substring(countStart + 1).TrimEnd(')')
                    : string.Empty;
                return string.IsNullOrEmpty(count)
                    ? $"[cyan]{id,2}[/]  [white]{Markup.Escape(groupTitle)}[/]"
                    : $"[cyan]{id,2}[/]  [white]{Markup.Escape(groupTitle)}[/] [grey50]({count})[/]";
            }

            return Markup.Escape(choice);
        }

        private static async Task<bool> RunSampleListAsync(
            string title,
            IEnumerable<SampleRef> listSamples,
            IReadOnlyList<SampleRef> allSamples,
            SampleContext ctx,
            SessionData session,
            string backLabel)
        {
            var sampleList = listSamples
                .OrderBy(s => s.SampleId)
                .ThenBy(s => s.SubSampleId)
                .ToList();

            while (true)
            {
                var choice = PickSampleMenu(title, sampleList, backLabel);
                switch (choice)
                {
                    case QuitChoice:
                        TerminalUi.Goodbye();
                        return false;
                    case JumpChoice:
                        await TryJumpAsync(allSamples, ctx, session);
                        continue;
                    default:
                        if (IsBack(choice))
                            return true;
                        if (TryResolveSampleChoice(choice, sampleList, out var sample))
                        {
                            await RunSampleAsync(sample, ctx, session);
                            continue;
                        }

                        TerminalUi.Warning("Pick a sample, jump, back, or quit.");
                        break;
                }
            }
        }

        private static string PickSampleMenu(string title, IReadOnlyList<SampleRef> sampleList, string backLabel)
        {
            if (sampleList.Count == 0)
            {
                TerminalUi.Warning("(no samples registered yet)");
                var (_, nav) = TerminalUi.TryPick(title, new[] { BackChoice, QuitChoice }, FormatNavChoice, BackChoice);
                return nav;
            }

            var backEntry = $"{BackChoice} ({backLabel})";
            var choices = sampleList
                .Select(s => $"{s.Key}  {s.Sample.Name} — {s.Sample.Description}")
                .Concat(new[] { JumpChoice, backEntry, QuitChoice })
                .ToArray();

            var (_, choice) = TerminalUi.TryPick(title, choices, FormatSampleChoice, backEntry);
            return choice;
        }

        private static string FormatNavChoice(string choice)
        {
            if (choice == QuitChoice)
                return "[red]Q[/]  [red]Quit[/]";
            if (IsBack(choice))
                return $"[grey50]{Markup.Escape(choice)}[/]";
            return Markup.Escape(choice);
        }

        private static string FormatSampleChoice(string choice)
        {
            if (choice == JumpChoice)
                return "[yellow]›[/]  [yellow]Jump to sample key[/]";
            if (choice == QuitChoice)
                return "[red]Q[/]  [red]Quit[/]";
            if (IsBack(choice))
                return $"[grey50]{Markup.Escape(choice)}[/]";

            var parts = choice.Split(new[] { " — " }, 2, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                var head = parts[0].Split(new[] { "  " }, 2, StringSplitOptions.None);
                if (head.Length == 2)
                {
                    return $"[cyan]{Markup.Escape(head[0])}[/]  [white]{Markup.Escape(head[1])}[/] [grey50]— {Markup.Escape(parts[1])}[/]";
                }
            }

            return Markup.Escape(choice);
        }

        private static async Task RunSampleAsync(SampleRef sampleRef, SampleContext ctx, SessionData session)
        {
            TerminalUi.Section(sampleRef.Sample.Name);
            AnsiConsole.MarkupLine(
                $"[cyan]{Markup.Escape(sampleRef.Key)}[/] [grey50]— {Markup.Escape(sampleRef.Sample.Description)}[/]");
            TerminalUi.Rule();

            try
            {
                await sampleRef.Sample.RunAsync(ctx);
                SessionStore.Capture(ctx, session);
                SessionStore.Save(session);
            }
            catch (Exception ex)
            {
                TerminalUi.Error($"{ex}");
            }

            TerminalUi.WaitForEnter("Sample done — press Enter to return to the menu");
        }

        private static void PrintAll(IReadOnlyList<SampleRef> samples)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Grey)
                .AddColumn(new TableColumn("[bold]Key[/]").Centered())
                .AddColumn("[bold]Sample[/]")
                .AddColumn("[bold]Description[/]");

            foreach (var (id, title) in Categories)
            {
                table.AddRow(
                    $"[cyan]{id}[/]",
                    $"[bold]{Markup.Escape(title)}[/]",
                    "[dim]category[/]");

                if (CategorySubgroups.TryGetValue(id, out var subgroups))
                {
                    foreach (var (subgroupId, subgroupTitle) in subgroups)
                    {
                        foreach (var sample in samples
                                     .Where(s => s.CategoryId == id && s.SampleId == subgroupId && s.SubSampleId > 0)
                                     .OrderBy(s => s.SubSampleId))
                        {
                            table.AddRow(
                                $"[cyan]{Markup.Escape(sample.Key)}[/]",
                                Markup.Escape(sample.Sample.Name),
                                Markup.Escape(sample.Sample.Description));
                        }
                    }
                }
                else
                {
                    foreach (var sample in samples.Where(s => s.CategoryId == id).OrderBy(s => s.SampleId).ThenBy(s => s.SubSampleId))
                    {
                        table.AddRow(
                            $"[cyan]{Markup.Escape(sample.Key)}[/]",
                            Markup.Escape(sample.Sample.Name),
                            Markup.Escape(sample.Sample.Description));
                    }
                }
            }

            AnsiConsole.Write(table);
            TerminalUi.Dim("Use › Jump to sample key from any menu, e.g. 4.1 or 5.2.1.");
        }

        private static bool TryResolveSample(string? input, IReadOnlyList<SampleRef> samples, out SampleRef sample)
        {
            sample = null!;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var match = SampleKeyPattern.Match(input.Trim());
            if (!match.Success)
                return false;

            var cat = int.Parse(match.Groups["cat"].Value);
            var samp = int.Parse(match.Groups["sample"].Value);
            var sub = match.Groups["sub"].Success ? int.Parse(match.Groups["sub"].Value) : 0;

            var found = samples.FirstOrDefault(s =>
                s.CategoryId == cat && s.SampleId == samp && s.SubSampleId == sub);

            if (found == null)
                return false;

            sample = found;
            return true;
        }

        private static bool TryParseCategoryChoice(string choice, out int categoryId)
        {
            categoryId = 0;
            var token = choice.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (token == null || !int.TryParse(token, out var parsed))
                return false;

            categoryId = parsed;
            return Categories.Any(c => c.Id == parsed);
        }

        private static bool TryParseSubgroupChoice(string choice, out int subgroupId)
        {
            subgroupId = 0;
            var token = choice.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            return token != null && int.TryParse(token, out subgroupId);
        }

        private static bool TryResolveSampleChoice(string choice, IReadOnlyList<SampleRef> sampleList, out SampleRef sample)
        {
            sample = null!;
            var key = choice.Trim().Split(new[] { "  " }, 2, StringSplitOptions.None)[0];
            var found = sampleList.FirstOrDefault(s => s.Key == key);
            if (found == null)
                return false;

            sample = found;
            return true;
        }

        private static bool IsBack(string choice) =>
            choice.StartsWith(BackChoice, StringComparison.Ordinal);
    }
}
