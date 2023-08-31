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
    /// <summary>
    /// Create instance parameters to element.
    /// </summary>
    internal class CreateInstanceParameterCommand:Command
    {
        public CreateInstanceParameterCommand(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            this.Name = "AddParam";
            Description = "Add instance parameters to element.";
            Options = new List<CommandOption>
            {
                new ExchangeTitle(),
                new ElementId(),
                new InstanceParameterType(),
                new ParameterValue(),
                new ParameterValueDataType()
            };
        }

        public CreateInstanceParameterCommand(CreateInstanceParameterCommand createInstanceParameterCommand) : base(createInstanceParameterCommand)
        {
        }

        public override Command Clone()
        {
            return new CreateInstanceParameterCommand(this);
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
            var parameterType = this.GetOption<InstanceParameterType>();
            var elementId = this.GetOption<ElementId>();
            var parameterDataType = this.GetOption<ParameterValueDataType>();
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

            ConsoleAppHelper.GetParameterHelper().AddInstanceParameter(element, parameterType.Value, value.Value, parameterDataType.Value);
            ConsoleAppHelper.SetExchangeUpdated(exchangeTitle.Value, true);
            Console.WriteLine("Parameter is added.\n");
            return Task.FromResult(true);
        }
    }
}
