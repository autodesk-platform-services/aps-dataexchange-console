using System;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Type-level built-in params attach to a type name, not an element.
    /// SDK: ElementDataModel.CreateTypeParameterAsync (built-in).
    /// Console plumbing: ParameterSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(5, 2, 1)]
    public sealed class AddBuiltInTypeParamSample : ISample
    {
        public string Name => "Add Built-In Type Param";
        public string Description => "Add a built-in schema type parameter";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ParameterSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var typeName = ParameterSampleHelper.PickTypeName(session.Model);
            if (typeName == null)
                return;
            var schemaId = Prompt.AskString("Built-in schema id", ParameterSampleHelper.DefaultBuiltInSchemaId(ctx));
            var valueText = Prompt.AskString("Value (number)", "10.333");
            if (!double.TryParse(valueText, out var value))
            {
                TerminalUi.Error("Invalid numeric value.");
                return;
            }

            var parameter = ParameterSampleHelper.CreateBuiltInParameter(schemaId, value);
            Autodesk.DataExchange.Interface.IParameter added;
            try
            {
                added = await session.Model.CreateTypeParameterAsync(typeName, parameter);
        }
            catch (Autodesk.DataExchange.Exceptions.NotFoundException)
            {
                TerminalUi.Error($"Type '{typeName}' was not found. Run 5.2.6 to list types, or 3.1 to create one.");
                return;
            }

            if (added == null)
            {
                TerminalUi.Error("Failed to add type parameter.");
                return;
            }

            ParameterSampleHelper.PersistBuiltInSchemaId(ctx, schemaId);
            ParameterSampleHelper.PrintParameter(added, "  ");
            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
