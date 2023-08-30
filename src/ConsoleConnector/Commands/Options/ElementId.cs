namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    /// <summary>
    /// ElementId command option.
    /// </summary>
    /// <seealso cref="CommandOption" />
    internal class ElementId : CommandOption
    {
        public ElementId()
        {
            this.Description = "Specify the element id to add parameter, etc.";
        }

        public override string ToString()
        {
            return "ElementId[" + Description + "]";
        }
    }
}