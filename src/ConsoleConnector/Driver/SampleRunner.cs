using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Samples;

namespace ConsoleConnector.Driver
{
    internal static class SampleRunner
    {
        public static async Task<RunAllResult> RunAllAsync(SampleContext ctx, SessionData session)
        {
            var samples = SampleDiscovery.DiscoverSamples();
            var results = new List<SampleRunResult>();

            TerminalUi.Section("Run all samples", $"{samples.Count} samples · non-interactive · defaults");
            TerminalUi.Rule();

            foreach (var sample in samples)
            {
                TerminalUi.Section($"{sample.Key} — {sample.Sample.Name}");
                TerminalUi.Dim(sample.Sample.Description);

                try
                {
                    await sample.Sample.RunAsync(ctx).ConfigureAwait(false);
                    SessionStore.Capture(ctx, session);
                    SessionStore.Save(session);
                    results.Add(new SampleRunResult(sample.Key, sample.Sample.Name, null));
                }
                catch (Exception ex)
                {
                    TerminalUi.Error($"{ex}");
                    results.Add(new SampleRunResult(sample.Key, sample.Sample.Name, ex.ToString()));
                }

                TerminalUi.Rule();
            }

            return new RunAllResult(results);
        }

        internal sealed record SampleRunResult(string Key, string Name, string? Error);
        internal sealed record RunAllResult(IReadOnlyList<SampleRunResult> Results)
        {
            public int Passed => Results.Count(r => r.Error == null);
            public int Failed => Results.Count(r => r.Error != null);
        }
    }
}
