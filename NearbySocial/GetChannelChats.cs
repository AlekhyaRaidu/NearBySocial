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
    public class Chat
    {
        public string channel;
        public string chatContent;
        public string userId;
    }
    public static class GetChannelChats
    {
        [FunctionName("GetChannelChats")]
        public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
    [CosmosDB(ConnectionStringSetting = "testnbs_DOCUMENTDB")] DocumentClient client,
    ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string channelId = req.Query["channel"]; 

            Uri chatCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId: "nbsDev", collectionId: "chatCollection");


            IDocumentQuery<Chat> query = client.CreateDocumentQuery<Chat>(chatCollectionUri)
                                                 .Where(chat => chat.channel == channelId)
                                                 .AsDocumentQuery();

            var chatForStore = new List<dynamic>();

            while (query.HasMoreResults)
            {
                foreach (dynamic chat in await query.ExecuteNextAsync())
                {
                    chatForStore.Add(chat);
                }
            }

            return new OkObjectResult(chatForStore);
        }
    }
}



