using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Interface;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;

namespace ConsoleConnector.Common
{
    internal static class ExchangeFolderPicker
    {
        internal static async Task<string?> PickFileUrnAsync(
            IHostingProvider hosting,
            string projectId,
            string folderId,
            SampleContext? ctx = null)
        {
            var exchanges = await ExchangeFolderListing.ListInFolderAsync(hosting, projectId, folderId);

            if (BatchMode.Enabled && ctx?.LastExchange?.FileUrn != null)
            {
                var preferred = exchanges.FirstOrDefault(e =>
                    string.Equals(e.FileUrn, ctx.LastExchange.FileUrn, StringComparison.OrdinalIgnoreCase));
                if (preferred != null)
                    return preferred.FileUrn;
            }

            var (outcome, entry, manual) = ListPicker.PickOneOrManual(
                "Pick an exchange",
                exchanges,
                e => ListPicker.FormatNameAndDetail(e.DisplayName, e.FileUrn),
                "Paste a file URN instead",
                () => Prompt.AskString("File URN", null));

            return outcome switch
            {
                ListPickOutcome.Item when entry != null => entry.FileUrn,
                ListPickOutcome.Manual => manual,
                _ => null,
            };
        }
    }
}
