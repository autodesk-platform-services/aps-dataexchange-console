using System;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Exchange-wide (model/root) params are separate from element params.
    /// SDK: ElementDataModel.AddParameterAsync (model level).
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(5, 3, 1)]
    public sealed class AddModelLevelParamSample : ISample
    {
        public string Name => "Add Model Level Param";
        public string Description => "Add a root/model-level custom parameter";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ParameterSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var name = BatchMode.Enabled
                ? ParameterSampleHelper.SuggestUniqueModelParamName(session.Model, "ProjectPhase")
                : Prompt.AskString("Parameter name", "ProjectPhase");
            var value = Prompt.AskString("Value (string)", "Design Development");
            if (string.IsNullOrWhiteSpace(name))
            {
                TerminalUi.Chat("Parameter name is required.");
                return;
            }

            var added = await ParameterSampleHelper.AddModelLevelParamAsync(session.Model, name, value);
            if (added == null)
            {
                TerminalUi.Error("Failed to add model parameter.");
                return;
            }

            ParameterSampleHelper.PrintParameter(added, "  ");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
