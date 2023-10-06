using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    internal class ParamName:CommandOption
    {
        public new Autodesk.Parameters.Parameter? Value { get; set; }
        public string ParameterName { get; set; }
        public ParamName()
        {
            this.Description = "Specify the name for parameter";
        }

        public override void SetValue(string value)
        {
            Value = null;
            ParameterName = value;
            if (Enum.TryParse(value, true, out Autodesk.Parameters.Parameter parameter))
                Value = parameter;

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
