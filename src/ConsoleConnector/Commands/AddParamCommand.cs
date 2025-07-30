using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Helper;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;

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
                new ParameterSchema(),
                new ParameterValue(),
                new ParameterValueDataType()
            };
        }

        public AddParamCommand(AddParamCommand addParamCommand) : base(addParamCommand)
        {
        }

        public override async Task<bool> Execute()
        {
            if (this.ValidateOptions() == false)
            {
                Console.WriteLine("[ERROR] Invalid inputs provided");
                return false;
            }

            var exchangeTitle = this.GetOption<ExchangeTitle>();
            var elementId = this.GetOption<ElementId>();
            var parameterName = this.GetOption<ParameterName>();
            var parameterSchema = this.GetOption<ParameterSchema>();
            var parameterValue = this.GetOption<ParameterValue>();
            var parameterValueType = this.GetOption<ParameterValueDataType>();
            if (parameterValueType.IsValidDataType == false)
            {
                Console.WriteLine("[ERROR] Invalid data type - use Help command for details");
                return false;
            }
            var exchangeData = ConsoleAppHelper.GetExchangeData(exchangeTitle.Value);
            if (exchangeData == null)
            {
                Console.WriteLine("[ERROR] Exchange data not found\n");
                return false;
            }

            var exchangeDetails = ConsoleAppHelper.GetExchangeDetails(exchangeTitle.Value);
            if (exchangeDetails == null)
            {
                Console.WriteLine("[ERROR] Exchange details not found\n");
                return false;
            }

            var elementDataModel = exchangeData;
            var element = elementDataModel.Elements.ToList().FirstOrDefault(n => n.Id == elementId.Value);            
            if (element == null)
            {
                Console.WriteLine("[ERROR] Element not found");
                return false;
            }

            IParameter parameter = null;
            if (parameterSchema!=null && parameterSchema.IsValid())
            {
                parameter = await ConsoleAppHelper.GetParameterHelper().AddBuiltInParameter(elementDataModel, parameterName.Value, parameterSchema.Value, element,parameterValue.Value, parameterValueType.Value, !IsInstanceParameter);
            }
            else
            {
                parameter = await ConsoleAppHelper.GetParameterHelper().AddCustomParameter(elementDataModel, parameterName.Value, parameterValue.Value, element, parameterValueType.Value, !IsInstanceParameter);
            }

            ConsoleAppHelper.SetExchangeUpdated(exchangeTitle.Value, true);
            if (parameter == null)
            {
                Console.WriteLine("[ERROR] Parameter could not be added\n");
            }
            else
            {
                var builtInParameter = parameterName.Value == null ? "Custom" : "Built-In";
                Console.WriteLine($"[SUCCESS] Parameter added successfully!\n[INFO] Type: {builtInParameter} parameter\n[INFO] Value type: {parameterValueType.Value}");
            }

            return true;
        }

        public override bool ValidateOptions()
        {
            foreach (var item in this.Options)
            {
                if(item is ParameterSchema)
                {
                    continue;
                }
                if (item.IsValid() == false)
                    return false;
            }
            return true;
        }
    }
}
