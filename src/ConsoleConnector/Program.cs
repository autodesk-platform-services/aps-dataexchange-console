using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.DataExchange.Exceptions;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;
using ConsoleConnector.Common;

namespace ConsoleConnector
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var runAll = args.Any(a => string.Equals(a, "--run-all", StringComparison.OrdinalIgnoreCase));
            if (runAll)
                BatchMode.Enabled = true;

            TerminalUi.ShowBanner();

            try
            {
                var session = SessionStore.Load();

                var credentials = CredentialsBootstrap.Resolve();
                var client = await SdkBootstrap.CreateClientAsync(credentials.ClientId, credentials.ClientSecret);

                await SdkBootstrap.SignInAsync(client);

                var ctx = new SampleContext(client, session.Defaults);

                session = await Bootstrap.EnsureSessionAsync(session, ctx);
                SessionStore.Apply(session, ctx);

                if (ctx.LastExchange != null)
                {
                    var title = ctx.LastExchange.Title;
                    var restored = false;
                    await TerminalUi.RunWithStatusAsync(
                        $"Restoring {title}…",
                        async () => restored = await ExchangeSessionHelper.TryRestoreAsync(ctx).ConfigureAwait(false));
                    if (restored)
                        TerminalUi.Success($"Restored last exchange: {title}");
                }

                if (runAll)
                {
                    var result = await SampleRunner.RunAllAsync(ctx, session);
                    TerminalUi.Section("Summary");
                    TerminalUi.Info($"{result.Passed} passed, {result.Failed} failed (of {result.Results.Count})");
                    foreach (var failure in result.Results.Where(r => r.Error != null))
                        TerminalUi.Error($"{failure.Key} {failure.Name}: {failure.Error!.Split('\n')[0]}");

                    Environment.ExitCode = result.Failed > 0 ? 1 : 0;
                    return;
                }

                TerminalUi.Rule("Ready");
                await Menu.RunAsync(ctx, session);
            }
            catch (AllCallbackPortsInUseException ex)
            {
                TerminalUi.Error("Authentication failed: all callback URLs are in use.");
                TerminalUi.WritePanel("Attempted URLs", string.Join(Environment.NewLine, ex.AttemptedUrls));
                TerminalUi.Dim(ex.Message);
            }
            catch (PortInUseException ex)
            {
                TerminalUi.Error("Authentication failed: callback port is in use.");
                TerminalUi.Dim(ex.Message);
                TerminalUi.Info("Close the app using that port, or add FallbackRedirectUrls in SdkBootstrap.");
            }
            catch (Exception ex)
            {
                TerminalUi.Error($"Fatal: {ex}");
                Environment.ExitCode = 1;
            }
        }
    }
}
