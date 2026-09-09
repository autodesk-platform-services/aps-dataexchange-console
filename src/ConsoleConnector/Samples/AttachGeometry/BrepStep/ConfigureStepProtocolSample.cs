using System;
using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Global STEP import protocol (203/214/225) affects translation.
    /// SDK: GeometryConfiguration.STEPProtocol.
    /// Console plumbing: GeometrySampleHelper.RequireGeometryConfiguration.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 1, 3)]
    public sealed class ConfigureStepProtocolSample : ISample
    {
        public string Name => "Configure STEP Protocol";
        public string Description => "View or set GeometryConfiguration.STEPProtocol";

        public Task RunAsync(SampleContext ctx)
        {
            var config = GeometrySampleHelper.RequireGeometryConfiguration(ctx);
            TerminalUi.Chat($"Current STEPProtocol: {config.STEPProtocol} ({(int)config.STEPProtocol})");
            TerminalUi.Chat("Common values: 203=ConfigurationControlledDesign, 214=AutomotiveDesign, 225=BuildingElements");
            var raw = Prompt.AskString("New protocol value (Enter to keep current)", ((int)config.STEPProtocol).ToString());
            if (string.IsNullOrWhiteSpace(raw))
                return Task.CompletedTask;

            if (!int.TryParse(raw, out var value) || !Enum.IsDefined(typeof(STEPProtocol), value))
            {
                TerminalUi.Error("Invalid STEP protocol value.");
            return Task.CompletedTask;
            }

            config.STEPProtocol = (STEPProtocol)value;
            TerminalUi.Success($"Updated STEPProtocol: {config.STEPProtocol}");
            return Task.CompletedTask;
        }
    }
}
