using System;
using System.Threading.Tasks;
using Autodesk.DataExchange.ProgressManager.Enums;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: SDK operations expose named progress steps.
    /// SDK: IProgressStepsManager.GetProgressStepsByOperation.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(10, 3)]
    public sealed class ShowProgressStepsSample : ISample
    {
        public string Name => "Show Progress Steps";
        public string Description => "List default progress steps per operation";

        public Task RunAsync(SampleContext ctx)
        {
            var manager = ctx.Client.ProgressStepsManager;
            DiagnosticsSampleHelper.PrintProgressSteps(manager, Operation.Create);
            DiagnosticsSampleHelper.PrintProgressSteps(manager, Operation.Update);
            DiagnosticsSampleHelper.PrintProgressSteps(manager, Operation.LoadLatest);
            return Task.CompletedTask;
        }
    }
}
