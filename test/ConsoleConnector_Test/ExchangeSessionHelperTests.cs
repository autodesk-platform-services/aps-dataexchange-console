using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using Autodesk.DataExchange.Models;
using ConsoleConnector.Common;
using ConsoleConnector.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace ConsoleConnector_Test
{
    [TestClass]
    public class ExchangeSessionHelperTests
    {
        private static ExchangeDetails NewDetails(string exchangeId = "test-exchange-id", string? hubId = null) =>
            JsonConvert.DeserializeObject<ExchangeDetails>(
                "{\"ProjectUrn\":\"b.test-project\",\"FileUrn\":\"urn:adsk.test:dm.lineage:test-file\"," +
                "\"FileVersionUrn\":\"urn:adsk.test:fs.file:vf.test-file?version=1\",\"FolderUrn\":\"urn:adsk.test:fs.folder:co.test-folder\"," +
                $"\"ExchangeID\":\"{exchangeId}\",\"CollectionID\":\"co.test-collection\",\"DisplayName\":\"Test Exchange\"," +
                $"\"HubId\":{(hubId == null ? "null" : $"\"{hubId}\"")},\"HubRegion\":\"US\"}}")!;

        [TestMethod]
        public void ToIdentifier_UsesDetailsHubId_WhenPresent()
        {
            var details = NewDetails(hubId: "hub-from-details");

            var identifier = ExchangeSessionHelper.ToIdentifier(details, "hub-fallback");

            Assert.AreEqual("test-exchange-id", identifier.ExchangeId);
            Assert.AreEqual("co.test-collection", identifier.CollectionId);
            Assert.AreEqual("hub-from-details", identifier.HubId);
        }

        [TestMethod]
        public void ToIdentifier_FallsBackToSessionHubId_WhenDetailsHubIdMissing()
        {
            var details = NewDetails(hubId: null);

            var identifier = ExchangeSessionHelper.ToIdentifier(details, "hub-fallback");

            Assert.AreEqual("hub-fallback", identifier.HubId);
        }

        [TestMethod]
        public void RegisterLoaded_TracksActiveExchangeAndLastExchange()
        {
            var ctx = new SampleContext(new Mock<IClient>().Object, new Defaults());
            var model = ElementDataModel.Create(ctx.Client);
            var details = NewDetails();

            ExchangeSessionHelper.RegisterLoaded(ctx, details, model);

            Assert.IsTrue(ctx.Exchanges.ContainsKey("Test Exchange"));
            Assert.AreEqual("Test Exchange", ctx.LastExchangeTitle);
            Assert.IsNotNull(ctx.LastExchange);
            Assert.AreEqual("test-exchange-id", ctx.LastExchange!.ExchangeId);
        }

        [TestMethod]
        public void ClearLastIfMatches_MatchingFileUrn_ClearsLastExchange()
        {
            var ctx = new SampleContext(new Mock<IClient>().Object, new Defaults());
            var model = ElementDataModel.Create(ctx.Client);
            var details = NewDetails();
            ExchangeSessionHelper.RegisterLoaded(ctx, details, model);

            ExchangeSessionHelper.ClearLastIfMatches(ctx, details.FileUrn);

            Assert.IsNull(ctx.LastExchange);
            Assert.IsNull(ctx.LastExchangeTitle);
        }

        [TestMethod]
        public void ClearLastIfMatches_DifferentFileUrn_LeavesLastExchangeUntouched()
        {
            var ctx = new SampleContext(new Mock<IClient>().Object, new Defaults());
            var model = ElementDataModel.Create(ctx.Client);
            var details = NewDetails();
            ExchangeSessionHelper.RegisterLoaded(ctx, details, model);

            ExchangeSessionHelper.ClearLastIfMatches(ctx, "urn:adsk.test:dm.lineage:some-other-file");

            Assert.IsNotNull(ctx.LastExchange);
        }

        [TestMethod]
        public void RememberCreated_SetsLastCreatedExchangeAndTitle()
        {
            var ctx = new SampleContext(new Mock<IClient>().Object, new Defaults());
            var details = NewDetails();

            ExchangeSessionHelper.RememberCreated(ctx, details, "fallback-name");

            Assert.AreSame(details, ctx.LastCreatedExchange);
            Assert.AreEqual("Test Exchange", ctx.LastExchangeTitle);
        }
    }
}
