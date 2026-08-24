using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach IFC geometry from a memory stream.
    /// SDK: ElementDataModel.CreateFileGeometry (IFC stream).
    /// Console plumbing: GeometrySampleHelper.AttachStreamGeometryAsync.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(4, 2, 2)]
    public sealed class AddIfcFromIfcStreamSample : ISample
    {
        public string Name => "Add IFC from IFC stream";
        public string Description => "Attach IFC geometry from a memory stream";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachStreamGeometryAsync(ctx, GeometryFormat.Ifc, "IFC path", ctx.Defaults.IfcPath, "Beam.ifc");
        }
}
