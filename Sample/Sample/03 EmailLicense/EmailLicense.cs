using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace Sample
{
    public static class EmailLicense
    {
        [FunctionName("EmailLicense")]
        public static async Task Run(
            [BlobTrigger("licenses/{orderId}.lic", Connection = "AzureWebJobsStorage")]string licenseFileContents,
            [SendGrid(ApiKey = "SendGridApiKey")] ICollector<SendGridMessage> mailSender,
            IBinder binder,
            string orderId,
            ILogger log)
        {
            var order = await binder.BindAsync<Order>(
                new TableAttribute("orders", "orders", orderId)
                {
                    Connection = "AzureWebJobsStorage"
                });

            var message = new SendGridMessage();
            
            message.From = new EmailAddress(Environment.GetEnvironmentVariable("EmailSender"));
            message.AddTo(order.Email);

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(licenseFileContents);
            var base64 = Convert.ToBase64String(plainTextBytes);

            message.AddAttachment(filename: orderId + ".lic", content: base64, type: "text/plain");
            message.Subject = "Your license file";
            message.HtmlContent = "Thank you for your order";

            mailSender.Add(message);

            log.LogInformation("Email Sent Successfully!");
        }
    }
}
