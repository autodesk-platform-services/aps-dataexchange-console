using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    /// <summary>
    /// Create primitive geometry element.
    /// </summary>
    internal class CreatePrimitiveGeometryCommand:Command
    {
        public CreatePrimitiveGeometryCommand(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            Name = "AddPrimitive";
            Description = "Add random primitive geometry to element.";
            Options = new List<CommandOption>
            {
                new ExchangeTitle(),
                new Options.PrimitiveGeometry(),
            };
        }

        public CreatePrimitiveGeometryCommand(CreatePrimitiveGeometryCommand setFolderCommand) : base(setFolderCommand)
        {
        }

        public override Command Clone()
        {
            return new CreatePrimitiveGeometryCommand(this);
        }

        public override Task<bool> Execute()
        {
            if (this.ValidateOptions() == false)
            {
                Console.WriteLine("[ERROR] Invalid inputs provided");
                return Task.FromResult(false);
            }

            var exchangeTitle = this.GetOption<ExchangeTitle>();
            var primitiveGeometryOption = this.GetOption<Options.PrimitiveGeometry>();

            var exchangeData = ConsoleAppHelper.GetExchangeData(exchangeTitle.Value);
            if (exchangeData == null)
            {
                Console.WriteLine("[ERROR] Exchange data not found\n");
                return Task.FromResult(false);
            }

            var elementDataModel = exchangeData;

            Element element;
            if (primitiveGeometryOption.Value == PrimitiveGeometryType.Circle)
            {
                element = ConsoleAppHelper.GetGeometryHelper().AddCircle(elementDataModel);
            }
            else if (primitiveGeometryOption.Value == PrimitiveGeometryType.Line)
            {
                element = ConsoleAppHelper.GetGeometryHelper().AddLine(elementDataModel);
            }
            else if (primitiveGeometryOption.Value == PrimitiveGeometryType.Point)
            {
                element = ConsoleAppHelper.GetGeometryHelper().AddPoint(elementDataModel);
            }
            else if (primitiveGeometryOption.Value == PrimitiveGeometryType.Polyline)
            {
                element = ConsoleAppHelper.GetGeometryHelper().AddPolyline(elementDataModel);
            }
            else
            {
                element = ConsoleAppHelper.GetGeometryHelper().AddPrimitive(elementDataModel);
            }

                            Console.WriteLine($"[PRIMITIVE] Element Created:");
                Console.WriteLine($"   ID: {element.Id}");
                Console.WriteLine($"   Name: {element.Name}");
                Console.WriteLine($"   Category: {element.Category}");
                Console.WriteLine($"   Family: {element.Family}");
                Console.WriteLine($"   Type: {element.Type}");
            Console.WriteLine();
            ConsoleAppHelper.SetExchangeUpdated(exchangeTitle.Value, true);
            return Task.FromResult(true);
        }
    }
}
