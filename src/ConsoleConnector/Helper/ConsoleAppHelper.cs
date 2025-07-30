using Autodesk.DataExchange.ConsoleApp.Commands;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using Autodesk.DataExchange.Core.Interface;
using Autodesk.DataExchange.Core.Models;
using Autodesk.DataExchange.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Core.Enums;
using Autodesk.DataExchange.ConsoleApp.Exceptions;
using Autodesk.DataExchange.Interface;

namespace Autodesk.DataExchange.ConsoleApp.Helper
{
    internal class ConsoleAppHelper : IConsoleAppHelper
    {
        public List<Command> Commands { get; set; } = new List<Command>();
        private readonly Dictionary<string, bool> _exchangeUpdateStatus = new Dictionary<string, bool>();
        private readonly Dictionary<string, ElementDataModel> _exchangeData = new Dictionary<string, ElementDataModel>();
        private readonly Dictionary<string, string> _exchangeVersions = new Dictionary<string, string>();

        private readonly Dictionary<string, ExchangeDetails> _exchangeDetails =
            new Dictionary<string, ExchangeDetails>();

        private string _hubId;
        private string _region;
        private string _projectUrn;
        private string _folderUrn;
        public Client Client;
        private IStorage Storage => Client.SDKOptions.Storage;

        public ILogger Logger { get => Client.SDKOptions.Logger; }

        public GeometryHelper GeometryHelper = new GeometryHelper();
        public ParameterHelper ParameterHelper = new ParameterHelper();

        public void Start()
        {
            BuildCommands();
            CreateClient();
            ReadFolderDetails();
        }

        /// <summary>
        /// Create an instance of Client.
        /// </summary>
        private void CreateClient()
        {
            var authClientId = ConfigurationManager.AppSettings["AuthClientID"];
            var authCallBack = ConfigurationManager.AppSettings["AuthCallBack"];
            var authClientSecret = ConfigurationManager.AppSettings["AuthClientSecret"];
            if (string.IsNullOrEmpty(authClientId) || string.IsNullOrEmpty(authCallBack) || string.IsNullOrEmpty(authClientSecret))
            {             
                var message = "Authentication details are missing.";
                message += "\nPlease add AuthClientID, AuthCallBack, AuthClientSecret in App.config file.";
                throw new AuthenticationMissingException(message);
            }
            var sdkOptions = new SDKOptionsDefaultSetup()
            {
                HostApplicationName = "ConsoleConnector",
                ClientId = authClientId,
                CallBack = authCallBack,
                ClientSecret = authClientSecret,
                ConnectorName = "ConsoleConnector",
                ConnectorVersion = "1.0.0",
                HostApplicationVersion = "1.0",
            };

            Client = new Client(sdkOptions);

            sdkOptions.Logger.SetDebugLogLevel();
            Client.EnableHttpDebugLogging();
        }

        public void AddExchangeData(string exchangeTitle, ElementDataModel exchangeData)
        {
            _exchangeData[exchangeTitle] = exchangeData;
        }

        public async Task<string> GetRegionAsync(string hubId)
        {
            return await Client.SDKOptions.HostingProvider.GetRegionAsync(hubId);
        }
        public async Task<IResponse<string>> GetHubIdAsync(string projectUrn)
        {
            return await Client.GetHubIdAsync(projectUrn);
        }

        public void GetHubId(string projectUrn, out string hubId)
        {
            hubId = GetHubIdAsync(projectUrn).Result.Value;
        }

        public void GetRegion(string hubId, out string region)
        {
            region = GetRegionAsync(hubId).Result;
        }

        public ElementDataModel GetExchangeData(string exchangeTitle)
        {
            if (_exchangeData.TryGetValue(exchangeTitle, out _) == false)
            {
                TryGetFolderDetails(out var region, out var hubId, out _, out _);
                var exchangeDetails = this.GetExchangeDetails(exchangeTitle);
                if (exchangeDetails == null)
                    return null;
                var currentExchangeData = Client.GetElementDataModelAsync(new DataExchangeIdentifier
                {
                    ExchangeId = exchangeDetails.ExchangeID,
                    CollectionId = exchangeDetails.CollectionID,
                    HubId = hubId
                });
                currentExchangeData.Wait();
                _exchangeData[exchangeTitle] = currentExchangeData.Result.Value;
            }

            return _exchangeData[exchangeTitle];
        }

