namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    /// <summary>
    /// HelpCommand command option.
    /// </summary>
    /// <seealso cref="CommandOption" />
    internal class HelpCommand : CommandOption
    {
        public HelpCommand()
        {
            this.Description = "Help";
        }

        public override string ToString()
        {
            return "Help[" + Description + "]";
        }
    }
}