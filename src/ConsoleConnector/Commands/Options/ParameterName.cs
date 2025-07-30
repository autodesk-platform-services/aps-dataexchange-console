using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    internal class ParameterName:CommandOption
    {
        public string Value { get; set; }
        public string SchemaName { get; set; }
        public ParameterName()
        {
            this.Description = "Specify the name for parameter";
        }

        public override void SetValue(string value)
        {
            Value = value;
            //We try to check command value is builtin parameter or not. If not then it is treated as a custom parameter.
            //Value = null;
            //SchemaName = value;
            //if (Enum.TryParse(value, true, out Autodesk.Parameters.Parameter parameter))
                //Value = parameter;

        }

        public override string ToString()
        {
            return "ParamName[" + Description + "]";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
