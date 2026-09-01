using System.Threading.Tasks;
using Autodesk.DataExchange.Models;
using Autodesk.GeometryUtilities.PrimitivesAPI;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Exchange-level viewable orientation (UP, Front, North).
    /// SDK: SetViewableWorldCoordinates / GetViewableWorldCoordinates.
    /// Console plumbing: GeometrySampleHelper.BeginAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 5, 4)]
    public sealed class SetViewableWorldCoordinatesSample : ISample
    {
        public string Name => "Set Viewable World Coordinates";
        public string Description => "Set viewable world coordinate vectors on the exchange";

        public async Task RunAsync(SampleContext ctx)
        {
            var session = await GeometrySampleHelper.BeginAsync(ctx);
            if (session == null)
                return;
            var coords = new ViewableWorldCoordinates
            {
                UP = new Vector3d { X = 0, Y = 0, Z = 1 },
                Front = new Vector3d { X = 0, Y = 1, Z = 0 },
                North = new Vector3d { X = 1, Y = 0, Z = 0 },
            };

            session.Model.SetViewableWorldCoordinates(coords);
            var current = session.Model.GetViewableWorldCoordinates();
            TerminalUi.Chat("Viewable world coordinates set.");
            if (current != null)
            {
                TerminalUi.Chat($"  UP:    {current.UP}");
            TerminalUi.Chat($"  Front: {current.Front}");
            TerminalUi.Chat($"  North: {current.North}");
        }

            await ElementSampleHelper.SyncAsync(ctx, session);
        }
    }
}
