using Autodesk.DataExchange.ConsoleApp.Commands.Options;
using Autodesk.DataExchange.ConsoleApp.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Autodesk.DataExchange.ConsoleApp.Commands
{
    /// <summary>
    /// Abstract class for commands. 
    /// </summary>
    internal abstract class Command
    {
        /// <summary>
        /// Command name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Command description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Command parameters
        /// </summary>
        public List<CommandOption> Options { get; set; }
        internal IConsoleAppHelper ConsoleAppHelper { get; set; }

        protected Command()
        {
            Options = new List<CommandOption>();
        }

        protected Command(IConsoleAppHelper consoleAppHelper):this()
        {
            this.ConsoleAppHelper = consoleAppHelper;
        }


        protected Command(Command command) : this()
        {
            this.ConsoleAppHelper = command.ConsoleAppHelper;
            this.Name = command.Name;
            this.Description = command.Description;
            this.Options = command.Options.Select(n=>n).ToList();
        }

        public virtual Task<bool> Execute()
        {
            return Task.FromResult(true);
        }

        public abstract Command Clone();

        public T GetOption<T>()
        {
            return (T)(object)Options.FirstOrDefault(n => n.GetType() == typeof(T));
        }

        public virtual bool ValidateOptions()
        {
            foreach (var option in this.Options)  
            {
                if (option.IsValid() == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
