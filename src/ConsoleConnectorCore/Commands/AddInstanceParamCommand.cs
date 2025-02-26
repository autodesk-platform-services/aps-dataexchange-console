using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Helper;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using Autodesk.DataExchange.DataModels;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    internal class AddInstanceParamCommand : AddParamCommand
    { 
        public AddInstanceParamCommand(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            this.IsInstanceParameter = true;
            this.Name = "AddInstanceParameter";
            Description = "Add instance parameters to element.";
        }

        public AddInstanceParamCommand(AddInstanceParamCommand addTypeParamCommand) : base(addTypeParamCommand)
        {
            this.IsInstanceParameter = addTypeParamCommand.IsInstanceParameter;
        }

        public override Command Clone()
        {
            return new AddInstanceParamCommand(this);
        }
    }
}
