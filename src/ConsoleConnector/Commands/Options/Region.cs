namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    /// <summary>
    /// Region for command option.
    /// </summary>
    internal class Region : CommandOption
    {
        public Region()
        {
            this.Description = "Specify region for exchange creation, etc.";
        }

        public override string ToString()
        {
            return "Region[" + Description + "]";
        }
    }
}