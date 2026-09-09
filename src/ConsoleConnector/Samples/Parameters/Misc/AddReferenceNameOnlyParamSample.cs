using System;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Reference params can be name-only without an entity id.
    /// SDK: Element.CreateReferenceNameOnlyParameters.
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(5, 3, 8)]
    public sealed class AddReferenceNameOnlyParamSample : ISample
    {
        public string Name => "Add Reference Name-Only Param";
        public string Description => "Add a reference parameter by name only";

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
                "autodesk.revit.parameter:phaseCreated-1.0.0");
            var referencedName = Prompt.AskString("Referenced entity name", "New Construction");
            concrete.CreateReferenceNameOnlyParameters(schemaId, referencedName);
            TerminalUi.Success($"Name-only reference set on '{element.Name}'.");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
