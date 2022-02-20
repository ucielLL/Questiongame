using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.Cosmos.Table;
using TheMessage;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace Funtions
{
    public static class InitGameFuntion
    {
        [FunctionName("InitGameFuntion")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "initGame")] HttpRequest req,
            [SignalR(ConnectionStringSetting = "AzureSignalRConnectionString", HubName = "Questios")] IAsyncCollector<SignalRMessage> collector,
            [Table("questiosn", Connection = "StorageConection")] CloudTable table,
            ILogger log)
        {
            MemoryStream ms = new MemoryStream();
            req.Body.CopyTo(ms);
            var initGame = JsonSerializer.Deserialize<InitGame>(new ReadOnlySpan<byte>(ms.ToArray()), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            Random r = new Random();
            // TableOperation Operation;
            var questions = new List<models.Question>();
            //var questions = new List<Question>();
            for (int i = 0; i < 10; i++)
            {
                var id = r.Next(1, 13).ToString();
                var Operation = TableOperation.Retrieve<models.Question>(initGame.Category, id);
                var result = await table.ExecuteAsync(Operation);
                questions.Add(result.Result as models.Question);
            }
            initGame.Questions = questions?.Select(a=> 
            {
                return new Question()
                {
                    TheQuestion = a.TheQuestion,
                    OptionA = a.OptionA,
                    OptionB = a.OptionB,
                    OptionC = a.OptionC,
                    Answer = a.Answer,
                    NumberQuestion = a.NumberQuestion
                };
            }).ToList();
            var init1 = new SignalRMessage
            {
                UserId = initGame.Sender,
                Target = "OnSyncUsers",
                Arguments = new object[] {initGame}
            };
            var init2 = new SignalRMessage
            {
                UserId = initGame.Receiver,
                Target = "OnSyncUsers",
                Arguments = new object[] { initGame }
            };
            await collector.AddAsync(init1);
            await collector.AddAsync(init2);
            

        }
    }
}
