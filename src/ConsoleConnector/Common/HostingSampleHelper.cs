using ConsoleConnector.Samples;
using System;
using System.Threading.Tasks;
using Autodesk.DataExchange;
using Autodesk.DataExchange.Core.Interface;
using Autodesk.DataExchange.Core.Models;

namespace ConsoleConnector.Common
{
    /// <summary>
    /// Console plumbing for samples — resolves IHostingProvider from the SDK client.
    /// </summary>
    internal static class HostingSampleHelper
    {
        internal static IHostingProvider GetProvider(SampleContext ctx)
        {
            if (ctx.Client is Client client && client.SDKOptions?.HostingProvider != null)
                return client.SDKOptions.HostingProvider;

            throw new InvalidOperationException("HostingProvider is not available on this client.");
        }

        internal static async Task<ProjectInfo?> ResolveProjectAsync(SampleContext ctx)
        {
            if (!NavigationHelper.EnsureFullFolder(ctx))
                return null;

            var hosting = GetProvider(ctx);
            return await hosting.GetProjectInformationAsync(ctx.Folder!.HubId, ctx.Folder.ProjectUrn);
        }
    }
}
