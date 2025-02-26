namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    /// <summary>
    /// FolderUrn command option.
    /// </summary>
    /// <seealso cref="CommandOption" />
    internal class FolderUrl : CommandOption
    {
        public FolderUrl()
        {
            this.Description = "Specify folder URL for exchange creation, etc.";
        }

        public override string ToString()
        {
            return "FolderURL[" + Description + "](Optional)";
        }
    }
}