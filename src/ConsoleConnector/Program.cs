using Autodesk.DataExchange.ConsoleApp.Exceptions;
using Autodesk.DataExchange.ConsoleApp.Helper;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Autodesk.DataExchange.ConsoleApp
{
    class Program
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

        static Program()
        {

        }

        static IConsoleAppHelper _consoleAppHelper;
        static async Task Main(string[] args)
        {
            try
            {                
                Console.Title = "Console Connector";
                IntPtr handle = FindWindowByCaption(IntPtr.Zero, Console.Title);

                //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                _consoleAppHelper = new ConsoleAppHelper();
                _consoleAppHelper.Start();

                SetForegroundWindow(handle);
                
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
            catch (AuthenticationMissingException authenticationMissingException)
            {
                Console.WriteLine(authenticationMissingException.Message);
                Console.ReadKey();
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
