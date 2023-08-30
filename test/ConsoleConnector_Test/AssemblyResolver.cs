using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleConnector_Test
{
    internal class AssemblyResolver
    {
        public AssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var dllName = GetAssemblyName(args) + ".dll";
                if (dllName == "Autodesk.GeometryUtilities.dll" || dllName == "Autodesk.GeometryPrimitives.dll" ||
                    dllName == "Autodesk.DataExchange.OpenAPITools.dll")
                {
                    var currentAssemblyPath = new System.Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                    currentAssemblyPath = Path.GetDirectoryName(currentAssemblyPath);
                    if (File.Exists(Path.Combine(currentAssemblyPath, "FDXToCollab", dllName)))
                    {
                        //Logging._logger?.Information(dllName + " is loading.");
                        return Assembly.LoadFile(Path.Combine(currentAssemblyPath, "FDXToCollab", dllName));
                    }

                }
            }
            catch (Exception ex)
            {
                //Logging._logger?.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Get assembly name
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private string GetAssemblyName(ResolveEventArgs args)
        {
            string name;
            if (args.Name.IndexOf(",") > -1)
            {
                name = args.Name.Substring(0, args.Name.IndexOf(","));
            }
            else
            {
                name = args.Name;
            }
            return name;
        }
    }
}
