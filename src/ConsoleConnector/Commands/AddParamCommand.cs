using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Helper;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using Autodesk.DataExchange.DataModels;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    internal abstract class AddParamCommand : Command
    {
        public bool IsInstanceParameter = false;
        public AddParamCommand(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            Options = new List<CommandOption>
            {
                new ExchangeTitle(),
                new ElementId(),
                new ParamName(),
                new ParameterValue(),
            };
        }

        public AddParamCommand(AddParamCommand addParamCommand) : base(addParamCommand)
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
            var parameterName = this.GetOption<ParamName>();
            var parameterValue = this.GetOption<ParameterValue>();
            var exchangeData = ConsoleAppHelper.GetExchangeData(exchangeTitle.Value);
            if (exchangeData == null)
            {
                Console.WriteLine("Exchange data not found.\n");
                return Task.FromResult(false);
            }

            var revitExchangeData = ElementDataModel.Create(ConsoleAppHelper.GetClient(), exchangeData);
            var element = revitExchangeData.Elements.ToList().FirstOrDefault(n => n.Id == elementId.Value);
            if (element == null)
            {
                Console.WriteLine("Element not found");
                return Task.FromResult(false);
            }

            if (parameterName.Value != null)
                ConsoleAppHelper.GetParameterHelper().AddInstanceParameter(element, parameterName.Value.Value, parameterValue.Value, parameterValue.ParameterValueType, IsInstanceParameter);
            else
                ConsoleAppHelper.GetParameterHelper().AddCustomParameter(parameterName.ParameterName, element, parameterValue.Value, parameterValue.ParameterValueType, IsInstanceParameter);

            ConsoleAppHelper.SetExchangeUpdated(exchangeTitle.Value, true);
            Console.WriteLine("Parameter is added.\n");
            return Task.FromResult(true);
            return base.Execute();
        }
    }
}
