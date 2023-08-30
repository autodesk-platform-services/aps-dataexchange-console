using Autodesk.DataExchange;
using Autodesk.DataExchange.Authentication;
using Autodesk.DataExchange.Core.Enums;
using Autodesk.DataExchange.Core.Interface;
using Autodesk.DataExchange.Core.Models;
using Autodesk.DataExchange.Extensions.Logging.File;
using Autodesk.DataExchange.Extensions.Storage.File;
using Autodesk.DataExchange.Interface;
using Autodesk.DataExchange.Models;
using Autodesk.DataExchange.Models.Revit;
using Autodesk.DataExchange.SchemaObjects.Units;
using Autodesk.GeometryPrimitives.Design;
using Autodesk.GeometryPrimitives.Geometry;
using Autodesk.Parameters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.DataExchange.ConsoleApp.Helper;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using PrimitiveGeometry = Autodesk.GeometryPrimitives;

namespace Autodesk.DataExchange.ConsoleApp
{
    class Program
    {
        static Program()
        {

        }

        static IConsoleAppHelper _consoleAppHelper;
        static async Task Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                _consoleAppHelper = new ConsoleAppHelper();
                _consoleAppHelper.Start();

                Console.Clear();
                Console.WriteLine(
                    "The Data Exchange CLI is a sample application that demonstrates usage of the Data Exchange SDK and the various operations like loading exchanges, creating exchanges, updating and exchange with various types of data, etc.\nPlease type help to get a list of all the commands supported. \nType help commandName to get detailed information on a specific command.\n");
                while (true)
                {
                    try
                    {
                        Console.Write(">>");
                        var input = Console.ReadLine();
                        if (string.IsNullOrEmpty(input))
                            continue;

                        var command = _consoleAppHelper.GetCommand(input);
                        if (command == null)
                            Console.WriteLine(input + " command is not found.");
                        else
                            await command.Execute();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                }
            }
            catch (Exception a)
            {
                Console.WriteLine(a);
                Console.ReadKey();
            }
            finally
            {

            }
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var dllName = GetAssemblyName(args) + ".dll";
                var currentAssemblyPath = new System.Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                currentAssemblyPath = Path.GetDirectoryName(currentAssemblyPath);
                if (currentAssemblyPath != null && File.Exists(Path.Combine(currentAssemblyPath, dllName)))
                {
                    return Assembly.LoadFile(Path.Combine(currentAssemblyPath, dllName));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        private static string GetAssemblyName(ResolveEventArgs args)
        {
            var name = args.Name.IndexOf(",", StringComparison.Ordinal) > -1 ? args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal)) : args.Name;
            return name;
        }

    }    
}
