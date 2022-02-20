using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Funtions.models;
using System.Text.Json;

namespace Funtions
{
    public static class UserStateFuntion
    {
        [FunctionName("UserStateFuntion")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UserState")] HttpRequest req,
            [Table("Users", Connection = "StorageConection")] CloudTable table,
            ILogger log)
        {
            MemoryStream ms = new MemoryStream();
            req.Body.CopyTo(ms);
            var user = JsonSerializer.Deserialize<User>(new ReadOnlySpan<byte>(ms.ToArray()), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            user.RowKey = user.UserName;
            user.PartitionKey = user.Language;
            user.LastGameDate = DateTime.Now;
            user.ETag = "*";
            if (user != null)
            {
                TableOperation Update = TableOperation.Replace(user);
               await table.ExecuteAsync(Update);
            }
        }
    }
}
