using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MongoDB.Driver;
using Magneto.AzureFunctions.Metrics;
using System.Collections.Generic;
using System.Linq;

namespace Magneto.AzureFunctions.Metrics
{
    public static class Function
    {

        [FunctionName("Stats")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            int countHumans = await GetCountMutants();
            int countMutants = await GetCountHumans();
            int total = countHumans + countMutants;
            decimal ratio = (decimal)countMutants / (decimal)total;
            var response = new Response(countMutants,countHumans,ratio);

            return new OkObjectResult(response);
        }
        public static async Task<int> GetCountMutants()
        {
            IMongoClient _mongoClient = new MongoClient(Environment.GetEnvironmentVariable("ConDb"));
            var database = _mongoClient.GetDatabase("Magneto");
            var collection = database.GetCollection<Human>("Human");
            List<Human> humans = await collection.FindAsync(x => true).Result.ToListAsync();
            int countHumans = humans.Count();
            return countHumans;
        }
        public static async Task<int> GetCountHumans()
        {
            IMongoClient _mongoClient = new MongoClient(Environment.GetEnvironmentVariable("ConDb"));
            var database = _mongoClient.GetDatabase("Magneto");
            var collection = database.GetCollection<Mutant>("Mutant");
            List<Mutant> mutants = await collection.FindAsync(x => true).Result.ToListAsync();
            int countMutants = mutants.Count();
            return countMutants;
        }
    }
}
