using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Autodesk.DataExchange;
using Autodesk.DataExchange.Authentication;
using Autodesk.DataExchange.Core.Interface;
using Autodesk.DataExchange.Extensions.Logging.File;
using Autodesk.DataExchange.Interface;

namespace ConsoleConnector.Driver
{
    internal static class SdkBootstrap
    {
        public static Task<IClient> CreateClientAsync(string clientId, string clientSecret)
        {
            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                throw new InvalidOperationException("Forge Client ID and Client Secret are required.");

            var logDir = Path.Combine(SessionStore.SessionDirectory, "logs");
            Directory.CreateDirectory(logDir);

            var logger = new Log(logDir);
            logger.SetDebugLogLevel();

            var callback = AppSetting("AuthCallback", "http://localhost:8080/");
            var auth = new Auth(new AuthOptions
            {
                ClientId = clientId.Trim(),
                ClientSecret = clientSecret.Trim(),
                CallBack = callback,
                FallbackRedirectUrls = new List<string>
                {
                    "http://127.0.0.1:63212/",
                    "http://localhost:9090/",
                    "http://localhost:3000/",
                },
                Logger = logger,
            });

            // SDKOptionsDefaultSetup (not a hand-built SDKOptions) supplies the SourceProvider /
            // ContractProvider / HostingProvider / Storage that this SDK build expects — constructing
            // those manually leads Client.Initialize() down a code path this alpha build can't satisfy.
            var sdkOptions = new SDKOptionsDefaultSetup
            {
                ConnectorName = AppSetting("ConnectorName", "ConsoleConnector"),
                AuthProvider = auth,
                Logger = logger,
                ConnectorVersion = AppSetting("ConnectorVersion", GetSdkVersion()),
                HostApplicationName = AppSetting("HostApplicationName", "ConsoleConnector"),
                HostApplicationVersion = AppSetting("HostApplicationVersion", GetHostVersion()),
            };

            IClient client = new Client(sdkOptions);
            return Task.FromResult(client);
        }

        private static string AppSetting(string key, string fallback)
        {
            var value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        public static async Task SignInAsync(IClient client)
        {
            var auth = GetAuthProvider(client);

            TerminalUi.Section("Sign in", "Your browser will open for Autodesk login");
            TerminalUi.WriteMarkupPanel(
                "Forge callback URLs",
                "[dim]Add these to your Forge app if you have not already:[/]\n" +
                "http://localhost:8080/\n" +
                "http://127.0.0.1:63212/\n" +
                "http://localhost:9090/\n" +
                "http://localhost:3000/");

            await TerminalUi.RunWithStatusAsync(
                "Waiting for browser login…",
                async () => await auth.GetAuthTokenAsync().ConfigureAwait(false))
                .ConfigureAwait(false);

            var user = await auth.GetUserAccountAsync().ConfigureAwait(false);
            TerminalUi.Success($"Signed in as {user.FirstName} {user.LastName} ({user.Email})");
        }

        private static string GetSdkVersion()
        {
            var version = typeof(Client).Assembly.GetName().Version;
            return version != null ? version.ToString(3) : "unknown";
        }

        private static string GetHostVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version != null ? version.ToString(3) : "unknown";
        }

        private static IAuth GetAuthProvider(IClient client)
        {
            if (client is Client concrete && concrete.SDKOptions?.AuthProvider != null)
                return concrete.SDKOptions.AuthProvider;

            throw new InvalidOperationException("AuthProvider is not configured on this client.");
        }
    }
}
