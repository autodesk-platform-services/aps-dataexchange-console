using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autodesk.DataExchange.ConsoleApp.Commands;
using Autodesk.DataExchange.ConsoleApp.Helper;
using Autodesk.DataExchange.Core.Models;
using Autodesk.DataExchange.Models;

namespace Autodesk.DataExchange.ConsoleApp.Interfaces
{
    internal interface IConsoleAppHelper
    {
        List<Command> Commands { get; set; }
        void Start();
        void AddExchangeData(string exchangeTitle,ExchangeData exchangeData);
        ExchangeData GetExchangeData(string exchangeTitle);
        void AddExchangeDetails(string exchangeTitle,ExchangeDetails exchangeDetails);
        ExchangeDetails GetExchangeDetails(string exchangeTitle);
        void SetFolder(string region,string hubId,string projectUrn,string folderUrn);
        bool TryGetFolderDetails(out string region, out string hubId, out string projectUrn, out string folderUrn);
        void SetExchangeUpdated(string exchangeTitle,bool status);
        bool IsExchangeUpdated(string exchangeTitle);
        void BuildCommands();
        Command GetCommand(string input);
        Task<ExchangeDetails> CreateExchange(string exchangeTitle);

        Task<bool> SyncExchange(DataExchangeIdentifier dataExchangeIdentifier,ExchangeDetails exchangeDetails, ExchangeData exchangeData);

        Client GetClient();
        GeometryHelper GetGeometryHelper();
        ParameterHelper GetParameterHelper();

        Task<Tuple<string, bool>> GetExchange(string exchangeId, string collectionId, string hubId, string hubRegion);

    }
}