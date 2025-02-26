using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using Autodesk.DataExchange.DataModels;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    internal class DeleteTypeParameter : DeleteParameter
    {
        public DeleteTypeParameter(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            this.IsInstanceParameter = false;
            this.Name = "DeleteTypeParam";
            Description = "Delete type parameters from element.";
            Options = new List<CommandOption>
            {
                new ExchangeTitle(),
                new ElementId(),
                new ParameterName()
            };
        }

        public DeleteTypeParameter(DeleteParameter removeParameter) : base(removeParameter)
        {
            this.IsInstanceParameter = removeParameter.IsInstanceParameter;

        }

        public override Command Clone()
        {
            return new DeleteInstanceParameter(this);
        }
    }
}
