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
    internal class AddTypeParamCommand: AddParamCommand
    {
        public AddTypeParamCommand(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            this.IsInstanceParameter = false;
            this.Name = "AddTypeParameter";
            Description = "Add type parameters to element.";
        }

        public AddTypeParamCommand(AddTypeParamCommand addTypeParamCommand) : base(addTypeParamCommand)
        {
            this.IsInstanceParameter = addTypeParamCommand.IsInstanceParameter;
        }

        public override Command Clone()
        {
            return new AddTypeParamCommand(this);
        }
    }
}
