namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    /// <summary>
    /// ExchangeTitle command option.
    /// </summary>
    /// <seealso cref="CommandOption" />
    internal class ExchangeTitle : CommandOption
    {
        public ExchangeTitle()
        {
            this.Description = "Specify title for exchange.";
        }

        public override string ToString()
        {
            return "ExchangeTitle[" + Description + "]";
        }
    }
}