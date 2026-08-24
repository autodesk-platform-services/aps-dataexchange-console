using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to delete a model-level parameter.
    /// SDK: ElementDataModel.DeleteParameter (model level).
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(5, 3, 3)]
    public sealed class DeleteModelLevelParamSample : ISample
    {
        public string Name => "Delete Model Level Param";
        public string Description => "Delete a root/model-level parameter";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ParameterSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var name = Prompt.AskString("Parameter name", "ProjectPhase");
            var existing = session.Model.Parameters.FirstOrDefault(p => p.Name == name);
            if (existing == null)
            {
                TerminalUi.Error($"Model parameter not found: {name}");
                return;
            }

            var deleted = session.Model.DeleteParameter(existing.SchemaId);
            TerminalUi.Error(deleted ? $"  Deleted '{name}'." : $"  Delete failed for '{name}'.");
            if (deleted)
                await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
