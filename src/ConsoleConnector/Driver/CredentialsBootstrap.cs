using System;
using System.Configuration;

namespace ConsoleConnector.Driver
{
    // Credentials are resolved in this order and are never written to disk by this class:
    //   1. DXSDK_CLIENT_ID / DXSDK_CLIENT_SECRET environment variables (recommended — safe for CI/shared machines).
    //   2. App.config AuthClientId / AuthClientSecret app settings (local-dev fallback — never commit real values).
    //   3. A one-off interactive prompt for this run only.
    // See README "Credential setup" for the recommended setup.
    internal sealed record ForgeCredentials(string ClientId, string ClientSecret);

    internal static class CredentialsBootstrap
    {
        public static ForgeCredentials Resolve()
        {
            if (TryFromEnvironment(out var fromEnv))
            {
                TerminalUi.Success("Using Forge credentials from DXSDK_CLIENT_ID / DXSDK_CLIENT_SECRET.");
                return fromEnv;
            }

            if (TryFromAppConfig(out var fromConfig))
            {
                TerminalUi.Success("Using Forge credentials from App.config.");
                return fromConfig;
            }

            if (BatchMode.Enabled)
            {
                throw new InvalidOperationException(
                    "No Forge credentials found. Set DXSDK_CLIENT_ID / DXSDK_CLIENT_SECRET environment variables before running --run-all.");
            }

            return PromptForRunOnly();
        }

        private static bool TryFromEnvironment(out ForgeCredentials credentials)
        {
            var clientId = Environment.GetEnvironmentVariable("DXSDK_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("DXSDK_CLIENT_SECRET");
            if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
            {
                credentials = new ForgeCredentials(clientId.Trim(), clientSecret.Trim());
                return true;
            }

            credentials = null!;
            return false;
        }

        private static bool TryFromAppConfig(out ForgeCredentials credentials)
        {
            var clientId = ConfigurationManager.AppSettings["AuthClientId"];
            var clientSecret = ConfigurationManager.AppSettings["AuthClientSecret"];
            if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
            {
                credentials = new ForgeCredentials(clientId.Trim(), clientSecret.Trim());
                return true;
            }

            credentials = null!;
            return false;
        }

        private static ForgeCredentials PromptForRunOnly()
        {
            TerminalUi.Section("First run", "Forge app credentials");
            TerminalUi.Info("Create an app at https://forge.autodesk.com and set callback http://localhost:8080/");
            TerminalUi.Warning(
                "Nothing will be saved. To skip this prompt next time, set DXSDK_CLIENT_ID / DXSDK_CLIENT_SECRET " +
                "environment variables, or add AuthClientId / AuthClientSecret to App.config.");

            var clientId = Prompt.AskString("Forge Client ID", null);
            var clientSecret = Prompt.AskString("Forge Client Secret", null);
            return new ForgeCredentials(clientId, clientSecret);
        }
    }
}
