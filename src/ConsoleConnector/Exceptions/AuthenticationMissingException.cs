using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk.DataExchange.ConsoleApp.Exceptions
{
    public class AuthenticationMissingException: Exception
    {
        public AuthenticationMissingException(string message) 
            : base(message)
        {

        }
    }
}
