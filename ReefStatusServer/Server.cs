using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redpoint.ReefStatus.Server
{
    using log4net;

    public class Server
    {
        private readonly ILog log = LogManager.GetLogger("wombat");

        public void Initialize()
        {
            log.Debug("Starting Server");
            // create and start web server
        }

        public void Shutdown()
        {
            log.Debug("Stopping Server");
        }
    }
}
