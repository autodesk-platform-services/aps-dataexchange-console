using Autodesk.DataExchange.SchemaObjects.Components;

namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    /// <summary>
    /// Parameter Value command option.
    /// </summary>
    /// <seealso cref="CommandOption" />
    internal class ParameterValue : CommandOption
    {
        public Autodesk.DataExchange.Core.Enums.ParameterDataType ParameterValueType { get; set; }
        public ParameterValue()
        {
            this.Description = "Specify value for parameter.";
        }

        public override void SetValue(string value)
        {
            base.SetValue(value);
            this.ParameterValueType = IdentifyDataType(value);
        }

        public override string ToString()
        {
            return "ParameterValue[" + Description + "]";
        }

        public static Autodesk.DataExchange.Core.Enums.ParameterDataType IdentifyDataType(string input)
        {
            if (int.TryParse(input, out _) && input.Contains(".") == false)
                return Autodesk.DataExchange.Core.Enums.ParameterDataType.Int32;
            if (long.TryParse(input, out _) && input.Contains(".") == false)
                return Autodesk.DataExchange.Core.Enums.ParameterDataType.Int64;
            if (double.TryParse(input, out _) && input.Contains("."))
                return Autodesk.DataExchange.Core.Enums.ParameterDataType.Float64;
            if (bool.TryParse(input, out _))
                return Autodesk.DataExchange.Core.Enums.ParameterDataType.Bool;
            return Autodesk.DataExchange.Core.Enums.ParameterDataType.String;
        }
    }
}