        public void RemoveExchangeData(string exchangeTitle)
        {
            if (_exchangeData.TryGetValue(exchangeTitle, out _))
            {
                _exchangeData.Remove(exchangeTitle);
            }
        }

        public void AddExchangeDetails(string exchangeTitle, ExchangeDetails exchangeDetails)
        {
            _exchangeDetails[exchangeTitle] = exchangeDetails;
            Storage.Add(exchangeTitle + "ExchangeDetails", exchangeDetails);
            try
            {
                // DEXC-1681, once the jira is fixed, we can remove the try catch.
                Storage.Save();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }

        }

        public ExchangeDetails GetExchangeDetails(string exchangeTitle)
        {
            if (_exchangeDetails.TryGetValue(exchangeTitle, out var exchangeDetails) == false)
            {
                exchangeDetails = Storage.Get<ExchangeDetails>(exchangeTitle + "ExchangeDetails");
                if (exchangeDetails == null)
                    return null;
                _exchangeDetails[exchangeTitle] = exchangeDetails;
            }

            return exchangeDetails;
        }

        public ExchangeDetails GetUpdatedExchangeDetails(DataExchangeIdentifier dataExchangeIdentifier)
        {
            var updatedExchangeData = Client.GetExchangeDetailsAsync(dataExchangeIdentifier);
            updatedExchangeData.Wait();
            return updatedExchangeData.Result.Value;
        }

        public void SetFolder(string region, string hubId, string projectUrn, string folderUrn)
        {
            this._region = region;
            this._hubId = hubId;
            this._projectUrn = projectUrn;
            this._folderUrn = folderUrn;

            this.SaveFolderDetails();
        }

        public bool TryGetFolderDetails(out string region, out string hubId, out string projectUrn,
            out string folderUrn)
        {
            region = this._region;
            hubId = this._hubId;
            projectUrn = this._projectUrn;
            folderUrn = this._folderUrn;

            return string.IsNullOrEmpty(_region) ||
                   string.IsNullOrEmpty(_hubId) ||
                   string.IsNullOrEmpty(_projectUrn) ||
                   string.IsNullOrEmpty(_folderUrn);
        }

        private void ReadFolderDetails()
        {
            // read default folder
            _hubId = Client.SDKOptions.Storage.Get<string>("DefaultHubId");
            _region = Client.SDKOptions.Storage.Get<string>("DefaultRegion");
            _projectUrn = Client.SDKOptions.Storage.Get<string>("DefaultProjectURN");
            _folderUrn = Client.SDKOptions.Storage.Get<string>("DefaultFolderURN");
        }

        private void SaveFolderDetails()
        {
            Client.SDKOptions.Storage.Add("DefaultHubId", _hubId);
            Client.SDKOptions.Storage.Add("DefaultRegion", _region);
            Client.SDKOptions.Storage.Add("DefaultProjectURN", _projectUrn);
            Client.SDKOptions.Storage.Add("DefaultFolderURN", _folderUrn);
            Client.SDKOptions.Storage.Save();
        }

        public void SetExchangeUpdated(string exchangeTitle, bool status)
        {
            _exchangeUpdateStatus[exchangeTitle] = status;
        }

        public bool IsExchangeUpdated(string exchangeTitle)
        {
            _exchangeUpdateStatus.TryGetValue(exchangeTitle, out var isUpdated);
            return isUpdated;
        }

        public void BuildCommands()
        {
            Commands.Add(new GetExchangeCommand(this));
            Commands.Add(new SetFolderCommand(this));
            Commands.Add(new CreateExchangeCommand(this));
            Commands.Add(new CreateBrepCommand(this));
            Commands.Add(new CreateMeshCommand(this));
            Commands.Add(new CreateIfcCommand(this));
            Commands.Add(new AddInstanceParamCommand(this));
            Commands.Add(new AddTypeParamCommand(this));
            Commands.Add(new DeleteInstanceParameter(this));
            Commands.Add(new DeleteTypeParameter(this));
            Commands.Add(new CreatePrimitiveGeometryCommand(this));
            Commands.Add(new SyncExchangeData(this));
            Commands.Add(new HelpCommand(this));
            Commands.Add(new ExitCommand());

#if DEBUG
            Commands.Add(new WorkFlowTestCommand(this));
#endif


        }


