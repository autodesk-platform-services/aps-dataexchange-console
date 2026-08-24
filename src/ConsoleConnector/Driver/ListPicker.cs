using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

namespace ConsoleConnector.Driver
{
    internal enum ListPickOutcome
    {
        Item,
        Manual,
        Cancel,
    }

    internal static class ListPicker
    {
        private const string EscapeCancel = "\x1b";

        private sealed record TaggedChoice<T>(T? Item, ListPickOutcome Outcome) where T : notnull;

        /// <summary>Single-line row. No inline fg colors — they fight Spectre's highlight on the first (selected) row.</summary>
        public static string FormatNameAndDetail(string primary, string? secondary)
        {
            var name = string.IsNullOrWhiteSpace(primary) ? "(unnamed)" : primary.Trim();
            var namePart = $"[bold]{Markup.Escape(name)}[/]";
            if (string.IsNullOrWhiteSpace(secondary))
                return namePart;

            var detail = secondary.Trim();
            if (detail.Length > 56)
                detail = detail.Substring(0, 22) + "…" + detail.Substring(detail.Length - 28);

            return $"{namePart}  [dim]{Markup.Escape(detail)}[/]";
        }

        public static T? PickOne<T>(
            string title,
            IReadOnlyList<T> items,
            Func<T, string> formatRow,
            bool allowCancel = true)
            where T : notnull
        {
            if (items.Count == 0)
                return default;

            if (items.Count == 1 || BatchMode.Enabled)
                return items[0];

            var choices = items
                .Select(i => new TaggedChoice<T>(i, ListPickOutcome.Item))
                .ToList();

            if (allowCancel)
                choices.Add(new TaggedChoice<T>(default, ListPickOutcome.Cancel));

            var picked = PromptTagged(title, choices, formatRow);
            return picked.Outcome == ListPickOutcome.Item ? picked.Item : default;
        }

        public static (ListPickOutcome Outcome, T? Item, string? ManualText) PickOneOrManual<T>(
            string title,
            IReadOnlyList<T> items,
            Func<T, string> formatRow,
            string manualLabel,
            Func<string?> manualPrompt)
            where T : notnull
        {
            if (items.Count == 0)
            {
                if (BatchMode.Enabled)
                    return (ListPickOutcome.Cancel, default, null);

                var manualOnly = manualPrompt();
                return string.IsNullOrWhiteSpace(manualOnly)
                    ? (ListPickOutcome.Cancel, default, null)
                    : (ListPickOutcome.Manual, default, manualOnly.Trim());
            }

            if (BatchMode.Enabled)
                return (ListPickOutcome.Item, items[0], null);

            var choices = items
                .Select(i => new TaggedChoice<T>(i, ListPickOutcome.Item))
                .Append(new TaggedChoice<T>(default, ListPickOutcome.Manual))
                .Append(new TaggedChoice<T>(default, ListPickOutcome.Cancel))
                .ToList();

            var picked = PromptTagged(title, choices, formatRow, manualLabel);
            if (picked.Outcome == ListPickOutcome.Manual)
            {
                var manual = manualPrompt();
                return string.IsNullOrWhiteSpace(manual)
                    ? (ListPickOutcome.Cancel, default, null)
                    : (ListPickOutcome.Manual, default, manual.Trim());
            }

            if (picked.Outcome == ListPickOutcome.Cancel)
                return (ListPickOutcome.Cancel, default, null);

            return (ListPickOutcome.Item, picked.Item, null);
        }

        public static int PickBinary(string title, string optionA, string optionB)
        {
            if (BatchMode.Enabled)
                return 1;

            var (cancelled, picked) = TerminalUi.TryPick(
                title,
                new[] { optionA, optionB },
                choice => choice == optionA
                    ? $"[cyan]1[/]  [white]{Markup.Escape(optionA)}[/]"
                    : $"[cyan]2[/]  [white]{Markup.Escape(optionB)}[/]",
                cancelOnEscape: EscapeCancel);

            if (cancelled)
                return 0;

            return picked == optionA ? 1 : 2;
        }

        private static TaggedChoice<T> PromptTagged<T>(
            string title,
            IReadOnlyList<TaggedChoice<T>> choices,
            Func<T, string> formatRow,
            string? manualLabel = null)
            where T : notnull
        {
            var cancelChoice = choices.FirstOrDefault(c => c.Outcome == ListPickOutcome.Cancel);
            var (cancelled, picked) = TerminalUi.TryPickColored(
                title,
                choices,
                entry =>
                {
                    if (entry.Outcome == ListPickOutcome.Cancel)
                        return "[grey50]← Cancel[/]";
                    if (entry.Outcome == ListPickOutcome.Manual)
                        return $"[yellow]›[/]  [yellow]{Markup.Escape(manualLabel ?? "Enter manually")}[/]";

                    return formatRow(entry.Item!);
                },
                cancelChoice);

            if (cancelled && cancelChoice is not null)
                return cancelChoice;

            return picked;
        }
    }
}
