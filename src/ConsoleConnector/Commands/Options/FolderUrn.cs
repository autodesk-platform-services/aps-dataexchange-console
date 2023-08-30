namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    /// <summary>
    /// FolderUrn command option.
    /// </summary>
    /// <seealso cref="CommandOption" />
    internal class FolderUrn : CommandOption
    {
        public FolderUrn()
        {
            this.Description = "Specify folder URN for exchange creation, etc.";
        }

        public override string ToString()
        {
            return "FolderURN[" + Description + "]";
        }
    }
}