        public Command GetCommand(string input)
        {
            var data = CustomSplit(input);
            var commandName = data.FirstOrDefault();
            var command =
                Commands.FirstOrDefault(n => commandName != null && n.Name.ToLower() == commandName.ToLower());
            if (command == null)
                return null;
            var cloneCommand = command.Clone();
            for (int i = 0; i < cloneCommand.Options.Count; i++)
            {
                var argumentInput = data.ElementAtOrDefault(i + 1);
                argumentInput = argumentInput?.Trim('"');
                cloneCommand.Options[i].SetValue(argumentInput);
            }

            return cloneCommand;
        }

        public async Task<IResponse<ExchangeDetails>> CreateExchange(string exchangeTitle)
        {
            var name = exchangeTitle;
            TryGetFolderDetails(out var region, out var hubId, out var projectUrn, out var folderUrn);
            var projectDetails = await Client.SDKOptions.HostingProvider.GetProjectInformationAsync(hubId, projectUrn);
            var projectType = ProjectType.ACC;
            if (projectDetails != null)
            {
                projectType = projectDetails.ProjectType;
            }

            var exchangeCreateRequest = new ExchangeCreateRequestACC()
            {
                Host = Client.SDKOptions.HostingProvider,
                Contract = new Autodesk.DataExchange.ContractProvider.ContractProvider(),
                Description = string.Empty,
                FileName = name,
                ACCFolderURN = folderUrn,
                ProjectId = projectUrn,
                Region = region,
                HubId = hubId,
                ProjectType = projectType
            };
            return await Client.CreateExchangeAsync(exchangeCreateRequest);
        }

        public async Task<bool> SyncExchange(DataExchangeIdentifier dataExchangeIdentifier, ExchangeDetails exchangeDetails, ElementDataModel exchangeData)
        {
            await Client.SyncExchangeDataAsync(dataExchangeIdentifier, exchangeData);
            await Client.GenerateViewableAsync(exchangeDetails.ExchangeID,
                exchangeDetails.CollectionID);
            return true;
        }

        public IClient GetClient()
        {
            return this.Client;
        }

        public GeometryHelper GetGeometryHelper()
        {
            return this.GeometryHelper;
        }

        public ParameterHelper GetParameterHelper()
        {
            return this.ParameterHelper;
        }

        static List<string> CustomSplit(string input)
        {
            List<string> parts = new List<string>();
            bool insideQuotes = false;
            int startIndex = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '"')
                {
                    insideQuotes = !insideQuotes;
                }
                else if (input[i] == ' ' && !insideQuotes)
                {
                    string part = input.Substring(startIndex, i - startIndex);
                    parts.Add(part);
                    startIndex = i + 1;
                }
            }

