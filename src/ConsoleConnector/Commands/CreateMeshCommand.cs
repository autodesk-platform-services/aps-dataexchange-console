using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    /// <summary>
    /// Create Mesh geometry element.
    /// </summary>
    internal class CreateMeshCommand : Command
    {
        public CreateMeshCommand(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            this.Name = "AddMesh";
            Description = "Add mesh geometry to Exchange.";
            Options = new List<CommandOption>
                {
                    new ExchangeTitle()
                };
        }

        public CreateMeshCommand(CreateMeshCommand createMeshCommand) : base(createMeshCommand)
        {
        }

        public override Command Clone()
        {
            return new CreateMeshCommand(this);
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
            var mesh = ConsoleAppHelper.GetGeometryHelper().CreateMesh(revitExchangeData);
            ConsoleAppHelper.AddExchangeData(exchangeTitle.Value, revitExchangeData.ExchangeData);
            ConsoleAppHelper.SetExchangeUpdated(exchangeTitle.Value, true);
            Console.WriteLine("Element Id: " + mesh.Id);
            Console.WriteLine("Element Name: " + mesh.Name);
            Console.WriteLine("Category: " + mesh.Category);
            Console.WriteLine("Family: " + mesh.Family);
            Console.WriteLine("Type: " + mesh.Type);
            Console.WriteLine();
            return Task.FromResult(true);
        }
    }
}
