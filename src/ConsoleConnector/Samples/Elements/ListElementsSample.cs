using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using Spectre.Console;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: The difference between all elements (flat) and top-level only.
    /// SDK: ElementDataModel.Elements + TopLevelElements.
    /// Console plumbing: ElementSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(3, 3)]
    public sealed class ListElementsSample : ISample
    {
        public string Name => "List Elements";
        public string Description => "List all elements in the loaded model";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ElementSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var model = session.Model;
            var title = session.Details.DisplayName ?? session.Active.ExchangeFileUrn;
            var all = model.Elements.ToList();
            var topLevel = model.TopLevelElements.ToList();
            TerminalUi.Chat($"Elements in {title}:");
            TerminalUi.Chat($"  Total (flat):   {all.Count}");
            TerminalUi.Chat($"  Top-level only: {topLevel.Count}");
            AnsiConsole.WriteLine();
            if (all.Count == 0)
            {
                TerminalUi.Warning("  (none — run 3.1 Add Root Element to create one)");
                return;
            }

            TerminalUi.Chat("All elements (flat):");
            ElementPicker.PrintList(all);
        }
    }
}
