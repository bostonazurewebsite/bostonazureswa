using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class Hello
    {
        [FunctionName("Hello")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Hello invoked");

                // get day of week as a string
            var DayOfWeek = DateTime.Now.DayOfWeek.ToString();

            var responseMessage = $"{'message': 'Hello from Azure Functions this fine {DayOfWeek}!'}";

            return new OkObjectResult(responseMessage);
        }
    }
}
