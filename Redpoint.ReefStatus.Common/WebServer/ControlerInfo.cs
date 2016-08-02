using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedPoint.ReefStatus.Common.WebServer
{
    public class ControllerInfo
    {
        public string Alarm { get; set; }

        public string LastUpdateString { get; set; }

        public string OperationMode { get; set; }

        public int ModeId { get; set; }

        public string SerialNumber { get; set; }

        public string Model { get; set; }

        public string SoftwareDate { get; set; }

        public string SoftwareVersion { get; set; }

        public string MoonPhase { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public int ReminderCount { get; set; }
        public int ProbeCount { get; set; }
        public int LevelSensorCount { get; set; }
        public int DigitalInputCount { get; set; }
        public int SPortCount { get; set; }
        public int LPortCount { get; set; }
        public int LightCount { get; set; }
        public int DosingPumpCount { get; set; }

        public int UserValueCount { get; set; }

        public bool ProbeAlarm { get; set; }
        public bool LevelAlarm { get; set; }
        public bool ReminderOverdue { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
