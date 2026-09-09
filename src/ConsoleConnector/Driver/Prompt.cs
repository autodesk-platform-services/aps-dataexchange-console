using System;
using System.Globalization;
using Spectre.Console;

namespace ConsoleConnector.Driver
{
    internal static class Prompt
    {
        public static string AskString(string label, string? defaultValue = null)
        {
            if (BatchMode.Enabled)
                return defaultValue ?? string.Empty;

            var prompt = new TextPrompt<string>($"[cyan]{Markup.Escape(label)}[/]")
                .PromptStyle("white")
                .AllowEmpty();

            if (defaultValue != null)
                prompt.DefaultValue(defaultValue).ShowDefaultValue();

            var input = AnsiConsole.Prompt(prompt)?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(input))
                return defaultValue ?? string.Empty;

            return input;
        }

        public static int AskInt(string label, int? defaultValue = null)
        {
            while (true)
            {
                var raw = AskString(label, defaultValue?.ToString(CultureInfo.InvariantCulture));
                if (defaultValue.HasValue && string.IsNullOrEmpty(raw))
                    return defaultValue.Value;

                if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                    return value;

                TerminalUi.Warning("Enter a whole number.");
            }
        }

        public static double AskDouble(string label, double? defaultValue = null)
        {
            while (true)
            {
                var raw = AskString(label, defaultValue?.ToString(CultureInfo.InvariantCulture));
                if (defaultValue.HasValue && string.IsNullOrEmpty(raw))
                    return defaultValue.Value;

                if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                    return value;

                TerminalUi.Warning("Enter a number.");
            }
        }

        public static bool AskBool(string label, bool defaultValue = true)
        {
            if (BatchMode.Enabled)
                return defaultValue;

            return AnsiConsole.Confirm(
                $"[cyan]{Markup.Escape(label)}[/]",
                defaultValue);
        }

        public static string AskJumpKey(string prompt = "Sample key")
        {
            return AskString(prompt, null);
        }
    }
}
