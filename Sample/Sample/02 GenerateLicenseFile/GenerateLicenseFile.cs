using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Sample
{
    public static class GenerateLicenseFile
    {
        // [Blob("licenses/{rand-guid}.lic")]TextWriter outputBolob (Blob Output Binding)

        [FunctionName("GenerateLicenseFile")]
        public static async Task Run(
            [QueueTrigger("orders", Connection = "AzureWebJobsStorage")] Order order,
            IBinder binder,
            ILogger log)
        {
            var outputBolob = await binder.BindAsync<TextWriter>(
                new BlobAttribute(blobPath: $"licenses/{order.OrderId}.lic")
                {
                    Connection = "AzureWebJobsStorage"
                });

            outputBolob.WriteLine($"OrderId: {order.OrderId}");
            outputBolob.WriteLine($"ProductId: {order.ProductId}");
            outputBolob.WriteLine($"Email: {order.Email}");
            outputBolob.WriteLine($"Price: {order.Price}");

            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(order.Email + "secret"));
            outputBolob.WriteLine($"SecretCode: {BitConverter.ToString(hash).Replace("-", "")}");

            log.LogInformation($"License Generated for the Mail Id: {order.Email}.");
        }
    }
}
