using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Helper;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using Autodesk.DataExchange.Core.Enums;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    /// <summary>
    /// Help for all commands.
    /// </summary>
    internal class HelpCommand :Command
    {
        public HelpCommand(IConsoleAppHelper consoleAppHelper) : base(consoleAppHelper)
        {
            this.Name = "Help";
            Description = "List all supported commands.";
            Options = new List<CommandOption>
            {
                new Options.HelpCommand()
            };
        }

        public HelpCommand(HelpCommand help) : base(help)
        {
        }

        public override Command Clone()
        {
            return new HelpCommand(this);
        }

        public override Task<bool> Execute()
        {
            var helpOption = this.GetOption<Options.HelpCommand>();
            if (helpOption!=null && string.IsNullOrEmpty(helpOption.Value)==false)
            {
                var command = ConsoleAppHelper.Commands.FirstOrDefault(n => n.Name.ToLower() == helpOption.Value?.ToLower());
                if (command == null)
                    return Task.FromResult(false);

                Console.WriteLine();
                Console.Write(command.Description + "\n");
                var allOptions = string.Join(" ", command.Options.Select(n => "[" + n.GetType().Name.Replace("Option", "") + "]"));
                Console.Write(command.Name.ToUpper() + " " + allOptions + "\n");
                var maxNameLength = command.Options.Count > 0 ? command.Options.Max(n => n.GetType().Name.Replace("Option", "").Length) + 5 : 0;
                var maxDescriptionLength = command.Options.Count > 0 ? command.Options.Max(n => n.Description.Length) + 5 : 0;
                foreach (var option in command.Options)
                {
                    var typeName = option.GetType().Name;
                    typeName = typeName.Replace("Option", "");

                    var str = string.Format("|{0,-" + maxNameLength + "} | {1,-" + maxDescriptionLength + "} |", typeName, option.Description);
                    Console.WriteLine(str);
                }
                Console.WriteLine();

                if (command is AddInstanceParamCommand || command is AddTypeParamCommand)
                {
                    Console.WriteLine("Instance parameter data types");
                    foreach (ParameterDataType parameter in (ParameterDataType[])Enum.GetValues(typeof(ParameterDataType)))
                    {
                        Console.WriteLine(parameter.ToString());
                    }
                    Console.WriteLine();

                    Console.WriteLine("Instance parameter types");
                    var lst = new List<string>();
                    foreach (Autodesk.Parameters.Parameter parameter in (Autodesk.Parameters.Parameter[])Enum.GetValues(typeof(Autodesk.Parameters.Parameter)))
                    {
                        lst.Add(parameter.ToString());
                        //Console.WriteLine(parameter.ToString());
                    }
                    var subList = lst.Select((x, i) => new { Index = i, Value = x })
                        .GroupBy(x => x.Index / 4)
                        .Select(x => x.Select(v => v.Value).ToList())
                        .ToList();
                    var maxLength1 = subList.Max(n => n.ElementAtOrDefault(0)?.Length)??0;
                    var maxLength2 = subList.Max(n => n.ElementAtOrDefault(1)?.Length) ?? 0;
                    var maxLength3 = subList.Max(n => n.ElementAtOrDefault(2)?.Length) ?? 0;
                    var maxLength4 = subList.Max(n => n.ElementAtOrDefault(3)?.Length) ?? 0;
                    foreach (var row in subList)
                    {
                        var str = string.Format("|{0,-" + maxLength1 + "} | {1,-" + maxLength2 + "} | {2,-" + maxLength3 + "} | {3,-" + maxLength4 + "} |", row.ElementAtOrDefault(0), row.ElementAtOrDefault(1), row.ElementAtOrDefault(2), row.ElementAtOrDefault(3));
                        Console.WriteLine(str);
                    }
                    Console.WriteLine();
                }
                else if (command is CreatePrimitiveGeometryCommand)
                {
                    Console.WriteLine("Primitive geometry types");
                    foreach (PrimitiveGeometryType parameter in (PrimitiveGeometryType[])Enum.GetValues(typeof(PrimitiveGeometryType)))
                    {
                        Console.WriteLine(parameter.ToString());
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.Write("For more information on a specific command, type HELP command-name.\n");
                var maxNameLength = ConsoleAppHelper.Commands.Max(n => n.Name.Length)+5;
                var maxDescriptionLength = ConsoleAppHelper.Commands.Max(n => n.Description.Length) + 5;
                Console.WriteLine();
                foreach (var command in ConsoleAppHelper.Commands)
                {
                    var str = string.Format("|{0,-"+ maxNameLength + "} | {1,-"+ maxDescriptionLength + "} |", command.Name, command.Description);
                    Console.WriteLine(str);
                }
                Console.WriteLine();
            }

            return Task.FromResult(true);
        }
    }
}
