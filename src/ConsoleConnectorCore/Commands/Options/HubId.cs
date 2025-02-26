namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    /// <summary>
    /// HubId command option.
    /// </summary>
    /// <seealso cref="CommandOption" />
    internal class HubId : CommandOption
    {
        public HubId()
        {
            this.Description = "Specify hub Id for exchange creation, etc.";
        }

        public override string ToString()
        {
            return "HubId[" + Description + "]";
        }
    }
}