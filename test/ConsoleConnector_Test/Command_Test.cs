using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DataExchange;
using Autodesk.DataExchange.ConsoleApp.Commands;
using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Helper;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using Autodesk.DataExchange.Core.Models;
using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using Autodesk.DataExchange.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace ConsoleConnector_Test
{
    [TestClass]
    public class Command_Test
    {
        private Mock<IConsoleAppHelper> consoleAppHelper;
        private Mock<IClient> client;
        [TestInitialize]
        public void TestInit()
        {
            consoleAppHelper = new Mock<IConsoleAppHelper>();
            client = new Mock<IClient>();

            var defaultFolder = "test";
           
            // Fix the mock setup for TryGetFolderDetails
            consoleAppHelper.Setup(n =>
                n.TryGetFolderDetails(out It.Ref<string>.IsAny, out It.Ref<string>.IsAny, out It.Ref<string>.IsAny, out It.Ref<string>.IsAny))
                .Returns(false); // Return false to indicate folder details were found

            consoleAppHelper.Setup(n => n.GetExchangeDetails("TestExchange")).Returns(() =>
            {
                return JsonConvert.DeserializeObject<ExchangeDetails>(
                    "{\"ProjectUrn\":\"b.e3be8c87-1df5-470f-9214-1b6cc85452fa\",\"FileUrn\":\"urn:adsk.wipprod:dm.lineage:IpFw2xoRRTS__n5-kV5XXA\",\"FileVersionUrn\":\"urn:adsk.wipprod:fs.file:vf.IpFw2xoRRTS__n5-kV5XXA?version=1\",\"FolderUrn\":\"urn:adsk.wipprod:fs.folder:co.NBWiKlvJSqOo1B4iUajHeA\",\"ExchangeID\":\"7a3102e6-645c-3b88-8e08-b5f3f0e243be\",\"CollectionID\":\"co.cBMZ-5QhTym2c-nfa1Fx2Q\",\"DisplayName\":\"TestExchange\",\"CreatedTime\":\"2023-08-21T15:41:30.0481133+05:30\",\"LastModifiedTime\":\"2023-08-21T15:41:30.0481133+05:30\",\"CreatedBy\":\"DhirajLotake\",\"LastModifiedBy\":\"DhirajLotake\",\"Attributes\":{},\"FolderPath\":\"\",\"SchemaNamespace\":\"c73cae7ea1540e39f45528aa243d4d26\",\"HubId\":null,\"HubRegion\":null}");
            });

            consoleAppHelper.Setup(n => n.GetUpdatedExchangeDetails(It.IsAny<DataExchangeIdentifier>())).Returns(() =>
                JsonConvert.DeserializeObject<ExchangeDetails>(
                    "{\"ProjectUrn\":\"b.e3be8c87-1df5-470f-9214-1b6cc85452fa\",\"FileUrn\":\"urn:adsk.wipprod:dm.lineage:IpFw2xoRRTS__n5-kV5XXA\",\"FileVersionUrn\":\"urn:adsk.wipprod:fs.file:vf.IpFw2xoRRTS__n5-kV5XXA?version=2\",\"FolderUrn\":\"urn:adsk.wipprod:fs.folder:co.NBWiKlvJSqOo1B4iUajHeA\",\"ExchangeID\":\"7a3102e6-645c-3b88-8e08-b5f3f0e243be\",\"CollectionID\":\"co.cBMZ-5QhTym2c-nfa1Fx2Q\",\"DisplayName\":\"TestExchange\",\"CreatedTime\":\"2023-08-21T15:41:30.0481133+05:30\",\"LastModifiedTime\":\"2023-08-21T15:41:30.0481133+05:30\",\"CreatedBy\":\"DhirajLotake\",\"LastModifiedBy\":\"DhirajLotake\",\"Attributes\":{},\"FolderPath\":\"\",\"SchemaNamespace\":\"c73cae7ea1540e39f45528aa243d4d26\",\"HubId\":null,\"HubRegion\":null}")
            );

            consoleAppHelper.Setup(n => n.CreateExchange("TestExchange")).ReturnsAsync(() =>
            {
                var exchangeDetails = JsonConvert.DeserializeObject<ExchangeDetails>(
                    "{\"ProjectUrn\":\"b.e3be8c87-1df5-470f-9214-1b6cc85452fa\",\"FileUrn\":\"urn:adsk.wipprod:dm.lineage:IpFw2xoRRTS__n5-kV5XXA\",\"FileVersionUrn\":\"urn:adsk.wipprod:fs.file:vf.IpFw2xoRRTS__n5-kV5XXA?version=1\",\"FolderUrn\":\"urn:adsk.wipprod:fs.folder:co.NBWiKlvJSqOo1B4iUajHeA\",\"ExchangeID\":\"7a3102e6-645c-3b88-8e08-b5f3f0e243be\",\"CollectionID\":\"co.cBMZ-5QhTym2c-nfa1Fx2Q\",\"DisplayName\":\"TestExchange\",\"CreatedTime\":\"2023-08-21T15:41:30.0481133+05:30\",\"LastModifiedTime\":\"2023-08-21T15:41:30.0481133+05:30\",\"CreatedBy\":\"DhirajLotake\",\"LastModifiedBy\":\"DhirajLotake\",\"Attributes\":{},\"FolderPath\":\"\",\"SchemaNamespace\":\"c73cae7ea1540e39f45528aa243d4d26\",\"HubId\":null,\"HubRegion\":null}");
                
                // Create a simple mock response
                var response = new Mock<IResponse<ExchangeDetails>>();
                response.SetupGet(x => x.Value).Returns(exchangeDetails);
                response.SetupGet(x => x.IsSuccess).Returns(true);
                
                return response.Object;
            });

            consoleAppHelper.Setup(n => n.IsExchangeUpdated(It.IsAny<string>())).Returns(() =>
            {
                return true;
            });

            consoleAppHelper.Setup(n => n.SyncExchange(It.IsAny<DataExchangeIdentifier>(), It.IsAny<ExchangeDetails>(), It.IsAny<ElementDataModel>())).ReturnsAsync(() =>
            {
                return true;
            });

            consoleAppHelper.Setup(n => n.GetExchangeData(It.IsAny<string>())).Returns(() =>
            {
                return ElementDataModel.Create(client.Object);
            });

            consoleAppHelper.Setup(n=>n.GetClient()).Returns(() =>
            {
                return client.Object;
            });
        }

        [TestMethod]
        public void CreateExchange_Test()
        {
            var createExchange = new CreateExchangeCommand(consoleAppHelper.Object);
            createExchange.GetOption<ExchangeTitle>().SetValue("TestExchange");
            var task = createExchange.Execute();
            task.Wait();
            
            Assert.IsNotNull(createExchange.ExchangeId);
            Assert.IsNotNull(createExchange.ExchangeDetails);
            Assert.AreEqual("TestExchange", createExchange.ExchangeDetails.DisplayName);
        }

        [TestMethod]
        public void SyncExchange_Test()
        {
            var syncExchangeData = new SyncExchangeData(consoleAppHelper.Object);
            syncExchangeData.GetOption<ExchangeTitle>().SetValue("TestExchange");
            var task = syncExchangeData.Execute();
            task.Wait();
            Assert.AreEqual(task.Result, true);
        }

        [TestMethod]
        public void SetFolder_IdsPath_ValidationFails_ReturnsFalse()
        {
            consoleAppHelper.Setup(n =>
                n.ValidateHubAccessAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((false, "[ERROR] Unable to resolve HubId for ProjectUrn 'b.wrong-id'. " +
                    "This usually means your app's ClientId has not been added to the Forma/ACC hub " +
                    "as a custom integration (via Hub Admin > Custom Integrations), or the ProjectUrn does not exist."));

            var setFolder = new SetFolderCommand(consoleAppHelper.Object);
            setFolder.GetOption<HubId>().SetValue("b.wrong-hub-id");
            setFolder.GetOption<Region>().SetValue("US");
            setFolder.GetOption<ProjectUrn>().SetValue("b.wrong-id");
            setFolder.GetOption<FolderUrn>().SetValue("urn:adsk.wipprod:fs.folder:co.test");

            var task = setFolder.Execute();
            task.Wait();
            Assert.AreEqual(false, task.Result);

            consoleAppHelper.Verify(
                n => n.SetFolder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never());
        }

        [TestMethod]
        public void SetFolder_IdsPath_ValidationPasses_ReturnsTrue()
        {
            consoleAppHelper.Setup(n =>
                n.ValidateHubAccessAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((true, (string)null));

            var region = "US";
            consoleAppHelper.Setup(n => n.GetRegion(It.IsAny<string>(), out region));

            var setFolder = new SetFolderCommand(consoleAppHelper.Object);
            setFolder.GetOption<HubId>().SetValue("b.valid-hub-id");
            setFolder.GetOption<Region>().SetValue("US");
            setFolder.GetOption<ProjectUrn>().SetValue("b.valid-project");
            setFolder.GetOption<FolderUrn>().SetValue("urn:adsk.wipprod:fs.folder:co.test");

            var task = setFolder.Execute();
            task.Wait();
            Assert.AreEqual(true, task.Result);

            consoleAppHelper.Verify(
                n => n.SetFolder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once());
        }

        [TestMethod]
        public void SetFolder_IdsPath_MissingFields_ReturnsFalse()
        {
            var setFolder = new SetFolderCommand(consoleAppHelper.Object);
            setFolder.GetOption<HubId>().SetValue("b.hub-id");
            // Region is not set, ProjectUrn is not set, FolderUrn is not set

            var task = setFolder.Execute();
            task.Wait();
            Assert.AreEqual(false, task.Result);

            consoleAppHelper.Verify(
                n => n.SetFolder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never());
        }

        [TestMethod]
        public void SetFolder_UrlPath_MalformedUrl_ReturnsFalse()
        {
            var setFolder = new SetFolderCommand(consoleAppHelper.Object);
            setFolder.GetOption<HubId>().SetValue("https://not-a-valid-url");

            var task = setFolder.Execute();
            task.Wait();
            Assert.AreEqual(false, task.Result);

            consoleAppHelper.Verify(
                n => n.SetFolder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never());
        }

        [TestMethod]
        public void SetFolder_UrlPath_HubIdResolutionFails_ReturnsFalse()
        {
            var emptyHubId = (string)null;
            consoleAppHelper.Setup(n => n.GetHubId(It.IsAny<string>(), out emptyHubId));

            var setFolder = new SetFolderCommand(consoleAppHelper.Object);
            setFolder.GetOption<HubId>().SetValue(
                "https://acc.autodesk.com/docs/files/projects/e3be8c87-1df5-470f-9214-1b6cc85452fa?folderUrn=urn%3Aadsk.wipprod%3Afs.folder%3Aco.NBWiKlvJSqOo1B4iUajHeA&viewModel=detail");

            var task = setFolder.Execute();
            task.Wait();
            Assert.AreEqual(false, task.Result);

            consoleAppHelper.Verify(
                n => n.SetFolder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never());
        }

        [TestMethod]
        public void SetFolder_UrlPath_RegionResolutionFails_ReturnsFalse()
        {
            var hubId = "b.valid-hub";
            consoleAppHelper.Setup(n => n.GetHubId(It.IsAny<string>(), out hubId));

            var emptyRegion = (string)null;
            consoleAppHelper.Setup(n => n.GetRegion(It.IsAny<string>(), out emptyRegion));

            var setFolder = new SetFolderCommand(consoleAppHelper.Object);
            setFolder.GetOption<HubId>().SetValue(
                "https://acc.autodesk.com/docs/files/projects/e3be8c87-1df5-470f-9214-1b6cc85452fa?folderUrn=urn%3Aadsk.wipprod%3Afs.folder%3Aco.NBWiKlvJSqOo1B4iUajHeA&viewModel=detail");

            var task = setFolder.Execute();
            task.Wait();
            Assert.AreEqual(false, task.Result);

            consoleAppHelper.Verify(
                n => n.SetFolder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never());
        }

        [TestMethod]
        public void SetFolder_UrlPath_Success_ReturnsTrue()
        {
            var hubId = "b.valid-hub";
            consoleAppHelper.Setup(n => n.GetHubId(It.IsAny<string>(), out hubId));

            var region = "US";
            consoleAppHelper.Setup(n => n.GetRegion(It.IsAny<string>(), out region));

            var setFolder = new SetFolderCommand(consoleAppHelper.Object);
            setFolder.GetOption<HubId>().SetValue(
                "https://acc.autodesk.com/docs/files/projects/e3be8c87-1df5-470f-9214-1b6cc85452fa?folderUrn=urn%3Aadsk.wipprod%3Afs.folder%3Aco.NBWiKlvJSqOo1B4iUajHeA&viewModel=detail");

            var task = setFolder.Execute();
            task.Wait();
            Assert.AreEqual(true, task.Result);

            consoleAppHelper.Verify(
                n => n.SetFolder("US", "b.valid-hub",
                    "b.e3be8c87-1df5-470f-9214-1b6cc85452fa",
                    "urn:adsk.wipprod:fs.folder:co.NBWiKlvJSqOo1B4iUajHeA"),
                Times.Once());
        }

        [TestMethod]
        public void CreateExchange_MissingFolderDetails_ReturnsFalse()
        {
            consoleAppHelper.Setup(n =>
                n.TryGetFolderDetails(out It.Ref<string>.IsAny, out It.Ref<string>.IsAny, out It.Ref<string>.IsAny, out It.Ref<string>.IsAny))
                .Returns(true);

            var createExchange = new CreateExchangeCommand(consoleAppHelper.Object);
            createExchange.GetOption<ExchangeTitle>().SetValue("TestExchange");
            var task = createExchange.Execute();
            task.Wait();

            Assert.AreEqual(false, task.Result);
        }

    }
}
