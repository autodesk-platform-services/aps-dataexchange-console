using System;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to add custom type params with a parameter group.
    /// SDK: ElementDataModel.CreateTypeParameterAsync (custom).
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(5, 2, 2)]
    public sealed class AddCustomTypeParamSample : ISample
    {
        public string Name => "Add Custom Type Param";
        public string Description => "Add a custom type parameter";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ParameterSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var typeName = ParameterSampleHelper.PickTypeName(session.Model);
            if (typeName == null)
                return;
            var name = Prompt.AskString("Parameter name", "TypeWidth");
            var valueText = Prompt.AskString("Value (number)", "2.5");
            if (string.IsNullOrWhiteSpace(name) || !double.TryParse(valueText, out var value))
            {
                TerminalUi.Chat("Name and numeric value are required.");
                return;
            }

            var added = await ParameterSampleHelper.AddCustomTypeParamAsync(
                session.Model,
                typeName,
                name,
                value,
                ParameterSampleHelper.DimensionsGroupId);
            if (added == null)
            {
                TerminalUi.Error("Failed to add type parameter.");
                return;
            }

            ParameterSampleHelper.PrintParameter(added, "  ");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
