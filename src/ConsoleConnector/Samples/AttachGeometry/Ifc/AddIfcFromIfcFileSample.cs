using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach IFC geometry from a file.
    /// SDK: ElementDataModel.CreateFileGeometry (IFC file).
    /// Console plumbing: GeometrySampleHelper.AttachFileGeometryAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 2, 1)]
    public sealed class AddIfcFromIfcFileSample : ISample
    {
        public string Name => "Add IFC from IFC file";
        public string Description => "Attach an IFC file as geometry";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachFileGeometryAsync(ctx, GeometryFormat.Ifc, "IFC path", ctx.Defaults.IfcPath, "Beam.ifc");
        }
}
