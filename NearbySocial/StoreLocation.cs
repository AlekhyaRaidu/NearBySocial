using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;

namespace NearbySocial
{
    public class StoreLocation
    {
        [FunctionName("StoreLocation")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            out object taskDocument, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            taskDocument = null;
            
            if (name == null)
            {
                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }

            if (data == null) {
                return new BadRequestObjectResult("Please pass data in the request body");
            }

            string userId = data.UserId;
            string xAxis = data.x;
            string yAxis = data.y;
            string channel = data.channel;

            if (AnyNullOrEmpty(userId, xAxis, yAxis, channel))
            {
                return new BadRequestObjectResult("Please pass data in the request body");
            }
            else
            {
                taskDocument = new
                {
                    userId,
                    xAxis,
                    yAxis,
                    channel
                };

                return new OkResult();
            }
        }

        public static bool AnyNullOrEmpty(params string[] strings)
        {
            return strings.Any(s => string.IsNullOrEmpty(s));
        }
    }
}
