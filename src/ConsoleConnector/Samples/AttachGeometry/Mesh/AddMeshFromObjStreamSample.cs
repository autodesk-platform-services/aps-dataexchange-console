using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach OBJ mesh geometry from a stream.
    /// SDK: ElementDataModel.CreateFileGeometry (OBJ stream).
    /// Console plumbing: GeometrySampleHelper.AttachStreamGeometryAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 3, 2)]
    public sealed class AddMeshFromObjStreamSample : ISample
    {
        public string Name => "Add Mesh from OBJ stream";
        public string Description => "Attach OBJ mesh geometry from a stream";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachStreamGeometryAsync(ctx, GeometryFormat.Obj, "OBJ path", ctx.Defaults.ObjPath, "mesh2.obj");
        }
}
