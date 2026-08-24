using System;
using System.Linq;
using Autodesk.DataExchange;
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
    }

    internal sealed class ConsoleProgressListener : IProgressUpdateListener
    {
        public void OnProgressUpdate(string stepId, double progress) =>
            TerminalUi.Progress($"{stepId}: {progress:F1}%");
        }
}
