using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to update a model-level parameter.
    /// SDK: ElementDataModel.UpdateParameter (model level).
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(5, 3, 2)]
    public sealed class UpdateModelLevelParamSample : ISample
    {
        public string Name => "Update Model Level Param";
        public string Description => "Update a root/model-level parameter";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ParameterSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var name = Prompt.AskString("Parameter name", "ProjectPhase");
            var existing = session.Model.Parameters.FirstOrDefault(p => p.Name == name);
            if (existing == null)
            {
                TerminalUi.Error($"Model parameter not found: {name}. Run 5.3.1 first.");
                return;
            }

            ParameterSampleHelper.PrintParameter(existing, "  Before: ");
            var newValue = Prompt.AskString("New value (string)", "Construction");
            var updated = session.Model.UpdateParameter(existing.SchemaId, newValue);
            ParameterSampleHelper.PrintParameter(updated, "  After:  ");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
