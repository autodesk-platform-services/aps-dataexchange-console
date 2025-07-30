using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [TestInitialize]
        public void TestInit()
        {
            consoleAppHelper = new Mock<IConsoleAppHelper>();
            var detauldFolder = "test";
            consoleAppHelper.Setup(n =>
                n.TryGetFolderDetails(out detauldFolder, out detauldFolder, out detauldFolder, out detauldFolder));

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
                 return JsonConvert.DeserializeObject<ExchangeDetails>(
                    "{\"ProjectUrn\":\"b.e3be8c87-1df5-470f-9214-1b6cc85452fa\",\"FileUrn\":\"urn:adsk.wipprod:dm.lineage:IpFw2xoRRTS__n5-kV5XXA\",\"FileVersionUrn\":\"urn:adsk.wipprod:fs.file:vf.IpFw2xoRRTS__n5-kV5XXA?version=1\",\"FolderUrn\":\"urn:adsk.wipprod:fs.folder:co.NBWiKlvJSqOo1B4iUajHeA\",\"ExchangeID\":\"7a3102e6-645c-3b88-8e08-b5f3f0e243be\",\"CollectionID\":\"co.cBMZ-5QhTym2c-nfa1Fx2Q\",\"DisplayName\":\"TestExchange\",\"CreatedTime\":\"2023-08-21T15:41:30.0481133+05:30\",\"LastModifiedTime\":\"2023-08-21T15:41:30.0481133+05:30\",\"CreatedBy\":\"DhirajLotake\",\"LastModifiedBy\":\"DhirajLotake\",\"Attributes\":{},\"FolderPath\":\"\",\"SchemaNamespace\":\"c73cae7ea1540e39f45528aa243d4d26\",\"HubId\":null,\"HubRegion\":null}");
            });

            consoleAppHelper.Setup(n => n.IsExchangeUpdated(It.IsAny<string>())).Returns(() =>
            {
                return true;
            } );

            consoleAppHelper.Setup(n => n.SyncExchange(null,null,null)).ReturnsAsync(() =>
            {
                return true;
            });

            consoleAppHelper.Setup(n => n.GetExchangeData(It.IsAny<string>())).Returns(() =>
            {
                return ElementDataModel.Create(null).ExchangeData;
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
    }
}
