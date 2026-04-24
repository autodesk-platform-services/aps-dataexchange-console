using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Helper;
using Autodesk.DataExchange.ConsoleApp.Interfaces;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    /// <summary>
    /// Set default folder details for new exchange creation
    /// </summary>
    internal class SetFolderCommand : Command
    {
        public SetFolderCommand(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            Name = "SetFolder";
            Description = "Set default folder details for exchange creation.";
            Options = new List<CommandOption>
            {
                new HubId(),
                new Region(),
                new ProjectUrn(),
                new FolderUrn(),
            };
        }

        public SetFolderCommand(SetFolderCommand setFolderCommand) : base(setFolderCommand)
        {
        }

        public override Command Clone()
        {
            return new SetFolderCommand(this);
        }

        public override async Task<bool> Execute()
        {
            var check = this.GetOption<HubId>().Value;
            if (check != null && check.Contains("http"))
            {
                return await ExecuteWithUrl(check);
            }
            if (this.ValidateOptions() == false)
            {
                var missing = new List<string>();
                if (!this.GetOption<HubId>().IsValid()) missing.Add("HubId");
                if (!this.GetOption<Region>().IsValid()) missing.Add("Region");
                if (!this.GetOption<ProjectUrn>().IsValid()) missing.Add("ProjectUrn");
                if (!this.GetOption<FolderUrn>().IsValid()) missing.Add("FolderUrn");
                Console.WriteLine($"[ERROR] Invalid inputs. Missing required fields: {string.Join(", ", missing)}");
                return false;
            }
            var hubId = this.GetOption<HubId>();
            var region = this.GetOption<Region>();
            var projectUrn = this.GetOption<ProjectUrn>();
            var folderUrn = this.GetOption<FolderUrn>();

            var validation = await ConsoleAppHelper.ValidateHubAccessAsync(hubId.Value, projectUrn.Value);
            if (!validation.IsValid)
            {
                Console.WriteLine(validation.ErrorMessage);
                return false;
            }

            ConsoleAppHelper.GetRegion(hubId.Value, out string resolvedRegion);
            if (!string.IsNullOrEmpty(resolvedRegion) &&
                !string.Equals(region.Value, resolvedRegion, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[WARNING] Provided region '{region.Value}' differs from the resolved region '{resolvedRegion}'. Using resolved region.");
                ConsoleAppHelper.SetFolder(resolvedRegion, hubId.Value, projectUrn.Value, folderUrn.Value);
            }
            else
            {
                ConsoleAppHelper.SetFolder(region.Value, hubId.Value, projectUrn.Value, folderUrn.Value);
            }
            Console.WriteLine("Default folder set!!!");
            return true;
        }

        private Task<bool> ExecuteWithUrl(string folderUrl)
        {
            string projectUrn;
            string folderUrn;
            try
            {
                projectUrn = "b." + folderUrl.Split('/')[6].Split('?')[0];
                folderUrn = folderUrl.Split('/')[6].Split('?')[1].Split('&')[0].Split('=')[1].Replace("%3A", ":");
            }
            catch (Exception)
            {
                Console.WriteLine("[ERROR] The provided URL format is not recognized. Expected a Forma/ACC folder URL.");
                return Task.FromResult(false);
            }

            if (string.IsNullOrEmpty(projectUrn) || projectUrn == "b.")
            {
                Console.WriteLine("[ERROR] Could not extract ProjectUrn from the URL. Please verify the URL format.");
                return Task.FromResult(false);
            }

            if (string.IsNullOrEmpty(folderUrn))
            {
                Console.WriteLine("[ERROR] Could not extract FolderUrn from the URL. Please verify the URL contains a valid folderUrn query parameter.");
                return Task.FromResult(false);
            }

            ConsoleAppHelper.GetHubId(projectUrn, out string hubId);
            if (string.IsNullOrEmpty(hubId))
            {
                Console.WriteLine(
                    $"[ERROR] Unable to resolve HubId from the folder URL. " +
                    "This usually means your app's ClientId has not been added to the Forma/ACC hub " +
                    $"as a custom integration, or the project in the URL does not exist. (ProjectUrn: '{projectUrn}')");
                return Task.FromResult(false);
            }

            ConsoleAppHelper.GetRegion(hubId, out string region);
            if (string.IsNullOrEmpty(region))
            {
                Console.WriteLine($"[ERROR] Unable to resolve region for the hub. The HubId '{hubId}' may be invalid or inaccessible.");
                return Task.FromResult(false);
            }

            ConsoleAppHelper.SetFolder(region, hubId, projectUrn, folderUrn);
            Console.WriteLine("Default folder set!!!");
            return Task.FromResult(true);
        }
    }
}
