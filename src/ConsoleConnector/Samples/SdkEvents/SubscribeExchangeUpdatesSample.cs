using System;
using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Events;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to subscribe to real-time exchange-modified events.
    /// SDK: IEventsController.SubscribeToExchangeUpdateEvent.
    /// Console plumbing: ElementSampleHelper.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(9, 1)]
    public sealed class SubscribeExchangeUpdatesSample : ISample
    {
        public string Name => "Subscribe Exchange Updates";
        public string Description => "Subscribe to exchange modified events";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await ElementSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var client = DiagnosticsSampleHelper.RequireClient(ctx);
            if (client.EventsController == null)
            {
                TerminalUi.Chat("Events controller is not available.");
                return;
            }

            void Handler(object? sender, ExchangeUpdatedEventArgs e)
            {
                TerminalUi.Event($"[event] Exchange={e.ExchangeID} Collection={e.CollectionID} Revision={e.RevisionID}");
        }

            client.EventsController.ExchangeModified += Handler;
            client.EventsController.SubscribeToExchangeUpdateEvent(session.Identifier.ExchangeId);
            TerminalUi.Chat($"Subscribed to updates for exchange {session.Identifier.ExchangeId}.");
            TerminalUi.Info("Waiting for events (sync this exchange elsewhere to trigger)...");
            Prompt.AskString("[Enter to unsubscribe and return]", null);
            await client.EventsController.UnsubscribeToExchangeUpdateEvent(session.Identifier.ExchangeId);
            client.EventsController.ExchangeModified -= Handler;
            TerminalUi.Chat("Unsubscribed.");
        }
    }
}
