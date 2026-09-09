using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to build and attach in-memory mesh via MeshAPI.
    /// SDK: ElementDataModel.CreateMeshGeometry.
    /// Console plumbing: GeometrySampleHelper.AttachMeshAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 3, 3)]
    public sealed class AddMeshFromTritonSample : ISample
    {
        public string Name => "Add Mesh from Triton";
        public string Description => "Attach in-memory MeshAPI mesh geometry";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachMeshAsync(ctx, GeometrySampleHelper.CreateSampleMesh(), "ConsoleConnectorMesh");
        }
}
