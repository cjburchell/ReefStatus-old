namespace RedPoint.ReefStatus.Common.WebServer
{
    using System.Linq;
    using System.Text;

    using HttpServer;
    using HttpServer.Exceptions;
    using HttpServer.MVC.Controllers;

    using RedPoint.ReefStatus.Common.Database;

    public class DataController : RequestController
    {
        private readonly IDataAccess dataAccess;

        public DataController(IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        /// <summary>
        /// Make a clone of this controller
        /// </summary>
        /// <returns>a new controller with the same base information as this one.</returns>
        public override object Clone()
        {
            return new DataController(this.dataAccess);
        }

        private struct Paramaters
        {
            public bool? Descending { get; set; }

            public int? Limit { get; set; }

            public string Id { get; set; }
        }

        public string Log()
        {
            if (this.Request.Method != Method.Get)
            {
                throw new BadRequestException("Only get accepted");
            }

            if (string.IsNullOrEmpty(this.Id))
            {
                throw new BadRequestException("Id not found");
            }

            var paramaters = this.GetParamaters();

            var infoItem = this.dataAccess.GetRawDataPoints(paramaters.Id, paramaters.Limit, paramaters.Descending);
            if (infoItem == null)
            {
                throw new BadRequestException("Missing Id");
            }

            var builder = new StringBuilder();
            builder.Append("[");
            var firstItem = true;
            foreach (var item in infoItem)
            {
                if (firstItem)
                {
                    firstItem = false;
                }
                else
                {
                    builder.Append(",");
                }

                builder.Append(item);
            }


            builder.Append("]");

            this.Response.ContentType = "application/json";
            return builder.ToString();
        }

        private Paramaters GetParamaters()
        {
            var param = new Paramaters();

            var tokens = this.Id.Split('?');

            if (tokens.Length == 1)
            {
                param.Id = tokens[0];
            }
            else if (tokens.Length == 2)
            {
                param.Id = tokens[0];
                var paramList = tokens[1].Split('&').Where(item => item.Contains('=') && !item.StartsWith("=") && !item.EndsWith("=")).ToDictionary(item => item.Split('=')[0].ToLower(), item => item.Split('=')[1]);

                if (paramList.ContainsKey("limit"))
                {
                    int value;
                    if(int.TryParse(paramList["limit"], out value))
                    {
                        param.Limit = value;
                    }
                }

                if (paramList.ContainsKey("descending"))
                {
                    bool value;
                    if (bool.TryParse(paramList["descending"], out value))
                    {
                        param.Descending = value;
                    }
                }
            }
            else
            {
                throw new BadRequestException("Unknown Arguments");
            }

            return param;
        }
    }
}
