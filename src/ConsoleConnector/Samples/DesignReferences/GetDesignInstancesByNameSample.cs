using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to query design instances by design name.
    /// SDK: ElementDataModel.GetDesignInstancesByName.
    /// Console plumbing: DesignSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(6, 5)]
    public sealed class GetDesignInstancesByNameSample : ISample
    {
        public string Name => "Get Design Instances By Name";
        public string Description => "Find design instances by design name";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await DesignSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var designName = Prompt.AskString("Design name", "Chair");
            var instances = session.Model.GetDesignInstancesByName(designName).ToList();
            TerminalUi.Chat($"Instances for design '{designName}': {instances.Count}");
            foreach (var instance in instances)
                TerminalUi.Chat($"  {instance.Name} ({instance.SourceId})");
        }
    }
}
