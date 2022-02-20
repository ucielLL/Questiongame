using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using TheMessage;

namespace Funtions
{
    public static class SyncUsersFuntion
    {
        [FunctionName("SyncUsers")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [SignalR(ConnectionStringSetting = "AzureSignalRConnectionString", HubName = "Questios")] IAsyncCollector<SignalRMessage> asyncCollectorMessage,
            ILogger log)
        {
            MemoryStream ms = new MemoryStream();
            req.Body.CopyTo(ms);
           
            //var syncUser = JsonSerializer.Deserialize<SyncUsers>(new StreamReader(req.Body.ReadToEnd());
            // var data = JsonDocument.Parse(ms);
            /*
            var data = JsonDocument.Parse(new System.Buffers.ReadOnlySequence<byte>(ms.ToArray()));
            var type = data.RootElement.GetProperty("Typeobject").ToString();
            */
            
                var msg = JsonSerializer.Deserialize<SyncUsers>(new ReadOnlySpan<byte>(ms.ToArray()), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
           
            await asyncCollectorMessage.AddAsync( new SignalRMessage
            {
                UserId = msg.Receiver,
            Target = "OnSyncUsers",
            Arguments = new object[] { msg }
            });

            /*
            var syncUser = JsonSerializer.Deserialize<object>(new ReadOnlySpan<byte>(ms.ToArray()));
            MessageBase m = (MessageBase)syncUser;  
            */
        }
    }
}
