using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedPoint.ReefStatus.Common.WebServer
{
    public class Reminder
    {
        public int Index { get; set; }

        public string Text { get; set; }

        public bool IsOverdue { get; set; }

        public string Date { get; set; }
    }
}
