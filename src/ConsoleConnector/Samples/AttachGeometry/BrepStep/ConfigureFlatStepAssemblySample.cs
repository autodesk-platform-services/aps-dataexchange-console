using System;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Flat vs hierarchical STEP assembly import behavior.
    /// SDK: GeometryConfiguration.IsFlatSTEPAssembly.
    /// Console plumbing: GeometrySampleHelper.RequireGeometryConfiguration.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(4, 1, 5)]
    public sealed class ConfigureFlatStepAssemblySample : ISample
    {
        public string Name => "Configure Flat STEP Assembly";
        public string Description => "View or set GeometryConfiguration.IsFlatSTEPAssembly";

        public Task RunAsync(SampleContext ctx)
        {
            var config = GeometrySampleHelper.RequireGeometryConfiguration(ctx);
            TerminalUi.Chat($"Current IsFlatSTEPAssembly: {config.IsFlatSTEPAssembly}");
            var flat = Prompt.AskBool("Flatten STEP assembly?", config.IsFlatSTEPAssembly);
            config.IsFlatSTEPAssembly = flat;
            TerminalUi.Success($"Updated IsFlatSTEPAssembly: {config.IsFlatSTEPAssembly}");
            return Task.CompletedTask;
        }
    }
}
