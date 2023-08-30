using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Helper;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    internal class GetExchangeCommand:Command
    {
        public GetExchangeCommand(ConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            Name = "GetExchange";
            Description = "Get exchange and download as a STEP file.";
            Options = new List<CommandOption>
            {
                new ExchangeId(),
                new CollectionId()
            };
        }

        public GetExchangeCommand(GetExchangeCommand getExchangeCommand) : base(getExchangeCommand)
        {

        }

        public override Command Clone()
        {
            return new GetExchangeCommand(this);
        }

        public override async Task<bool> Execute()
        {
            if (ConsoleAppHelper.TryGetFolderDetails(out var region, out var hubId, out var projectUrn, out var folderUrn))
            {
                Console.WriteLine("Folder details not found!!!");
                return false;
            }

            if (this.ValidateOptions() == false)
            {
                Console.WriteLine("Invalid inputs!!!");
                return false;
            }

            var exchangeId = GetOption<ExchangeId>().Value;
            var collectionId = GetOption<CollectionId>().Value;

            Console.WriteLine("Downloading exchange...");
            var status = await ConsoleAppHelper.GetExchange(exchangeId, collectionId, hubId, region);
            if (status == null || string.IsNullOrEmpty(status.Item1))
            {
                Console.WriteLine("Downloading exchange is failed.");
                return false;
            }

            Console.WriteLine("Exchange downloaded.");
            Console.WriteLine("Exchange STEP file: "+status.Item1);
            return true;
        }
    }
}
