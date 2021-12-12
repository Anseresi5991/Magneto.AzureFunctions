using System.Net;
using System.Net.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
namespace Magneto.AzureFunctions.Validator.AfValidator
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("Function1");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK, "Welcome to Azure Functions!");
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            return response;
        }
    }
}
