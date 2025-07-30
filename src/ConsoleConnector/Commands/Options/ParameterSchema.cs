using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    internal class ParameterSchema : CommandOption
    {
        public ParameterSchema()
        {
            this.Description = "Specify the schema for parameter";
        }

        public override string ToString()
        {
            return "ParameterSchema[" + Description + "]";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
