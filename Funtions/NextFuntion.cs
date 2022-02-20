using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Text.Json;
using TheMessage;
namespace Funtions
{
    public static class NextFuntion
    {
        [FunctionName("NextFuntion")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "next")] HttpRequest req,
            [SignalR(ConnectionStringSetting = "AzureSignalRConnectionString", HubName = "Questios") ] IAsyncCollector<SignalRMessage> asyncCollector,
            ILogger log)
        {
            MemoryStream Ms = new MemoryStream();
            req.Body.CopyTo(Ms);
            var data = JsonSerializer.Deserialize<NextQuestion>(new ReadOnlySpan<byte>(Ms.ToArray()), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

          await  asyncCollector.AddAsync(new SignalRMessage 
            {
                UserId = data.Receiver,
                Target ="OnNextQuestion",
                Arguments = new object[] {data }
            });
        }
    }
}
