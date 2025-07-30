using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using Autodesk.DataExchange.DataModels;

namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    internal class ParameterValueDataType : CommandOption
    {
        public bool IsValidDataType { get; private set; } = false;
        public new ParameterDataTypeEnum Value { get; private set; } = ParameterDataTypeEnum.String;
        public ParameterValueDataType()
        {
            this.Description = "Specify value type of parameter such as int,long,etc.";
        }

        public override void SetValue(string value)
        {
            IsValidDataType = false;
            if (Enum.TryParse(value, true, out ParameterDataTypeEnum parameterDataType))
            {
                switch (parameterDataType)
                {
                    case ParameterDataTypeEnum.Float64:
                        IsValidDataType = true;
                        Value = ParameterDataTypeEnum.Float64;
                        break;
                    case ParameterDataTypeEnum.Bool:
                        IsValidDataType = true;
                        Value = ParameterDataTypeEnum.Bool;
                        break;
                    case ParameterDataTypeEnum.Int64:
                        IsValidDataType = true;
                        Value = ParameterDataTypeEnum.Float64;
                        break;
                    case ParameterDataTypeEnum.Int32:
                        IsValidDataType = true;
                        Value = ParameterDataTypeEnum.Int32;
                        break;
                    case ParameterDataTypeEnum.String:
                        IsValidDataType = true;
                        Value = ParameterDataTypeEnum.String;
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

    internal enum ParameterDataTypeEnum
    {
        Float64,
        Bool,
        Int64,
        Int32,
        String
    }
}
