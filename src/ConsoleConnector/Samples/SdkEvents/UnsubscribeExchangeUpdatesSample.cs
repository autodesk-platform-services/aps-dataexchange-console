using System;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to unsubscribe from exchange update events.
    /// SDK: IEventsController.UnsubscribeToExchangeUpdateEvent.
    /// Console plumbing: ElementSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(9, 2)]
    public sealed class UnsubscribeExchangeUpdatesSample : ISample
    {
        public string Name => "Unsubscribe Exchange Updates";
        public string Description => "Unsubscribe from exchange modified events";

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

            await client.EventsController.UnsubscribeToExchangeUpdateEvent(session.Identifier.ExchangeId);
            TerminalUi.Chat($"Unsubscribed from exchange {session.Identifier.ExchangeId}.");
        }
    }
}
