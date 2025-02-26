namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    /// <summary>
    /// Base class for CLI commands
    /// </summary>
    internal abstract class CommandOption
    {
        /// <summary>
        /// Description about the command.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; } = "Description not found.";

        /// <summary>
        /// Command input value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }

        /// <summary>
        /// Set command input.
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void SetValue(string value)
        {
            Value = value;
        }

        public virtual bool IsValid()
        {
            return string.IsNullOrEmpty(Value)==false;
        }

    }
}
