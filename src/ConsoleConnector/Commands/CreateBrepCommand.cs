using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    /// <summary>
    /// Create brep geometry element. 
    /// </summary>
    internal class CreateBrepCommand : Command
    {
        public CreateBrepCommand(IConsoleAppHelper consoleAppHelper):base(consoleAppHelper)
        {
            Name = "AddBrep";
            Description = "Add brep geometry to Exchange.";
            Options = new List<CommandOption>
                {
                    new ExchangeTitle()
                };
        }

        public CreateBrepCommand(CreateBrepCommand createBrepCommand) : base(createBrepCommand)
        {

        }

        public override Command Clone()
        {
            return new CreateBrepCommand(this);
        }

        public override Task<bool> Execute()
        {
            if (this.ValidateOptions() == false)
            {
                Console.WriteLine("Invalid inputs!!!");
                return Task.FromResult(false);
            }

            var exchangeTitle = this.GetOption<ExchangeTitle>();
            var exchangeData = ConsoleAppHelper.GetExchangeData(exchangeTitle.Value);
            if (exchangeData == null)
            {
                Console.WriteLine("Exchange data not found.\n");
                return Task.FromResult(false);
            }

            var elementDataModel = ElementDataModel.Create(ConsoleAppHelper.GetClient(), exchangeData);
            var brep = ConsoleAppHelper.GetGeometryHelper().CreateBrep(elementDataModel);
            ConsoleAppHelper.AddExchangeData(exchangeTitle.Value, elementDataModel.ExchangeData);
            Console.WriteLine("Element Id: " + brep.Id);
            Console.WriteLine("Element Name: " + brep.Name);
            Console.WriteLine("Category: " + brep.Category);
            Console.WriteLine("Family: " + brep.Family);
            Console.WriteLine("Type: " + brep.Type);
            Console.WriteLine();
            ConsoleAppHelper.SetExchangeUpdated(exchangeTitle.Value, true);
            return Task.FromResult(true);
        }
    }
}
