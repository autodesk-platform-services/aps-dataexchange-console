using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to delete multiple instance params by schema id.
    /// SDK: Element.DeleteInstanceParameters (batch).
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(5, 1, 8)]
    public sealed class DeleteInstanceParamsBatchSample : ISample
    {
        public string Name => "Delete Instance Params Batch";
        public string Description => "Delete multiple instance parameters in one batch";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ParameterSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var element = ParameterSampleHelper.PickElement(session.Model);
            if (element == null)
            {
                TerminalUi.Dim("Cancelled.");
                return;
            }

            if (element is not Element concrete)
            {
                TerminalUi.Chat("Unexpected element type.");
                return;
            }

            var schemaIds = element.InstanceParameters.Select(p => p.SchemaId).ToList();
            if (schemaIds.Count == 0)
            {
                TerminalUi.Warning("No instance parameters to delete.");
                return;
            }

            var confirm = Prompt.AskString($"Delete all {schemaIds.Count} instance params? [y/N]", "N");
            if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
            {
                TerminalUi.Dim("Cancelled.");
                return;
            }

            var count = concrete.DeleteInstanceParameters(schemaIds);
            TerminalUi.Success($"Deleted {count} parameter(s).");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
