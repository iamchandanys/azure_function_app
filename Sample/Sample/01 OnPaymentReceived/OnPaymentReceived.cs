using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Sample
{
    public static class OnPaymentReceived
    {
        [FunctionName("OnPaymentReceived")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Queue("orders")] IAsyncCollector<Order> orderQueue,
            [Table("orders")] IAsyncCollector<Order> orderTable,
            ILogger log)
        {
            try
            {
                log.LogInformation("Payment Received!");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var order = JsonConvert.DeserializeObject<Order>(requestBody);

                log.LogInformation(order.Email);

                // Queue Output Binding
                await orderQueue.AddAsync(order);

                log.LogInformation($"Order {order.OrderId} added in Queue.");

                order.PartitionKey = "orders";
                order.RowKey = order.OrderId;

                // Table Output Binding
                await orderTable.AddAsync(order);

                log.LogInformation($"Order {order.OrderId} added in Table Storage.");

                return new OkObjectResult("Thank You for Purchase. Happy Day!");
            }
            catch(Exception ex)
            {
                log.LogError(ex.Message);

                return new BadRequestObjectResult(ex.Message);
            }
        }
    }

    public class Order
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Email { get; set; }
        public string Price { get; set; }
    }
}
