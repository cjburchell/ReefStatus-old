namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System.Collections.Generic;

    using RedPoint.ReefStatus.Common.ProfiLux.Data;

    public interface IController
    {
        /// <summary>
        /// Gets or sets the Controller info.
        /// </summary>
        /// <value>The info.</value>
        Info Info { get; }

        List<ProgramableLogic> ProgrammableLogic { get; }

        List<CurrentPump> Pumps { get; }

        List<DosingPump> DosingPumps { get; }

        List<DigitalInput> DigitalInputs { get; }

        List<LPort> LPorts { get; }

        List<SPort> SPorts { get; } 

        List<LevelSensor> LevelSensors { get; }

        List<Light> Lights { get; }

        List<Probe> Probes { get; }
    }
}