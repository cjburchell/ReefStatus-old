using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redpoint.ReefStatus.Server
{
    public class ConsoleApp
    {
        public static void Main()
        {
            var server = new Server();

            server.Initialize();

            Console.ReadLine();

            server.Shutdown();
        }
    }
}
