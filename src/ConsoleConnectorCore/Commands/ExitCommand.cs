using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    /// <summary>
    /// Close application.
    /// </summary>
    internal class ExitCommand : Command
    {
        public ExitCommand()
        {
            Name = "Exit";
            Description = "Close application.";
        }

        public ExitCommand(ExitCommand exitCommand) : base(exitCommand)
        {

        }

        public override Command Clone()
        {
            return new ExitCommand(this);
        }

        public override Task<bool> Execute()
        {
            Environment.Exit(0);
            return Task.FromResult(true);
        }
    }
}
