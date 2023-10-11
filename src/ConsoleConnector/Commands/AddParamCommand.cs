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
                new ParameterName(),
                new ParameterValue(),
                new ParameterValueDataType()
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
            var parameterName = this.GetOption<ParameterName>();
            var parameterValue = this.GetOption<ParameterValue>();
            var parameterValueType = this.GetOption<ParameterValueDataType>();
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

            DataModels.Parameter parameter = null;
            if (parameterName.Value != null)
                parameter = ConsoleAppHelper.GetParameterHelper().AddBuiltInParameter(element, parameterName.Value.Value, parameterValue.Value, parameterValueType.Value, !IsInstanceParameter);
            else
                parameter = ConsoleAppHelper.GetParameterHelper().AddCustomParameter(parameterName.SchemaName, element, parameterValue.Value, parameterValueType.Value, !IsInstanceParameter);

            ConsoleAppHelper.SetExchangeUpdated(exchangeTitle.Value, true);
            if (parameter == null)
            {
                Console.WriteLine("Parameter is not added.\n");
            }
            else
            {
                var builtInParameter = parameterName.Value == null ? "Custom" : "Built-In";
                Console.WriteLine("Parameter is added.\nThis is "+builtInParameter+" parameter"+"\nParameter value type is "+ parameterValueType.Value);
            }

            return Task.FromResult(true);
            return base.Execute();
        }
    }
}
