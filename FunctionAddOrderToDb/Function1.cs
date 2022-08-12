using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FunctionAddOrderToDb.Models;

namespace FunctionAddOrderToDb
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] 
                HttpRequest req,
            [Sql("dbo.Orders", ConnectionStringSetting ="SqlConnString")] 
                out Order order,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                order = JsonConvert.DeserializeObject<Order>(requestBody);
                return new OkObjectResult("success");
            }
            catch (Exception ex)
            {
                order = null;
                return new StatusCodeResult((int)StatusCodes.Status500InternalServerError);
            }            
        }
    }
}
