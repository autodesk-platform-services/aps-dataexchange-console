namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    /// <summary>
    /// ElementId command option.
    /// </summary>
    /// <seealso cref="CommandOption" />
    internal class ExchangeFileFormat : CommandOption
    {

        public ExchangeFileFormat()
        {
            this.Description = "Specify the exchange download file format.(STEP[Default],OBJ)";
        }

        public override string ToString()
        {
            return "ExchangeFileFormat[" + Description + "](Optional)";
        }
    }
}