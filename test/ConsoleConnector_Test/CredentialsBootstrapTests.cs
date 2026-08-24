using System;
using System.Configuration;
using ConsoleConnector.Driver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleConnector_Test
{
    [TestClass]
    public class CredentialsBootstrapTests
    {
        private string? originalClientId;
        private string? originalClientSecret;

        [TestInitialize]
        public void Init()
        {
            originalClientId = ConfigurationManager.AppSettings["AuthClientId"];
            originalClientSecret = ConfigurationManager.AppSettings["AuthClientSecret"];
        }

        [TestCleanup]
        public void Cleanup()
        {
            Environment.SetEnvironmentVariable("DXSDK_CLIENT_ID", null);
            Environment.SetEnvironmentVariable("DXSDK_CLIENT_SECRET", null);
            BatchMode.Enabled = false;
            ConfigurationManager.AppSettings.Set("AuthClientId", originalClientId);
            ConfigurationManager.AppSettings.Set("AuthClientSecret", originalClientSecret);
        }

        [TestMethod]
        public void Resolve_EnvironmentVariablesPresent_TakesPriorityOverAppConfig()
        {
            ConfigurationManager.AppSettings.Set("AuthClientId", "config-client-id");
            ConfigurationManager.AppSettings.Set("AuthClientSecret", "config-client-secret");
            Environment.SetEnvironmentVariable("DXSDK_CLIENT_ID", " env-client-id ");
            Environment.SetEnvironmentVariable("DXSDK_CLIENT_SECRET", " env-client-secret ");

            var credentials = CredentialsBootstrap.Resolve();

            Assert.AreEqual("env-client-id", credentials.ClientId);
            Assert.AreEqual("env-client-secret", credentials.ClientSecret);
        }

        [TestMethod]
        public void Resolve_NoEnvironmentVariables_FallsBackToAppConfig()
        {
            Environment.SetEnvironmentVariable("DXSDK_CLIENT_ID", null);
            Environment.SetEnvironmentVariable("DXSDK_CLIENT_SECRET", null);
            ConfigurationManager.AppSettings.Set("AuthClientId", "config-client-id");
            ConfigurationManager.AppSettings.Set("AuthClientSecret", "config-client-secret");

            var credentials = CredentialsBootstrap.Resolve();

            Assert.AreEqual("config-client-id", credentials.ClientId);
            Assert.AreEqual("config-client-secret", credentials.ClientSecret);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Resolve_BatchModeAndNoCredentialsAnywhere_Throws()
        {
            Environment.SetEnvironmentVariable("DXSDK_CLIENT_ID", null);
            Environment.SetEnvironmentVariable("DXSDK_CLIENT_SECRET", null);
            ConfigurationManager.AppSettings.Set("AuthClientId", "");
            ConfigurationManager.AppSettings.Set("AuthClientSecret", "");
            BatchMode.Enabled = true;

            CredentialsBootstrap.Resolve();
        }
    }
}
