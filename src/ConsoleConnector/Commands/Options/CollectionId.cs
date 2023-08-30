using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk.DataExchange.ConsoleApp.Commands.Options
{
    internal class CollectionId: CommandOption
    {
        public CollectionId()
        {
            this.Description = "Specify the collection id.";
        }

        public override string ToString()
        {
            return "CollectionId[" + Description + "]";
        }
    }
}
