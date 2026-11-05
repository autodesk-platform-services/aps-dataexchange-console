using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DataExchange;
using Autodesk.DataExchange.Exceptions;
using Autodesk.DataExchange.Core.Interface;
using Autodesk.DataExchange.ProgressManager.Enums;
using Autodesk.DataExchange.ProgressManager.Interfaces;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;

namespace ConsoleConnector.Common
{
    internal static class DiagnosticsSampleHelper
    {
        internal const string TrackingId = "ConsoleConnectorSample";

        internal static Client RequireClient(SampleContext ctx) =>
            GeometrySampleHelper.RequireClient(ctx);

        internal static ILogger GetLogger(Client client) =>
            client.SDKOptions!.Logger;

        /// <summary>
        /// Walks an exception chain (inner and aggregate exceptions) breadth-first and returns the HTTP status of the
        /// first exception that implements <see cref="IStatusCodeException"/>, or null if none does. Aggregate inner
        /// exceptions are visited in their original order.
        /// </summary>
        internal static int? FindStatusCode(Exception? exception)
        {
            var pending = new Queue<Exception>();
            if (exception != null)
                pending.Enqueue(exception);
            while (pending.Count > 0)
            {
                var current = pending.Dequeue();
                if (current is IStatusCodeException withStatus)
                    return withStatus.StatusCode;
                if (current is AggregateException aggregate)
                {
                    foreach (var inner in aggregate.InnerExceptions)
                        pending.Enqueue(inner);
                }
                else if (current.InnerException != null)
                {
                    pending.Enqueue(current.InnerException);
                }
            }

            return null;
        }

        internal static void EnableHttpDebugLogging(ILogger logger)
        {
            logger.SetDebugLogLevel();
            TerminalUi.Info($"Debug logging enabled. Request logs: {logger.RequestLogPath}");
            TerminalUi.Info($"Application logs: {logger.LogPath}");
        }

        internal static void PrintProgressSteps(IProgressStepsManager manager, Operation operation)
        {
            var steps = manager.GetProgressStepsByOperation(operation).ToList();
            TerminalUi.Chat($"  Operation {operation}: {steps.Count} step(s)");
            foreach (var step in steps)
                TerminalUi.Chat($"    {step.StepId}: {step.Title} -> {step.CompletionTitle}");
        }

        internal static void RegisterConsoleProgressListener(IProgressStepsManager manager) =>
            manager.RegisterProgressUpdateListener(new ConsoleProgressListener());

        internal static void UnregisterProgressUpdateListener(IProgressStepsManager manager) =>
            manager.RegisterProgressUpdateListener(NullProgressUpdateListener.Instance);
    }

    internal sealed class ConsoleProgressListener : IProgressUpdateListener
    {
        public void OnProgressUpdate(string stepId, double progress) =>
            TerminalUi.Progress($"{stepId}: {progress:F1}%");
    }

    internal sealed class NullProgressUpdateListener : IProgressUpdateListener
    {
        internal static readonly NullProgressUpdateListener Instance = new NullProgressUpdateListener();

        private NullProgressUpdateListener()
        {
        }

        public void OnProgressUpdate(string stepId, double progress)
        {
        }
    }
}
