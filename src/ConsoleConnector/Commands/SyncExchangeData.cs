using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Helper;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using Autodesk.DataExchange.Core.Models;
using Autodesk.DataExchange.Models;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    /// <summary>
    /// Update geometry information with exchange.
    /// </summary>
    internal class SyncExchangeData : Command
    {
        internal bool ExchangeSync;
        public SyncExchangeData(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            this.ExchangeSync = false;
            this.Name = "SyncExchange";
            Description = "Update geometry data with exchange.";
            Options = new List<CommandOption>
                {
                    new ExchangeTitle()
                };
        }

        public SyncExchangeData(SyncExchangeData syncExchangeGeometry) : base(syncExchangeGeometry)
        {
        }

        public override Command Clone()
        {
            return new SyncExchangeData(this);
        }

        public override async Task<bool> Execute()
        {
            var folderDetailsExist = ConsoleAppHelper.TryGetFolderDetails(out var region, out var hubId, out _, out _);
            if (folderDetailsExist)
            {
                Console.WriteLine("Folder details are not found.");
                return false;
            }

            var exchangeTitle = this.GetOption<ExchangeTitle>();

            var exchangeDetails = ConsoleAppHelper.GetExchangeDetails(exchangeTitle.Value);
            if (exchangeDetails == null)
            {
                Console.WriteLine("Exchange data not found\n");
                return false;
            }

            var exchangeUpdated = ConsoleAppHelper.IsExchangeUpdated(exchangeTitle.Value);
            if (exchangeUpdated == false)
            {
                Console.WriteLine("No exchange update found\n");
                return false;
            }

            var exchangeData = ConsoleAppHelper.GetExchangeData(exchangeTitle.Value);
            if (exchangeData == null)
            {
                Console.WriteLine("No changes found.\n");
                return false;
            }

            var exchangeIdentifier = new DataExchangeIdentifier
            {
                CollectionId = exchangeDetails.CollectionID,
                ExchangeId = exchangeDetails.ExchangeID,
                HubId = hubId,
                Region = region
            };

            ExchangeSync = await ConsoleAppHelper.SyncExchange(exchangeIdentifier, exchangeDetails, exchangeData);
            ConsoleAppHelper.SetExchangeUpdated(exchangeTitle.Value,false);
            Console.WriteLine(exchangeDetails.DisplayName+ " ("+ GetFileVersion(exchangeDetails.FileVersionUrn) + ") Exchange updated.");
            return true;
        }

        private string GetFileVersion(string fileVersionUrn)
        {
            string fileVersion = string.Empty;
            string[] splitString = fileVersionUrn.Split('=');
            if (splitString.Length == 2)
                fileVersion = "V" + splitString[1];
            return fileVersion;

        }
    }
}
