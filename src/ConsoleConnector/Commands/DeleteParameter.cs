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
    internal abstract class DeleteParameter:Command
    {
        public bool IsInstanceParameter = false;
        public DeleteParameter(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            Options = new List<CommandOption>
            {
                new ExchangeTitle(),
                new ElementId(),
                new ParameterName()
            };
        }

        public DeleteParameter(DeleteParameter removeParameter) : base(removeParameter)
        {

        }

        public override Task<bool> Execute()
        {
            if (this.ValidateOptions() == false)
            {
                Console.WriteLine("Invalid inputs!!!");
                return Task.FromResult(false);
            }

            var exchangeTitle = this.GetOption<ExchangeTitle>();
            var elementId = this.GetOption<ElementId>();
            var parameterName = this.GetOption<ParameterName>();

            var exchangeData = ConsoleAppHelper.GetExchangeData(exchangeTitle.Value);
            if (exchangeData == null)
            {
                Console.WriteLine("Exchange data not found.\n");
                return Task.FromResult(false);
            }

            var elementDataModel = ElementDataModel.Create(ConsoleAppHelper.GetClient(), exchangeData);
            var element = elementDataModel.Elements.ToList().FirstOrDefault(n => n.Id == elementId.Value);
            if (element == null)
            {
                Console.WriteLine("Element not found");
                return Task.FromResult(false);
            }

            var parameterIsDeleted = false;
            if (parameterName.Value != null && IsInstanceParameter)
            {
                parameterIsDeleted = true;
                element.DeleteParameter(parameterName.Value.Value);
            }
            else if (parameterName.Value != null && IsInstanceParameter==false)
            {
                parameterIsDeleted = element.DeleteTypeParameter(parameterName.Value.Value);
            }
            else if (parameterName.Value == null && IsInstanceParameter)
            {
                parameterIsDeleted = true;
                element.DeleteParameter(parameterName.SchemaName);
            }
            else if (parameterName.Value == null && IsInstanceParameter == false)
            {
                parameterIsDeleted = element.DeleteTypeParameter(parameterName.SchemaName);
            }

            if (parameterIsDeleted)
            {
                Console.WriteLine("Parameter is deleted.");
                return Task.FromResult(true);
            }

            Console.WriteLine("Parameter is not found.Please check schema.");
            return Task.FromResult(false);
        }
    }
}
