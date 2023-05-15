// next event coming

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HtmlAgilityPack;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Company.Function
{
    public class BostonAzureEventsFunction {
        private readonly ILogger<BostonAzureEventsFunction> _logger;

        public BostonAzureEventsFunction(ILogger<BostonAzureEventsFunction> log) {
            _logger = log;
        }

        [FunctionName("BostonAzureEventsFunction")]
        [OpenApiOperation(operationId: "Run")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req) {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            Regex _regex = new Regex("[^0-9]");

            string name = req.Query["name"];

            List<GroupEvent> groupEvents = new List<GroupEvent>();

            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc = web.Load("https://www.meetup.com/bostonazure/events");

            // extract each event id, link, and title
            foreach (HtmlNode eventDiv in doc.DocumentNode.SelectNodes("//div[@class='card card--hasHoverShadow eventCard border--none border--none buttonPersonality']")) {

                var eventNodes = eventDiv.SelectNodes("//a[@class='eventCard--link']");
                for (int i = 0; i < eventNodes.Count; i++) {
                    GroupEvent groupEvent = new GroupEvent();

                    groupEvent.Title = eventNodes[i].InnerText;

                    HtmlAttribute eventLink = eventNodes[i].Attributes["href"];
                    if (eventLink.Value.Contains("a")) {
                        groupEvent.Link = eventLink.Value;
                    }

                    if (int.TryParse(_regex.Replace(eventLink.Value, ""), out var result)) {
                        groupEvent.Id = result;
                    }

                    groupEvent.EventTypeId = groupEvent.Title.Contains("virtual", StringComparison.OrdinalIgnoreCase) ? EventType.VBA : EventType.NBA;
                    groupEvent.EventTypeName = groupEvent.Title.Contains("virtual", StringComparison.OrdinalIgnoreCase) ? EventType.VBA.ToString() : EventType.NBA.ToString();

                    groupEvents.Add(groupEvent);
                }
                break;
            }

            return new OkObjectResult(groupEvents);
        }
    }

    public static class BostonAzureEventsStaticFunction {
        [FunctionName("BostonAzureEventsStaticFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log) {
            Regex _regex = new Regex("[^0-9]");

            List<GroupEvent> groupEvents = new List<GroupEvent>();

            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc = web.Load("https://www.meetup.com/bostonazure/events");

            // extract each event id, link, and title
            foreach (HtmlNode eventDiv in doc.DocumentNode.SelectNodes("//div[@class='card card--hasHoverShadow eventCard border--none border--none buttonPersonality']")) {

                var eventNodes = eventDiv.SelectNodes("//a[@class='eventCard--link']");
                for (int i = 0; i < eventNodes.Count; i++) {
                    GroupEvent groupEvent = new GroupEvent();

                    groupEvent.Title = eventNodes[i].InnerText;

                    HtmlAttribute eventLink = eventNodes[i].Attributes["href"];
                    if (eventLink.Value.Contains("a")) {
                        groupEvent.Link = eventLink.Value;
                    }

                    if (int.TryParse(_regex.Replace(eventLink.Value, ""), out var result)) {
                        groupEvent.Id = result;
                    }

                    groupEvent.EventTypeId = groupEvent.Title.Contains("virtual", StringComparison.OrdinalIgnoreCase) ? EventType.VBA : EventType.NBA;
                    groupEvent.EventTypeName = groupEvent.Title.Contains("virtual", StringComparison.OrdinalIgnoreCase) ? EventType.VBA.ToString() : EventType.NBA.ToString();

                    groupEvents.Add(groupEvent);
                }
                break;
            }

            return new OkObjectResult(groupEvents);
        }
    }
}
