using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    public interface ISample
    {
        string Name { get; }
        string Description { get; }
        Task RunAsync(SampleContext ctx);
    }
}
