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
    /// What you learn: How to update multiple instance params in one call.
    /// SDK: Element.UpdateInstanceParameters (batch).
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(5, 1, 7)]
    public sealed class UpdateInstanceParamsBatchSample : ISample
    {
        public string Name => "Update Instance Params Batch";
        public string Description => "Update multiple instance parameters in one batch";

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

            var parameters = element.InstanceParameters.ToList();
            if (parameters.Count == 0)
            {
                TerminalUi.Warning("No instance parameters. Run 5.1.2 or 5.1.6 first.");
                return;
            }

            ParameterSampleHelper.PrintParameters(parameters, "Before update");
            var updates = new Dictionary<string, ParameterDataType>();
            foreach (var parameter in parameters.Take(3))
            {
                if (parameter.Value is bool)
                    updates[parameter.SchemaId] = !(bool)parameter.Value;
                else if (parameter.Value is double d)
                    updates[parameter.SchemaId] = d + 1.0;
                else if (parameter.Value is long l)
                    updates[parameter.SchemaId] = l + 1L;
                else if (parameter.Value is string s)
                    updates[parameter.SchemaId] = s + "-updated";
            }

            var results = concrete.UpdateInstanceParameters(updates).ToList();
            TerminalUi.Success($"Updated {results.Count} parameters.");
            ParameterSampleHelper.PrintParameters(results, "After update");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
