// Ignore Spelling: App

using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using Autodesk.DataExchange.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Core.Models;


namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    /// <summary>
    /// Create new exchange.
    /// </summary>
    internal class CreateExchangeCommand : Command
    {
        internal string ExchangeId;
        internal ExchangeDetails ExchangeDetails;
        public CreateExchangeCommand(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            this.ExchangeDetails = null;
            this.ExchangeId = string.Empty;
            this.ConsoleAppHelper = consoleAppHelper;
            this.Name = "CreateExchange";
            Description = "Create new exchange.";
            Options = new List<CommandOption>
                {
                    new ExchangeTitle()
                };
        }

        public CreateExchangeCommand(CreateExchangeCommand createExchangeCommand) : base(createExchangeCommand)
        {
        }

        public override Command Clone()
        {
            return new CreateExchangeCommand(this);
        }

        public override async Task<bool> Execute()
        {
            if (this.ValidateOptions() == false)
            {
                Console.WriteLine("[ERROR] Invalid inputs provided");
                return false;
            }

            if (ConsoleAppHelper.TryGetFolderDetails(out _,out _,out _,out _))
            {
                Console.WriteLine("[FOLDER] Please set default folder first");
                Console.WriteLine();
                return false;
            }

            var exchangeTitle = this.GetOption<ExchangeTitle>();
            ExchangeDetails = (await ConsoleAppHelper.CreateExchange(exchangeTitle.Value)).Value;
            var elementDataModel = ElementDataModel.Create(ConsoleAppHelper.GetClient());
            ConsoleAppHelper.AddExchangeData(exchangeTitle.Value, elementDataModel);
            ConsoleAppHelper.AddExchangeDetails(exchangeTitle.Value, ExchangeDetails);
            CommandOutput["ExchangeId"] = ExchangeDetails.ExchangeID;
            CommandOutput["CollectionId"] = ExchangeDetails.CollectionID;
            CommandOutput["HubId"] = ExchangeDetails.HubId;
            
            Console.WriteLine($"[SUCCESS] Exchange '{exchangeTitle.Value}' created successfully!");
            Console.WriteLine($"[INFO] ExchangeId: {ExchangeDetails.ExchangeID}");
            Console.WriteLine($"[INFO] CollectionID: {ExchangeDetails.CollectionID}");
            Console.WriteLine();
            ExchangeId = ExchangeDetails.ExchangeID;
            ConsoleAppHelper.SetExchangeUpdated(exchangeTitle.Value, false);
            return true;
        }
    }
}
