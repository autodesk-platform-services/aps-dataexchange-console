using System;
using System.Linq;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to query design instances by design source id.
    /// SDK: ElementDataModel.GetDesignInstancesBySourceId.
    /// Console plumbing: DesignSampleHelper.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(6, 4)]
    public sealed class GetDesignInstancesBySourceIdSample : ISample
    {
        public string Name => "Get Design Instances By Source Id";
        public string Description => "Find design instances by design source id";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await DesignSampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var designId = Prompt.AskString("Design source id", "chair-id");
            var instances = session.Model.GetDesignInstancesBySourceId(designId).ToList();
            TerminalUi.Chat($"Instances for design source id '{designId}': {instances.Count}");
            foreach (var instance in instances)
                TerminalUi.Chat($"  {instance.Name} ({instance.SourceId})");
        }
    }
}
