using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;

namespace ConsoleConnector.Driver
{
    internal static class TerminalUi
    {
#if NET48
        private static bool UsePlainBatch => BatchMode.Enabled;
#endif

        public static void ShowBanner()
        {
            if (!BatchMode.Enabled)
                AnsiConsole.Clear();

            if (BatchMode.Enabled)
            {
                Console.WriteLine("Console Connector — batch run");
                return;
            }

            var logo = new FigletText("DXSDK")
                .Color(Color.Cyan1);

            var panel = new Panel(logo)
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Cyan1)
                .Header("[bold white]Console Connector[/]", Justify.Center)
                .Padding(1, 0);

            AnsiConsole.Write(panel);
            AnsiConsole.MarkupLine("[dim]Interactive SDK samples · arrow keys to navigate · Enter to select · Esc back[/]");
            AnsiConsole.Write(new Rule("[dim][/]").RuleStyle("grey37"));
        }

        public static void Section(string title, string? subtitle = null)
        {
#if NET48
            if (UsePlainBatch)
            {
                Console.WriteLine();
                Console.WriteLine(subtitle == null ? title : $"{title} — {subtitle}");
                return;
            }
#endif
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold cyan]{Markup.Escape(title)}[/]");
            if (!string.IsNullOrWhiteSpace(subtitle))
                AnsiConsole.MarkupLine($"[dim]{Markup.Escape(subtitle)}[/]");
        }

        public static void Rule(string? label = null)
        {
#if NET48
            if (UsePlainBatch)
            {
                Console.WriteLine(new string('─', 16));
                return;
            }
#endif
            AnsiConsole.Write(new Rule(label == null ? "[dim][/]" : $"[dim]{Markup.Escape(label)}[/]")
                .RuleStyle("grey37"));
        }

        public static void Info(string message)
        {
#if NET48
            if (UsePlainBatch) { Console.WriteLine("> " + message); return; }
#endif
            AnsiConsole.MarkupLine($"[deepskyblue1]>[/] [deepskyblue1]{Markup.Escape(message)}[/]");
        }

        public static void Success(string message)
        {
            if (BatchMode.Enabled)
            {
                Console.WriteLine("+ " + message);
                return;
            }

            AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
        }

        public static void Warning(string message)
        {
#if NET48
            if (UsePlainBatch) { Console.WriteLine("! " + message); return; }
#endif
            AnsiConsole.MarkupLine($"[yellow]![/] [yellow]{Markup.Escape(message)}[/]");
        }

        public static void Error(string message)
        {
#if NET48
            if (UsePlainBatch) { Console.WriteLine("X " + message); return; }
#endif
            AnsiConsole.MarkupLine($"[red]X[/] [red]{Markup.Escape(message)}[/]");
        }

        public static void Dim(string message)
        {
#if NET48
            if (UsePlainBatch) { Console.WriteLine(message); return; }
#endif
            AnsiConsole.MarkupLine($"[grey50]{Markup.Escape(message)}[/]");
        }

        /// <summary>Sample / SDK output — distinct from system status messages.</summary>
        public static void Chat(string message)
        {
#if NET48
            if (UsePlainBatch) { Console.WriteLine(message); return; }
#endif
            AnsiConsole.MarkupLine($"[grey84]{Markup.Escape(message)}[/]");
        }

        public static void Detail(string label, string value)
        {
#if NET48
            if (UsePlainBatch) { Console.WriteLine($"  {label}: {value}"); return; }
#endif
            AnsiConsole.MarkupLine($"  [grey50]{Markup.Escape(label)}:[/] [white]{Markup.Escape(value)}[/]");
        }

        public static void Event(string message)
        {
#if NET48
            if (UsePlainBatch) { Console.WriteLine("* " + message); return; }
#endif
            AnsiConsole.MarkupLine($"[magenta1]*[/] [magenta1]{Markup.Escape(message)}[/]");
        }

        public static void Progress(string message)
        {
#if NET48
            if (UsePlainBatch) { Console.WriteLine("... " + message); return; }
#endif
            AnsiConsole.MarkupLine($"... [cyan]{Markup.Escape(message)}[/]");
        }

        public static void WriteResultPanel(string title, params (string Label, string Value)[] rows)
        {
#if NET48
            if (UsePlainBatch)
            {
                Console.WriteLine(title);
                foreach (var row in rows)
                    Console.WriteLine($"  {row.Label}: {row.Value}");
                return;
            }
#endif
            var content = string.Join(
                "\n",
                rows.Select(r => $"[grey50]{Markup.Escape(r.Label)}[/]  [white]{Markup.Escape(r.Value)}[/]"));

            WriteMarkupPanel(title, content);
        }

        public static void WriteTable<T>(
            string? title,
            IReadOnlyList<T> items,
            params (string Header, Func<T, string> Cell)[] columns)
        {
#if NET48
            if (UsePlainBatch)
            {
                if (!string.IsNullOrWhiteSpace(title))
                    Console.WriteLine(title);
                if (items.Count == 0)
                {
                    Console.WriteLine("! No items.");
                    return;
                }

                Console.WriteLine(string.Join(" | ", columns.Select(c => c.Header)));
                foreach (var item in items)
                    Console.WriteLine(string.Join(" | ", columns.Select(c => c.Cell(item))));
                return;
            }
#endif
            if (!string.IsNullOrWhiteSpace(title))
                Section(title);

            if (items.Count == 0)
            {
                Warning("No items.");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Grey);

            foreach (var (header, _) in columns)
                table.AddColumn($"[bold]{Markup.Escape(header)}[/]");

            foreach (var item in items)
            {
                table.AddRow(columns.Select(c => Markup.Escape(c.Cell(item))).ToArray());
            }

            AnsiConsole.Write(table);
        }

        public static void WritePanel(string title, string content)
        {
#if NET48
            if (UsePlainBatch)
            {
                Console.WriteLine(title);
                Console.WriteLine(content);
                return;
            }
#endif
            AnsiConsole.Write(
                new Panel(Markup.Escape(content))
                    .Header($"[bold]{Markup.Escape(title)}[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.Grey)
                    .Padding(1, 1));
        }

        public static void WriteMarkupPanel(string title, string markupContent)
        {
#if NET48
            if (UsePlainBatch)
            {
                Console.WriteLine(title);
                Console.WriteLine(markupContent);
                return;
            }
#endif
            AnsiConsole.Write(
                new Panel(new Markup(markupContent))
                    .Header($"[bold]{Markup.Escape(title)}[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.Grey)
                    .Padding(1, 1));
        }

        public static string Pick(string title, params string[] choices)
        {
            return Pick(title, choices, static c => Markup.Escape(c));
        }

        public static T Pick<T>(string title, IEnumerable<T> choices, Func<T, string> toDisplay)
            where T : notnull
        {
            if (BatchMode.Enabled)
                return choices.First();

            var (cancelled, value) = TryPick(title, choices, toDisplay);
            if (cancelled)
                throw new InvalidOperationException("Pick cancelled.");

            return value;
        }

        /// <summary>Returns (cancelled: true) when the user presses Esc (requires cancelOnEscape).</summary>
        public static (bool Cancelled, T Value) TryPick<T>(
            string title,
            IEnumerable<T> choices,
            Func<T, string> toDisplay,
            T? cancelOnEscape = default)
            where T : notnull
        {
            var list = choices.ToList();
            if (BatchMode.Enabled)
            {
                if (cancelOnEscape is not null)
                {
                    var first = list.FirstOrDefault(c =>
                        !EqualityComparer<T>.Default.Equals(c, cancelOnEscape));
                    if (first is not null)
                        return (false, first);
                }

                return (false, list[0]);
            }

            var hint = cancelOnEscape is null ? string.Empty : " [grey50](Esc back)[/]";
            var prompt = new SelectionPrompt<T>()
                .Title($"[bold]{Markup.Escape(title)}[/]{hint}")
                .PageSize(Math.Min(15, Math.Max(list.Count, 5)))
                .HighlightStyle(new Style(foreground: Color.Cyan1, decoration: Decoration.Bold))
                .UseConverter(toDisplay)
                .AddChoices(list);

            if (cancelOnEscape is not null)
                prompt.AddCancelResult(cancelOnEscape);

            var picked = AnsiConsole.Prompt(prompt);
            if (cancelOnEscape is not null && EqualityComparer<T>.Default.Equals(picked, cancelOnEscape))
                return (true, picked);

            return (false, picked);
        }

        public static T PickColored<T>(string title, IEnumerable<T> choices, Func<T, string> toDisplay)
            where T : notnull
        {
            return Pick(title, choices, toDisplay);
        }

        /// <summary>Returns (cancelled: true) when the user presses Esc (requires cancelOnEscape).</summary>
        public static (bool Cancelled, T Value) TryPickColored<T>(
            string title,
            IEnumerable<T> choices,
            Func<T, string> toDisplay,
            T? cancelOnEscape = default)
            where T : notnull
        {
            return TryPick(title, choices, toDisplay, cancelOnEscape);
        }

        public static async Task RunWithStatusAsync(string message, Func<Task> action)
        {
            if (BatchMode.Enabled)
            {
                Progress(message);
                await action().ConfigureAwait(false);
                return;
            }

            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("cyan"))
                .StartAsync(message, async ctx =>
                {
                    ctx.Status(message);
                    await action().ConfigureAwait(false);
                });
        }

        public static T RunWithStatus<T>(string message, Func<T> action)
        {
            if (BatchMode.Enabled)
            {
                Progress(message);
                return action();
            }

            return AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("cyan"))
                .Start(message, ctx =>
                {
                    ctx.Status(message);
                    return action();
                });
        }

        public static void WaitForEnter(string hint = "Press Enter to return to the menu")
        {
            if (BatchMode.Enabled)
                return;

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[dim]{Markup.Escape(hint)}[/] [grey50](Esc)[/]");
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key is ConsoleKey.Enter or ConsoleKey.Escape)
                    break;
            }
        }

        public static void Goodbye()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[dim]Goodbye.[/]");
        }
    }
}