            // Add the last part
            string lastPart = input.Substring(startIndex);
            parts.Add(lastPart);
            parts = parts.Where(n => string.IsNullOrEmpty(n) == false).ToList();
            return parts;
        }

        private string GetExchangeTitle(string exchangeId)
        {
            var exchangeDetails = _exchangeDetails.FirstOrDefault(n => n.Value.ExchangeID == exchangeId).Value;
            if (exchangeDetails == null)
            {
                return string.Empty;
            }

            return exchangeDetails.DisplayName;
        }

        public async Task<Tuple<string, bool>> GetExchange(string exchangeId, string collectionId, string hubId, string hubRegion, string fileFormat)
        {
            var exchangeFile = string.Empty;
            var isUpdated = false;

            try
            {
                var exchangeTitle = GetExchangeTitle(exchangeId);
                var exchangeIdentifier = new DataExchangeIdentifier
                {
                    CollectionId = collectionId,
                    ExchangeId = exchangeId,
                    HubId = hubId
                };

                //Get a list of all revisions
                var revisions = (await Client.GetExchangeRevisionsAsync(exchangeIdentifier)).Value;

                //Get the latest revision

                var firstRev = revisions.First().Id;

                _exchangeVersions.TryGetValue(exchangeId, out var currentRevision);
                if (!string.IsNullOrEmpty(currentRevision) && currentRevision == firstRev && string.IsNullOrEmpty(exchangeTitle) == false)
                {
                    Console.WriteLine("[INFO] No changes found on exchange");
                    return null;
                }

                _exchangeData.TryGetValue(exchangeTitle, out var currentExchangeData);

                // Get Exchange data
                if (currentExchangeData == null /*|| currentExchangeData?.ExchangeID != exchangeIdentifier.ExchangeId*/)
                {
                    // Get full Exchange Data till the latest revision
                    currentExchangeData = (await Client.GetElementDataModelAsync(exchangeIdentifier)).Value;
                    currentRevision = firstRev;

                    var data = currentExchangeData;// ElementDataModel.Create(Client, currentExchangeData);

                    //data.Elements.a

                    // Get all Wall Elements
                    //var wallElements = data.Elements.Where(element => element.Category == "Walls").ToList();

                    // Get all added Elements
                    //var addedElements = data.GetCreatedElements(new List<string> { currentRevision });

                    // Get all modified Elements
                    //var modifiedElements = data.GetModifiedElements(new List<string> { currentRevision });


                    // Get all deleted Elements
                    //var deletedElements = data.DeletedElements.ToList();


                    await data.GetElementGeometriesAsync(data.Elements).ConfigureAwait(false);

                    //Get Geometry of whole exchange file
                    if (fileFormat == "OBJ")
                    {
                        exchangeFile = Client.DownloadCompleteExchangeAsOBJ(exchangeId, collectionId).Value;
                    }
                    else
                    {
                        exchangeFile = Client.DownloadCompleteExchangeAsSTEP(new DataExchangeIdentifier
                        {
                            ExchangeId = exchangeId,
                            CollectionId = collectionId,
                            HubId = hubId
                        }).Value;
                    }

                    isUpdated = false;
                }
                else
                {
                    // Update Exchange data with Delta
                    var newRevision = (await Client.RetrieveLatestExchangeDataAsync(currentExchangeData)).Value;
                    var newerRevisions = new List<string>();
                    if (!string.IsNullOrEmpty(newRevision))
                    {
                        foreach (var revision in revisions)
                        {
                            if (revision.Id == currentRevision)
                            {
                                break;
                            }

                            newerRevisions.Add(revision.Id);
                        }
                        currentRevision = newRevision;

                    }

                    var data = currentExchangeData;// ElementDataModel.Create(Client, currentExchangeData);

                    // Get all Wall Elements
                    //var wallElements = data.Elements.Where(element => element.Category == "Walls").ToList();

                    // Get all added Elements
                    //var addedElements = data.GetCreatedElements(newerRevisions);

                    // Get all modified Elements
                    //var modifiedElements = data.GetModifiedElements(newerRevisions);

                    // Get all deleted Elements
                    //var deletedElements = data.GetDeletedElements(newerRevisions);

                    await data.GetElementGeometriesAsync(data.Elements).ConfigureAwait(false);


                    //Get Geometry of whole exchange file
                    if (fileFormat == "OBJ")
                    {
                        exchangeFile = Client.DownloadCompleteExchangeAsOBJ(exchangeId, collectionId).Value;
                    }
                    else
                    {
                        exchangeFile = Client.DownloadCompleteExchangeAsSTEP(new DataExchangeIdentifier
                        {
                            ExchangeId = exchangeId,
                            CollectionId = collectionId,
                            HubId = hubId
                        }).Value;
                    }
                    isUpdated = true;
                }

                _exchangeData[exchangeTitle] = currentExchangeData;
                _exchangeVersions[exchangeId] = currentRevision;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] {e}");
                Logger?.Error(e);
            }

            return new Tuple<string, bool>(exchangeFile, isUpdated);
        }
    }
}
