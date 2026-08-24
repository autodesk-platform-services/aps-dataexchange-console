using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach OBJ mesh geometry from a file.
    /// SDK: ElementDataModel.CreateFileGeometry (OBJ file).
    /// Console plumbing: GeometrySampleHelper.AttachFileGeometryAsync.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(4, 3, 1)]
    public sealed class AddMeshFromObjFileSample : ISample
    {
        public string Name => "Add Mesh from OBJ file";
        public string Description => "Attach an OBJ mesh file";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachFileGeometryAsync(ctx, GeometryFormat.Obj, "OBJ path", ctx.Defaults.ObjPath, "mesh2.obj");
        }
}
