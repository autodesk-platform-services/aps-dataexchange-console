using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Batch create is more efficient than one-by-one.
    /// SDK: Element.CreateInstanceParametersAsync (batch).
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(5, 1, 6)]
    public sealed class AddInstanceParamsBatchSample : ISample
    {
        public string Name => "Add Instance Params Batch";
        public string Description => "Add multiple instance parameters in one batch";

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

            var group = ParameterSampleHelper.GeneralGroupId;
            var definitions = new List<Parameter>
            {
                ParameterSampleHelper.CreateCustomParameter("BatchBool", true, group),
                ParameterSampleHelper.CreateCustomParameter("BatchLength", 12.5, ParameterSampleHelper.DimensionsGroupId),
                ParameterSampleHelper.CreateCustomParameter("BatchLabel", "batch-value", ParameterSampleHelper.GraphicsGroupId),
            };

            var added = (await concrete.CreateInstanceParametersAsync(definitions)).ToList();
            TerminalUi.Success($"Added {added.Count} parameters.");
            ParameterSampleHelper.PrintParameters(added, "Batch result");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
