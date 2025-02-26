using System;

namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    /// <summary>
    /// Primitive geometry command option.
    /// </summary>
    /// <seealso cref="CommandOption" />
    internal class PrimitiveGeometry : CommandOption
    {
        public new PrimitiveGeometryType Value { get; set; }
        public PrimitiveGeometry()
        {
            this.Description = "Specify primitive geometry type such as Line, Point, etc.";
        }

        public override string ToString()
        {
            return "PrimitiveGeometry[" + Description + "]";
        }

        public override void SetValue(string value)
        {
            Enum.TryParse(value, true, out PrimitiveGeometryType parameter);
            Value = parameter;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}