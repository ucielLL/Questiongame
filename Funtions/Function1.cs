using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Description;

namespace Funtions
{
    public static class FuntionLogin
    {
        [FunctionName("Negotiate")]

        public static SignalRConnectionInfo Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Negotiate/{UserId}")] HttpRequest req,
          [SignalRConnectionInfo(ConnectionStringSetting = "AzureSignalRConnectionString", HubName = "Questios", UserId = "{UserId}")] SignalRConnectionInfo connectionInfo,
            ILogger log)
        {
            return connectionInfo;
        }
    }
}
