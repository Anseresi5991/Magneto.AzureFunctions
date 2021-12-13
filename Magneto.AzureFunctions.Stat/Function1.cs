using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;

namespace Magneto.AzureFunctions.Stat
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }
        [FunctionName("Stats")]
        public static async Task<IActionResult> Stat(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            log.LogInformation("C# HTTP trigger function processed a request.");

            int countHumans = await GetCountMutants();
            int countMutants = await GetCountHumans();
            int total = countHumans + countMutants;
            decimal ratio = (decimal)countMutants / (decimal)total;
            var response = new Response(countMutants, countHumans, ratio);

           return new OkObjectResult(response);
        }
        public static async Task<int> GetCountMutants()
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(
            new MongoUrl(@Environment.GetEnvironmentVariable("ConDb"))
            );
            settings.SslSettings =
            new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            IMongoClient _mongoClient = new MongoClient(settings);
            var database = _mongoClient.GetDatabase("Magneto");
            var collection = database.GetCollection<Human>("Human");
            List<Human> humans = await collection.FindAsync(x => true).Result.ToListAsync();
            int countHumans = humans.Count();
            return countHumans;
        }
        public static async Task<int> GetCountHumans()
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(
            new MongoUrl(@Environment.GetEnvironmentVariable("ConDb"))
            );
            settings.SslSettings =
            new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            IMongoClient _mongoClient = new MongoClient(settings);
            var database = _mongoClient.GetDatabase("Magneto");
            var collection = database.GetCollection<Mutant>("Mutant");
            List<Mutant> mutants = await collection.FindAsync(x => true).Result.ToListAsync();
            int countMutants = mutants.Count();
            return countMutants;
        }
    }
}
