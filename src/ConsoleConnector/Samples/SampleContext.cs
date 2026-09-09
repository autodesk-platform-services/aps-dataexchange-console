using System.Collections.Generic;
using Autodesk.DataExchange.Core.Models;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using Autodesk.DataExchange.Models;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    public sealed class SampleContext
    {
        public SampleContext(IClient client, Defaults defaults)
        {
            Client = client;
            Defaults = defaults;
        }

        public IClient Client { get; }
        public Defaults Defaults { get; }
        public FolderInfo? Folder { get; set; }
        public Dictionary<string, ActiveExchange> Exchanges { get; } = new();
        public LoadedExchangeInfo? LastExchange { get; set; }
        public string? LastExchangeTitle { get; set; }

        /// <summary>Set by 2.1 after a successful create; consumed by scenario samples.</summary>
        internal ExchangeDetails? LastCreatedExchange { get; set; }

        /// <summary>When set by 11.x scenarios after create, pickers bind to this exchange without prompting.</summary>
        internal string? ScenarioExchangeTitle { get; set; }
    }

    public sealed record FolderInfo(
        string HubId,
        string ProjectUrn,
        string FolderUrn,
        string Region,
        string? HubName = null,
        string? ProjectName = null,
        string? FolderName = null);

    public sealed record ActiveExchange(
        string ExchangeFileUrn,
        string CollectionId,
        ElementDataModel DataModel,
        int? VersionNumber = null);

    public sealed record LoadedExchangeInfo(
        string Title,
        string FileUrn,
        string ExchangeId,
        string CollectionId,
        string? HubId);

    public sealed class Defaults
    {
        public string? StepPath { get; set; }
        public string? IfcPath { get; set; }
        public string? ObjPath { get; set; }
        public string? BuiltInSchemaId { get; set; }
    }
}
