using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Funtions.models;
using Microsoft.Azure.Cosmos.Table;


namespace Funtions
{
    public static class SearchUserFuntion
    {
        [FunctionName("SearchUserFuntion")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/{userName}/{languge}/{israndom}")] HttpRequest req,
            [Table("Users",Connection = "StorageConection")] CloudTable table, string userName,string languge, bool israndom ,
            ILogger log)
        {
            if (israndom) 
            {
                TableQuery<User> rangequry =
             new TableQuery<User>();
                string filer1 = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, languge);
                string filter2 = TableQuery.GenerateFilterCondition("Estado", QueryComparisons.Equal, "wait");
                rangequry.Where(TableQuery.CombineFilters(filer1, TableOperators.And, filter2)).Take(4);
                var users = new List<User>();

                foreach (var entity in
                    await table.ExecuteQuerySegmentedAsync(rangequry, null))
                {
                    users.Add(entity);
                }
                return new OkObjectResult(users);
            }
            
             TableOperation retrieveOperation = TableOperation.Retrieve<User>(languge, userName);
              TableResult result = await table.ExecuteAsync(retrieveOperation);
              User Ustomer = result.Result as User;
            
            return new OkObjectResult(Ustomer);

        }
    }
}
