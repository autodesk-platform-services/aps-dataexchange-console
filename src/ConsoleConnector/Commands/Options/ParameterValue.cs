namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    /// <summary>
    /// Parameter Value command option.
    /// </summary>
    /// <seealso cref="CommandOption" />
    internal class ParameterValue : CommandOption
    {
        public ParameterValue()
        {
            this.Description = "Specify value for parameter.";
        }
        public override string ToString()
        {
            return "ParameterValue[" + Description + "]";
        }
    }
}