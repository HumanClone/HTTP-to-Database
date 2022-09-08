using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Cloud_Ice
{
    public static class id
    {
        [FunctionName("Ice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "id/{id}")] HttpRequest req, string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string id = req.Query["id"];
            string num = id;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            num = num ?? data?.num;



            string responseMessage = string.IsNullOrEmpty(num)
                ? "This HTTP triggered function executed successfully. Pass an id in the query string or in the request body for a personalized response."
                : "ID not found in database";


            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                string command = "Insert Into Ids(Id) Values (" + id + ")";

                using (SqlCommand cmd = new SqlCommand(command, conn))
                {
                    // Execute the command and log the # rows affected.
                    var rows = await cmd.ExecuteNonQueryAsync();
                    log.LogInformation($"{rows} record inserted");
                    responseMessage = "inserted";
                }
            }


            return new OkObjectResult(responseMessage);
        }
    }
}
