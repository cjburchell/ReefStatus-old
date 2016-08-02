

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// Controller Class
    /// </summary>
    public class Controller
    {
        [JsonProperty("_id")]
        public string Id { get; set; } = "controller";

        [JsonProperty("_rev")]
        public string Rev { get; set; }

        /// <summary>
        /// Gets or sets the Controller info.
        /// </summary>
        /// <value>The info.</value>
        public Info Info { get; set; } = new Info();

        /// <summary>
        /// Gets or sets the programmable logic.
        /// </summary>
        /// <value>
        /// The programmable logic.
        /// </value>
        public List<ProgramableLogic> ProgrammableLogic { get; set; } = new List<ProgramableLogic>();

        public List<CurrentPump> Pumps { get; set; } = new List<CurrentPump>();

        public List<DosingPump> DosingPumps { get; set; } = new List<DosingPump>();

        public List<DigitalInput> DigitalInputs { get; set; } = new List<DigitalInput>();

        public List<LPort> LPorts { get; set; } = new List<LPort>();

        public List<SPort> SPorts { get; set; } = new List<SPort>();

        public List<LevelSensor> LevelSensors { get; set; } = new List<LevelSensor>();

        public List<Light> Lights { get; set; } = new List<Light>();

        public List<Probe> Probes { get; set; } = new List<Probe>();

        public List<UserInfo> UserInfo { get; set; } = new List<UserInfo>();
    }
}