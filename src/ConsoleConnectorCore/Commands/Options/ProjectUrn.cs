namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    /// <summary>
    /// Project URN command option.
    /// </summary>
    internal class ProjectUrn : CommandOption
    {
        public ProjectUrn()
        {
            this.Description = "Specify project URN for exchange creation, etc.";
        }

        public override string ToString()
        {
            return "ProjectURN[" + Description + "]";
        }
    }
}