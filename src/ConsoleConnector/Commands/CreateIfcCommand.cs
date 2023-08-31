using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    /// <summary>
    /// Create IFC geometry element. 
    /// </summary>
    internal class CreateIfcCommand : Command
    {
        public CreateIfcCommand(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            this.ConsoleAppHelper = consoleAppHelper;
            this.Name = "AddIFC";
            Description = "Add ifc geometry to Exchange.";
            Options = new List<CommandOption>
                {
                    new ExchangeTitle()
                };
        }

        public CreateIfcCommand(CreateIfcCommand createIfcCommand) : base(createIfcCommand)
        {
        }

        public override Command Clone()
        {
            return new CreateIfcCommand(this);
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

            var revitExchangeData = ElementDataModel.Create(ConsoleAppHelper.GetClient(), exchangeData);
            var ifc = ConsoleAppHelper.GetGeometryHelper().CreateIfc(revitExchangeData);
            ConsoleAppHelper.AddExchangeData(exchangeTitle.Value,revitExchangeData.ExchangeData);
            ConsoleAppHelper.SetExchangeUpdated(exchangeTitle.Value, true);
            Console.WriteLine("Element Id: " + ifc.Id);
            Console.WriteLine("Element Name: " + ifc.Name);
            Console.WriteLine("Category: " + ifc.Category);
            Console.WriteLine("Family: " + ifc.Family);
            Console.WriteLine("Type: " + ifc.Type);
            Console.WriteLine();
            return Task.FromResult(true);
        }
    }
}
