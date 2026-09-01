using System;
using System.Linq;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;

namespace ConsoleConnector.Common
{
    internal static class LoadedExchangePicker
    {
        internal static ActiveExchange? Pick(SampleContext ctx, string? preferredTitle = null)
        {
            preferredTitle ??= ctx.ScenarioExchangeTitle;

            if (ctx.Exchanges.Count == 0)
                return null;

            if (!string.IsNullOrWhiteSpace(preferredTitle)
                && ctx.Exchanges.TryGetValue(preferredTitle, out var preferred))
            {
                return preferred;
            }

            if (ctx.Exchanges.Count == 1 || BatchMode.Enabled)
            {
                if (!string.IsNullOrWhiteSpace(ctx.LastExchangeTitle)
                    && ctx.Exchanges.TryGetValue(ctx.LastExchangeTitle, out var lastUsed))
                    return lastUsed;

                return ctx.Exchanges.Values.First();
            }

            var names = ctx.Exchanges.Keys.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();
            var defaultName = !string.IsNullOrWhiteSpace(ctx.LastExchangeTitle)
                && ctx.Exchanges.ContainsKey(ctx.LastExchangeTitle)
                    ? ctx.LastExchangeTitle
                    : names[0];

            var picked = ListPicker.PickOne(
                "Pick loaded exchange",
                names,
                n =>
                {
                    var active = ctx.Exchanges[n];
                    var versionSuffix = active.VersionNumber.HasValue ? $" (v{active.VersionNumber})" : string.Empty;
                    var detail = n == defaultName
                        ? $"{active.ExchangeFileUrn}{versionSuffix} (last used)"
                        : $"{active.ExchangeFileUrn}{versionSuffix}";
                    return ListPicker.FormatNameAndDetail(n, detail);
                });

            if (picked == null)
                return null;

            return ctx.Exchanges[picked];
        }
    }
}
