using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.Extensions.Storage.File;
using ConsoleConnector.Driver;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to keep connector state on disk with the SDK's file Storage, committing one entry at a time.
    /// SDK: Storage(path), Storage.Add(key, value, group), Storage.Save(key, group), Storage.Get&lt;T&gt;(key, group), Storage.Remove(key, group).
    /// Console plumbing: SessionStore.SessionDirectory (sample folder).
    /// Prerequisites: none.
    /// </summary>
    [SampleAddress(12, 1)]
    public sealed class PersistWithStorageSample : ISample
    {
        private const string Group = "ExchangeSettings";

        public string Name => "Persist State with Storage";
        public string Description => "Add entries in memory, save one key at a time, read them back";

        public Task RunAsync(SampleContext ctx)
        {
            var folder = Path.Combine(SessionStore.SessionDirectory, "storage-sample", Guid.NewGuid().ToString("N"));
            TerminalUi.Info($"Storage folder: {folder}");
            try
            {
                // SDK: file-backed key/value store rooted at a folder; groups are sub-folders
                var storage = new Storage(folder);

                TerminalUi.Info("Add only changes memory. Nothing is written until you Save that key.");
                // SDK: two entries in the same group
                storage.Add("LastSyncedRevision", "rev-42", Group);
                storage.Add("PendingDraft", "unsaved work", Group);

                TerminalUi.Info("Save(key, group) commits exactly one entry. Other pending entries stay in memory only.");
                // SDK: scoped save of a single key
                storage.Save("LastSyncedRevision", Group);
                TerminalUi.Detail("Files on disk", CountFiles(folder).ToString());

                TerminalUi.Info("A new Storage on the same folder only sees what was saved.");
                // SDK: read back through a fresh instance
                var reopened = new Storage(folder);
                TerminalUi.Detail("LastSyncedRevision", reopened.Get<string>("LastSyncedRevision", Group) ?? "(null)");
                TerminalUi.Detail("PendingDraft", reopened.Get<string>("PendingDraft", Group) ?? "(null)");

                TerminalUi.Info("Remove marks an entry for deletion; Save(key, group) then deletes its file.");
                // SDK: remove + scoped save deletes the on-disk entry
                reopened.Remove("LastSyncedRevision", Group);
                reopened.Save("LastSyncedRevision", Group);
                TerminalUi.Detail("Files on disk", CountFiles(folder).ToString());
                TerminalUi.Success("Storage round trip complete.");
            }
            finally
            {
                TryDelete(folder);
            }

            return Task.CompletedTask;
        }

        private static int CountFiles(string folder) =>
            Directory.Exists(folder) ? Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories).Count() : 0;

        private static void TryDelete(string folder)
        {
            try
            {
                if (Directory.Exists(folder))
                    Directory.Delete(folder, recursive: true);
            }
            catch (IOException ex)
            {
                TerminalUi.Dim($"Could not delete {folder}: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                TerminalUi.Dim($"Could not delete {folder}: {ex.Message}");
            }
        }
    }
}
