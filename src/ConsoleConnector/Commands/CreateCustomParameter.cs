using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    /// <summary>
    /// Create custom parameters to element. 
    /// </summary>
    internal class CustomParameterCommand : Command
    {
        public CustomParameterCommand(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            this.Name = "AddCustomParam";
            Description = "Add custom parameter to element.";
            Options = new List<CommandOption>
            {
                new ExchangeTitle(),
                new ElementId(),
                new ParameterValue(),
                new ParameterValueDataType()
            };
        }

        public CustomParameterCommand(CustomParameterCommand parameterCommand) : base(parameterCommand)
        {
        }

        public override Command Clone()
        {
            return new CustomParameterCommand(this);
        }

        public override Task<bool> Execute()
        {
            if (this.ValidateOptions() == false)
            {
                Console.WriteLine("Invalid inputs!!!");
                return Task.FromResult(false);
            }
            var exchangeTitle = this.GetOption<ExchangeTitle>();
            var value = this.GetOption<ParameterValue>();
            var elementId = this.GetOption<ElementId>();
            var parameterDataType = this.GetOption<ParameterValueDataType>();

            var exchangeData = ConsoleAppHelper.GetExchangeData(exchangeTitle.Value);
            if (exchangeData == null)
            {
                Console.WriteLine("Exchange data not found.\n");
                return Task.FromResult(false);
            }

            var exchangeDetails = ConsoleAppHelper.GetExchangeDetails(exchangeTitle.Value);
            if (exchangeDetails == null)
            {
                Console.WriteLine("Exchange details not found.\n");
                return Task.FromResult(false);
            }

            var revitExchangeData = ElementDataModel.Create(ConsoleAppHelper.GetClient(), exchangeData);
            var element = revitExchangeData.Elements.ToList().FirstOrDefault(n => n.Id == elementId.Value);
            if (element == null)
            {
                Console.WriteLine("Element not found");
                return Task.FromResult(false);
            }

            //need to check.
            ConsoleAppHelper.GetParameterHelper().AddCustomParameter(exchangeDetails.SchemaNamespace,element, value.Value, parameterDataType.Value);
            ConsoleAppHelper.SetExchangeUpdated(exchangeTitle.Value, true);
            Console.WriteLine("Custom parameter is added.\n");
            return Task.FromResult(true);
        }
    }
}
