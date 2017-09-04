
namespace RedPoint.ReefStatus.Common.WebServer
{
    using System.Collections.Generic;
    using System.Linq;

    using HttpServer;
    using HttpServer.Exceptions;
    using HttpServer.MVC.Controllers;

    using Newtonsoft.Json;

    using ProfiLux.Data;

    using RedPoint.ReefStatus.Common.ProfiLux;

    public class ControllerController : RequestController
    {
        private readonly IController controller;

        public ControllerController(IController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Make a clone of this controller
        /// </summary>
        /// <returns>a new controller with the same base information as this one.</returns>
        public override object Clone()
        {
            return new ControllerController(this.controller);
        }

        public string Info()
        {
            if (this.Request.Method != Method.Get)
            {
                throw new BadRequestException("Only get accepted");
            }

            return this.FormatResult(this.controller.Info);
        }

        private string FormatResult<T>(T result)
        {
            this.Response.ContentType = "application/json";
            var data = JsonConvert.SerializeObject(result, Formatting.None);
            return data;
        }

        public string Pump()
        {
            return DataRequest(this.controller.Pumps);
        }

        public string LevelSensor()
        {
            return DataRequest(this.controller.LevelSensors);
        }

        public string DigitalInput()
        {
            return DataRequest(this.controller.DigitalInputs);
        }

        public string DosingPump()
        {
            return DataRequest(this.controller.DosingPumps);
        }

        public string LPort()
        {
            return DataRequest(this.controller.LPorts);
        }

        public string SPort()
        {
            return DataRequest(this.controller.SPorts);
        }

        public string Light()
        {
            return DataRequest(this.controller.Lights);
        }

        public string Probe()
        {
            return DataRequest(this.controller.Probes);
        }

        public string ProgrammableLogic()
        {
            if (this.Request.Method != Method.Get)
            {
                throw new BadRequestException("Only get accepted");
            }

            if (string.IsNullOrEmpty(this.Id))
            {
                return JsonConvert.SerializeObject(this.controller.ProgrammableLogic.ToList());
            }

            var infoItem = this.controller.ProgrammableLogic.FirstOrDefault(item => item.Index.ToString() == this.Id);
            if (infoItem == null)
            {
                throw new BadRequestException("Id not found");
            }

            return this.FormatResult(infoItem);
        }

        private string DataRequest<T>(IEnumerable<T> items) where T : BaseInfo
        {
            if (this.Request.Method != Method.Get)
            {
                throw new BadRequestException("Only get accepted");
            }

            if (string.IsNullOrEmpty(this.Id))
            {
                return JsonConvert.SerializeObject(items.ToList());
            }

            var infoItem = items.FirstOrDefault(item => item.Id == this.Id);
            if (infoItem == null)
            {
                throw new BadRequestException("Id not found");
            }

            return this.FormatResult(infoItem);
        }
    }
}
