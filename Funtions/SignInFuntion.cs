using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Microsoft.WindowsAzure.Storage.Table;
using Funtions.models;
using System.Collections.Generic;
using System.Text.Json;

namespace Funtions
{
    public static class SignInFuntion
    {
        [FunctionName("SignInFuntion")]
        [StorageAccount("StorageConection")]
        [return: Table("Users")]
        public static async Task<User> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "SignIn")] HttpRequest req,
            ILogger log)
        {
            MemoryStream ms = new MemoryStream();
            req.Body.CopyTo(ms);
            var user = JsonSerializer.Deserialize<User>(new ReadOnlySpan<byte>(ms.ToArray()), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            user.Estado = "wait";
            user.RowKey = user.UserName;
            user.PartitionKey = user.Language;
            user.LastGameDate = DateTime.Now;

            return user;
        }
    }
}
