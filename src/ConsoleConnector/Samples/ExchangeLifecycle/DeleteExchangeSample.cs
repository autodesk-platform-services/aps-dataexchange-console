using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Models;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to permanently delete an exchange from Forma.
    /// SDK: IClient.DeleteExchangeAsync.
    /// Console plumbing: NavigationHelper (exchange picker), ExchangeSessionHelper.RemoveLoadedByFileUrn.
    /// Prerequisites: saved session folder.
    /// </summary>
    [SampleAddress(2, 6)]
    public sealed class DeleteExchangeSample : ISample
    {
        public string Name => "Delete Exchange";
        public string Description => "Delete a Data Exchange from your saved folder";

        public async Task RunAsync(SampleContext ctx)
        {
            // Console plumbing (not SDK): pick exchange from saved folder
            var picked = await NavigationHelper.PickExchangeAsync(ctx);
            if (picked == null)
                return;

            TerminalUi.Chat($"File URN: {picked.FileUrn}");
            if (!Prompt.AskBool("Delete this exchange from Forma?", defaultValue: false))
            {
                TerminalUi.Dim("Cancelled.");
                return;
            }

            // SDK: delete exchange from Forma
            TerminalUi.Chat("Deleting...");
            var response = await ctx.Client.DeleteExchangeAsync(picked.Project.ProjectId, picked.FileUrn);
            if (response.IsFailed)
            {
                var message = response.Errors.FirstOrDefault()?.Message ?? "Unknown error";
                TerminalUi.Error($"Failed: {message}");
                return;
            }

            // Console plumbing (not SDK): remove from in-memory session
            ExchangeSessionHelper.RemoveLoadedByFileUrn(ctx, picked.FileUrn);
            TerminalUi.Chat("Deleted.");
        }
    }
}
