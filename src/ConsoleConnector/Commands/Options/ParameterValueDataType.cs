using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;

namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    internal class ParameterValueDataType:CommandOption
    {
        public bool IsValidDataType { get; private set; } = false;
        public new ParameterDataType Value { get; private set; } = ParameterDataType.String;
        public ParameterValueDataType()
        {
            this.Description = "Specify value type of parameter such as int,long,etc.";
        }

        public override void SetValue(string value)
        {
            IsValidDataType = false;
            if (Enum.TryParse(value, true, out ParameterDataType parameterDataType))
            {
                switch (parameterDataType)
                {
                    case ParameterDataType.Float64:
                        IsValidDataType = true;
                        Value = ParameterDataType.Float64;
                        break;
                    case ParameterDataType.Bool:
                        IsValidDataType = true;
                        Value = ParameterDataType.Bool;
                        break;
                    case ParameterDataType.Int64:
                        IsValidDataType = true;
                        Value = ParameterDataType.Float64;
                        break;
                    case ParameterDataType.Int32:
                        IsValidDataType = true;
                        Value = ParameterDataType.Int32;
                        break;
                    case ParameterDataType.String:
                        IsValidDataType = true;
                        Value = ParameterDataType.String;
                        break;
                }
            }
        }

        public override string ToString()
        {
            return "ParameterValueDataType[" + Description + "]";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
