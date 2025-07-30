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
                    "╔══════════════════════════════════════════════════════════════════╗\n" +
                    "║                >> Autodesk Data Exchange CLI <<                  ║\n" +
                    "║                   DataExchange SDK Integration                   ║\n" +
                    "╚══════════════════════════════════════════════════════════════════╝\n\n" +
                    
                    "[OVERVIEW]\n" +
                    "   This is a sample console connector that demonstrates how to use\n" +
                    "   the Autodesk Data Exchange SDK. It serves as a reference implementation\n" +
                    "   for developers building their own Data Exchange integrations.\n\n" +                  
                    
                    "[GETTING STARTED]\n" +
                    "   > Type 'help' to display all available commands\n" +
                    "   > Type 'help [command]' for detailed usage information\n" +
                    "   > Type 'WorkFlowTest' to run complete integration demo\n\n" +
                    
                    "────────────────────────────────────────────────────────────────────\n");
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
                            Console.WriteLine($"[ERROR] Command '{input}' not found");
                        else
                            await command.Execute();
                    }
                    catch (Exception e)
                    {
                        _consoleAppHelper.Logger?.Error(e.Message, "An error occurred while executing the command.");
                        Console.WriteLine($"[ERROR] {e}");
                    }

                }
            }
            catch (AuthenticationMissingException authenticationMissingException)
            {
                _consoleAppHelper.Logger?.Error(authenticationMissingException.Message, "An error occurred while executing the command.");
                Console.WriteLine($"[AUTH ERROR] {authenticationMissingException.Message}");
                Console.ReadKey();
            }
            catch (Exception a)
            {
                _consoleAppHelper.Logger?.Error(a.Message, "An error occurred while executing the command.");
                Console.WriteLine($"[APP ERROR] {a}");
                Console.ReadKey();
            }
        }
    }    
}
