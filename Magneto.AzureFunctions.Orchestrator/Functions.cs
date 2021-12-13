using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using System.Net.Http;

namespace Magneto.AzureFunctions.Orchestrator
{
    public static class Functions
    {
        [FunctionName("Mutant")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            QueueDna data = JsonConvert.DeserializeObject<QueueDna>(requestBody);
            try
            {
                data.IsMutant = await IsMutant(requestBody);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
            PublishMessage(data);
            return (data.IsMutant ? new HttpResponseMessage(System.Net.HttpStatusCode.OK) : new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden));
        }
        public static async Task<bool> IsMutant(string json)
        {
            string url = Environment.GetEnvironmentVariable("FValidator");
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;

                    using (var response = await client
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        return true;
                    }
                }
            }
        }
        public static void PublishMessage(QueueDna request)
        {
            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("HostName"),
                VirtualHost = Environment.GetEnvironmentVariable("VHost"),
                Password = Environment.GetEnvironmentVariable("Password"),
                UserName = Environment.GetEnvironmentVariable("User")
            };
            IConnection conn = factory.CreateConnection();
            var channel = conn.CreateModel();
            var message = JsonConvert.SerializeObject(request);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(Environment.GetEnvironmentVariable("QueueExchange"), ExchangeType.Direct, null, body);
        }
    }
}
