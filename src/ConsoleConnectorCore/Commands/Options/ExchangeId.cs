using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    internal class ExchangeId : CommandOption
    {
        public ExchangeId()
        {
            this.Description = "Specify the exchange id.";
        }

        public override string ToString()
        {
            return "ExchangeId[" + Description + "]";
        }
    }
}
