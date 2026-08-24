using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Parameters;
using ConsoleConnector.Driver;
using Spectre.Console;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Built-in schema ids come from Autodesk.Parameters.Group taxonomy.
    /// SDK: Autodesk.Parameters.Group taxonomy.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(5, 3, 9)]
    public sealed class ListAllParamGroupsAndSpecsSample : ISample
    {
        public string Name => "List Param Groups And Specs";
        public string Description => "List parameter groups and sample built-in schema ids";

        public Task RunAsync(SampleContext ctx)
        {
            TerminalUi.Chat("Parameter groups (Autodesk.Parameters.Group):");
            foreach (var group in Enum.GetValues(typeof(Group)).Cast<Group>())
                TerminalUi.Chat($"  {group}: {group.DisplayName()}");
            AnsiConsole.WriteLine();
            TerminalUi.Chat("Sample built-in schema ids:");
            PrintSpec("CurveElemLength", "autodesk.revit.parameter:curveElemLength-1.0.0");
            PrintSpec("HostAreaComputed", "autodesk.revit.parameter:hostAreaComputed-1.0.0");
            PrintSpec("HostVolumeComputed", "autodesk.revit.parameter:hostVolumeComputed-1.0.0");
            PrintSpec("WallBaseConstraint", "autodesk.revit.parameter:wallBaseConstraint-1.0.0");
            PrintSpec("PhaseCreated", "autodesk.revit.parameter:phaseCreated-1.0.0");
            PrintSpec("AllModelDescription", "autodesk.revit.parameter:allModelDescription-1.0.0");
            PrintSpec("IfcGuid", "autodesk.revit.parameter:ifcGuid-1.0.0");
            return Task.CompletedTask;
        }

        private static void PrintSpec(string name, string schemaId) =>
            TerminalUi.Chat($"  {name}: {schemaId}");
        }
}
