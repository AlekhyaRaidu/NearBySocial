using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq;
using System.Collections.Generic;

namespace NearbySocial
{
    public class User
    {
        public string userId;
        public string xAxis;
        public string yAxis;
        public string channel;
    }
    public static class GetChannelUsers
    {
        [FunctionName("GetChannelUsers")]
        public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
    [CosmosDB(ConnectionStringSetting = "testnbs_DOCUMENTDB")] DocumentClient client,
    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string channelId = req.Query["channel"];

            Uri chatCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId: "taskDatabase", collectionId: "taskCollection");


            IDocumentQuery<User> query = client.CreateDocumentQuery<User>(chatCollectionUri)
                                                 .Where(user => user.channel == channelId)
                                                 .AsDocumentQuery();

            var usersForStore = new List<dynamic>();

            while (query.HasMoreResults)
            {
                foreach (dynamic user in await query.ExecuteNextAsync())
                {
                    usersForStore.Add(user);
                }
            }

            return new OkObjectResult(usersForStore);
        }
    }
}



