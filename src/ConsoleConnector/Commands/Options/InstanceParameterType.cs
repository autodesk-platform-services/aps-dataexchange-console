using System;

namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{

    /// <summary>
    /// Parameter instance type command option.
    /// </summary>
    /// <seealso cref="CommandOption" />
    internal class InstanceParameterType : CommandOption
    {
        public new string Value { get; set; }

        public InstanceParameterType()
        {
            this.Description = "Specify type of parameter such as HostVolumeComputed, RelatedToMass, etc.";
        }

        public override void SetValue(string value)
        {
            //Enum.TryParse(value, true, out Autodesk.Parameters.Parameter parameter);
            Value = value;
        }

        public override string ToString()
        {
            return "ParameterType[" + Description + "]";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}