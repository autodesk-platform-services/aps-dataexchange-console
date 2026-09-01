using System;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Reference params link to another entity by id.
    /// SDK: Element.CreateReferenceParameter.
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(5, 3, 7)]
    public sealed class AddReferenceParamSample : ISample
    {
        public string Name => "Add Reference Param";
        public string Description => "Add a reference parameter with entity id";

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

            var schemaId = Prompt.AskString(
                "Built-in schema id",
                "autodesk.revit.parameter:wallBaseConstraint-1.0.0");
            var referenceId = Prompt.AskString("Reference entity id", "1C4F1B4A52597F316FE15C0533238314EEA43E75");
            concrete.CreateReferenceParameter(schemaId, referenceId);
            TerminalUi.Success($"Reference parameter set on '{element.Name}'.");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
