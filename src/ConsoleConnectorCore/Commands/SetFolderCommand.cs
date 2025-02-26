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

        public override Task<bool> Execute()
        {
            var check = this.GetOption<HubId>().Value;
            if (check.Contains("http"))
            {
                return Execute(check);
            }
            if (this.ValidateOptions() == false)
            {
                Console.WriteLine("Invalid inputs!!!");
                return Task.FromResult(false);
            }
            var hubId = this.GetOption<HubId>();
            var region = this.GetOption<Region>();
            var projectUrn = this.GetOption<ProjectUrn>();
            var folderUrn = this.GetOption<FolderUrn>();
            ConsoleAppHelper.SetFolder(region.Value,hubId.Value,projectUrn.Value,folderUrn.Value);
            Console.WriteLine("Default folder set!!!");
            return Task.FromResult(true);
        }

        private Task<bool> Execute(string folderUrl)
        {
            var projectUrn = "b." + folderUrl.ToString().Split('/')[6].Split('?')[0];
            var folderUrn = folderUrl.ToString().Split('/')[6].Split('?')[1].Split('&')[0].Split('=')[1].Replace("%3A", ":");

            ConsoleAppHelper.GetHubId(projectUrn, out string hubId);
            if (string.IsNullOrEmpty(hubId))
            {
                Console.WriteLine("Invalid FolderUrl!!!");
                return Task.FromResult(false);
            }
            ConsoleAppHelper.GetRegion(hubId, out string region);
            if(string.IsNullOrEmpty(region)) 
            {
                Console.WriteLine("Invalid FolderUrl!!!");
                return Task.FromResult(false);
            }
            if(string.IsNullOrEmpty(folderUrn))
            {
                Console.WriteLine("Invalid FolderUrl!!!");
                return Task.FromResult(false);
            }
            if(string.IsNullOrEmpty(projectUrn))
            {
                Console.WriteLine("Invalid FolderUrl!!!");
                return Task.FromResult(false);
            }
            ConsoleAppHelper.SetFolder(region, hubId, projectUrn, folderUrn);
            Console.WriteLine("Default folder set!!!");
            return Task.FromResult(true);
        }
    }
}
