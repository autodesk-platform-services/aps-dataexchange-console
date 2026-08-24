using System;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: STEP tolerance is a global geometry configuration setting.
    /// SDK: GeometryConfiguration.STEPTolerance.
    /// Console plumbing: GeometrySampleHelper.RequireGeometryConfiguration.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(4, 1, 4)]
    public sealed class ConfigureStepToleranceSample : ISample
    {
        public string Name => "Configure STEP Tolerance";
        public string Description => "View or set GeometryConfiguration.STEPTolerance";

        public Task RunAsync(SampleContext ctx)
        {
            var config = GeometrySampleHelper.RequireGeometryConfiguration(ctx);
            TerminalUi.Chat($"Current STEPTolerance: {config.STEPTolerance}");
            TerminalUi.Chat($"ValidateSTEPTolerance: {config.ValidateSTEPTolerance()}");
            var raw = Prompt.AskString("New tolerance (Enter to keep current)", config.STEPTolerance.ToString());
            if (string.IsNullOrWhiteSpace(raw))
                return Task.CompletedTask;

            if (!double.TryParse(raw, out var tolerance))
            {
                TerminalUi.Error("Invalid tolerance.");
            return Task.CompletedTask;
            }

            config.STEPTolerance = tolerance;
            TerminalUi.Success($"Updated STEPTolerance: {config.STEPTolerance}");
            TerminalUi.Chat($"ValidateSTEPTolerance: {config.ValidateSTEPTolerance()}");
            return Task.CompletedTask;
        }
    }
}
